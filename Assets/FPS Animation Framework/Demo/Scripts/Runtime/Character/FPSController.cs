// Designed by KINEMATION, 2024.

using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using KINEMATION.KAnimationCore.Runtime.Input;
using KINEMATION.KAnimationCore.Runtime.Rig;

using Demo.Scripts.Runtime.Item;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

using STARTING;

namespace Demo.Scripts.Runtime.Character
{
    public enum FPSAimState
    {
        None,
        Ready,
        Aiming,
        PointAiming
    }

    public enum FPSActionState
    {
        None,
        PlayingAnimation,
        WeaponChange,
        AttachmentEditing
    }

    [RequireComponent(typeof(CharacterController), typeof(FPSMovement))]
    public class FPSController : MonoBehaviour
    {
        //~ Legacy Controller Interface

        [SerializeField] private FPSControllerSettings settings;

        private FPSMovement _movementComponent;

        private Transform _weaponBone;
        private Vector2 _playerInput;

        private int _activeWeaponIndex;
        private int _previousWeaponIndex;

        private FPSAimState _aimState;
        private FPSActionState _actionState;

        private bool _isUnarmed;
        private Animator _animator;

        //~ Legacy Controller Interface

        // ~Scriptable Animation System Integration
        private FPSAnimator _fpsAnimator;
        private UserInputController _userInput;
        // ~Scriptable Animation System Integration

        public List<FPSItem> _instantiatedWeapons;
        private Vector2 _lookDeltaInput;

        private RecoilPattern _recoilPattern;
        private int _sensitivityMultiplierPropertyIndex;

        public event Action<FPSAimState> OnActiveAiming;

        [Header("달리기 화면 번짐 효과 관련")]
        public Volume localVolume;
        private VolumeProfile profile;
        private ChromaticAberration chromaticAberration;

        //무기변경 시스템
        private Inventory inventory;

        private void Awake()
        {
            inventory = this.gameObject.GetComponent<Inventory>();
        }

        private void PlayTransitionMotion(FPSAnimatorLayerSettings layerSettings)
        {
            if (layerSettings == null)
            {
                return;
            }

            _fpsAnimator.LinkAnimatorLayer(layerSettings);
        }

        private bool IsSprinting()
        {
            return _movementComponent.MovementState == FPSMovementState.Sprinting;
        }

        private bool HasActiveAction()
        {
            return _actionState != FPSActionState.None;
        }

        private bool IsAiming()
        {
            return _aimState is FPSAimState.Aiming or FPSAimState.PointAiming;
        }

        private void InitializeMovement()
        {
            _movementComponent = GetComponent<FPSMovement>();

            _movementComponent.onJump.AddListener(() => { PlayTransitionMotion(settings.jumpingMotion); });
            _movementComponent.onLanded.AddListener(() => { PlayTransitionMotion(settings.jumpingMotion); });

            _movementComponent.onCrouch.AddListener(OnCrouch);
            _movementComponent.onUncrouch.AddListener(OnUncrouch);

            _movementComponent.onSprintStarted.AddListener(OnSprintStarted);
            _movementComponent.onSprintEnded.AddListener(OnSprintEnded);

            _movementComponent.onSlideStarted.AddListener(OnSlideStarted);

            _movementComponent.slideCondition += () => !HasActiveAction();
            _movementComponent.sprintCondition += () => !HasActiveAction();
            _movementComponent.proneCondition += () => !HasActiveAction();

            _movementComponent.onStopMoving.AddListener(() =>
            {
                PlayTransitionMotion(settings.stopMotion);
            });

            _movementComponent.onProneEnded.AddListener(() =>
            {
                _userInput.SetValue(FPSANames.PlayablesWeight, 1f);
            });
        }

        private void InitializeWeapons()
        {
            _instantiatedWeapons = new List<FPSItem>();

            foreach (var prefab in settings.weaponPrefabs)
            {
                var weapon = Instantiate(prefab, transform.position, Quaternion.identity);

                var weaponTransform = weapon.transform;

                weaponTransform.parent = _weaponBone;
                weaponTransform.localPosition = Vector3.zero;
                weaponTransform.localRotation = Quaternion.identity;

                _instantiatedWeapons.Add(weapon.GetComponent<FPSItem>());
                weapon.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;

            _weaponBone = GetComponentInChildren<KRigComponent>().GetRigTransform(settings.weaponBone);
            _fpsAnimator = GetComponent<FPSAnimator>();
            _userInput = GetComponent<UserInputController>();
            _animator = GetComponent<Animator>();
            _recoilPattern = GetComponent<RecoilPattern>();

            InitializeMovement();
            InitializeWeapons();

            _actionState = FPSActionState.None;
            // 첫 번째 장착된 무기 자동 장착
            if (inventory.equippedWeaponIndices.Count > 0)
            {
                _activeWeaponIndex = inventory.equippedWeaponIndices[0]; // equippedWeaponIndices에서 첫 번째 인덱스 사용
                EquipWeapon();
            }
            else
            {
                Debug.LogWarning("장착된 무기가 없습니다.");
            }

            _sensitivityMultiplierPropertyIndex = _userInput.GetPropertyIndex("SensitivityMultiplier");

            //volume
            profile = localVolume.sharedProfile;
        }

        private void UnequipWeapon()
        {
            DisableAim();
            _actionState = FPSActionState.WeaponChange;
            GetActiveItem().OnUnEquip();
        }

        public void ResetActionState()
        {
            _actionState = FPSActionState.None;
        }

        private void EquipWeapon()
        {
            if (_instantiatedWeapons.Count == 0) return;

            _instantiatedWeapons[_previousWeaponIndex].gameObject.SetActive(false);
            GetActiveItem().gameObject.SetActive(true);
            GetActiveItem().OnEquip(gameObject);

            _actionState = FPSActionState.None;
        }

        private void DisableAim()
        {
            if (GetActiveItem().OnAimReleased()) _aimState = FPSAimState.None;
        }

        private void OnFirePressed()
        {
            if (_instantiatedWeapons.Count == 0 || HasActiveAction()) return;
            GetActiveItem().OnFirePressed();
        }

        private void OnFireReleased()
        {
            if (_instantiatedWeapons.Count == 0) return;
            GetActiveItem().OnFireReleased();
        }

        private FPSItem GetActiveItem()
        {
            if (_instantiatedWeapons.Count == 0) return null;
            return _instantiatedWeapons[_activeWeaponIndex];
        }

        private void OnSlideStarted()
        {
            _animator.CrossFade("Sliding", 0.1f);
        }

        private void OnSprintStarted()
        {
            OnFireReleased();
            DisableAim();

            _aimState = FPSAimState.None;

            _userInput.SetValue(FPSANames.StabilizationWeight, 0f);
            _userInput.SetValue(FPSANames.PlayablesWeight, 0f);
            _userInput.SetValue("LookLayerWeight", 0.3f);

            if (profile.TryGet(out ChromaticAberration chromaticAberration))
            {
                chromaticAberration.intensity.value = 0.5f;
            }
        }

        private void OnSprintEnded()
        {
            if (_animator.GetFloat("OverlayType") == 0) return;

            _userInput.SetValue(FPSANames.StabilizationWeight, 1f);
            _userInput.SetValue(FPSANames.PlayablesWeight, 1f);
            _userInput.SetValue("LookLayerWeight", 1f);

            if (profile.TryGet(out ChromaticAberration chromaticAberration))
            {
                chromaticAberration.intensity.value = 0f;
            }
        }

        private void OnCrouch()
        {
            PlayTransitionMotion(settings.crouchingMotion);
        }

        private void OnUncrouch()
        {
            PlayTransitionMotion(settings.crouchingMotion);
        }

        private bool _isLeaning;

        private void StartWeaponChange(int newIndex)
        {
            if (newIndex == _activeWeaponIndex || newIndex > _instantiatedWeapons.Count - 1)
            {
                return;
            }

            UnequipWeapon();

            OnFireReleased();
            Invoke(nameof(EquipWeapon), settings.equipDelay);

            _previousWeaponIndex = _activeWeaponIndex;
            _activeWeaponIndex = newIndex;
        }

        private void UpdateLookInput()
        {
            float scale = _userInput.GetValue<float>(_sensitivityMultiplierPropertyIndex);

            float deltaMouseX = _lookDeltaInput.x * settings.sensitivity * scale;
            float deltaMouseY = -_lookDeltaInput.y * settings.sensitivity * scale;

            _playerInput.y += deltaMouseY;
            _playerInput.x += deltaMouseX;

            if (_recoilPattern != null)
            {
                _playerInput += _recoilPattern.GetRecoilDelta();
                deltaMouseX += _recoilPattern.GetRecoilDelta().x;
            }

            float proneWeight = _animator.GetFloat("ProneWeight");
            Vector2 pitchClamp = Vector2.Lerp(new Vector2(-90f, 90f), new Vector2(-30, 0f), proneWeight);

            _playerInput.y = Mathf.Clamp(_playerInput.y, pitchClamp.x, pitchClamp.y);

            transform.rotation *= Quaternion.Euler(0f, deltaMouseX, 0f);

            _userInput.SetValue(FPSANames.MouseDeltaInput, new Vector4(deltaMouseX, deltaMouseY));
            _userInput.SetValue(FPSANames.MouseInput, new Vector4(_playerInput.x, _playerInput.y));
        }

        private void OnMovementUpdated()
        {
            if (_movementComponent.PoseState == FPSPoseState.Prone)
            {
                float targetWeight = _movementComponent.IsMoving() ? 0f : 1f;
                _userInput.SetValue(FPSANames.PlayablesWeight, targetWeight);
                _userInput.SetValue(FPSANames.StabilizationWeight, targetWeight);
            }
        }

        private void Update()
        {
            Time.timeScale = settings.timeScale;

            //Pause메뉴가 열려있으면 마우스회전 X
            if (PlayerStats.Instance.playerState == PlayerState.IDLE || PlayerStats.Instance.playerState == PlayerState.INIT)
            {
                UpdateLookInput();
                OnMovementUpdated();
            }
        }

#if ENABLE_INPUT_SYSTEM
        public void OnReload()
        {
            if (IsSprinting() || HasActiveAction() || !GetActiveItem().OnReload()) return;
            _actionState = FPSActionState.PlayingAnimation;
        }

        public void OnThrowGrenade()
        {
            if (IsSprinting() || HasActiveAction() || !GetActiveItem().OnGrenadeThrow()) return;
            _actionState = FPSActionState.PlayingAnimation;
        }

        public void OnToggleUnarmed()
        {
            _isUnarmed = !_isUnarmed;

            if (_isUnarmed)
            {
                GetActiveItem().gameObject.SetActive(false);
                GetActiveItem().OnUnarmedEnabled();
                _fpsAnimator.LinkAnimatorProfile(settings.unarmedProfile);
                return;
            }

            GetActiveItem().gameObject.SetActive(true);
            GetActiveItem().OnUnarmedDisabled();
        }

        public void OnFire(InputValue value)
        {
            if (IsSprinting()) return;

            //Pause메뉴가 열려있으면 총알발사 X
            if (value.isPressed && (PlayerStats.Instance.playerState == PlayerState.IDLE || PlayerStats.Instance.playerState == PlayerState.INIT))
            {
                OnFirePressed();
                return;
            }

            OnFireReleased();
        }

        public void OnAim(InputValue value)
        {
            if (IsSprinting()) return;

            if (value.isPressed && !IsAiming())
            {
                if (GetActiveItem().OnAimPressed()) _aimState = FPSAimState.Aiming;
                PlayTransitionMotion(settings.aimingMotion);
                //에임상태(줌) 변경될 때 이벤트 발생
                OnActiveAiming?.Invoke(_aimState);
                return;
            }

            if (!value.isPressed && IsAiming())
            {
                DisableAim();
                PlayTransitionMotion(settings.aimingMotion);
                //에임상태(줌) 변경될 때 이벤트 발생
                OnActiveAiming?.Invoke(_aimState);
            }
        }

        public void OnChangeWeapon1()
        {
            if (inventory.equippedWeaponIndices.Count > 0)
            {
                StartWeaponChange(inventory.equippedWeaponIndices[0]);
            }
        }

        public void OnChangeWeapon2()
        {
            if (inventory.equippedWeaponIndices.Count > 1)
            {
                StartWeaponChange(inventory.equippedWeaponIndices[1]);
            }
        }

        public void OnLook(InputValue value)
        {
            _lookDeltaInput = value.Get<Vector2>();
        }

        public void OnLean(InputValue value)
        {
            _userInput.SetValue(FPSANames.LeanInput, value.Get<float>() * settings.leanAngle);
            PlayTransitionMotion(settings.leanMotion);
        }

        public void OnCycleScope()
        {
            if (!IsAiming()) return;

            GetActiveItem().OnCycleScope();
            PlayTransitionMotion(settings.aimingMotion);
        }

        public void OnChangeFireMode()
        {
            GetActiveItem().OnChangeFireMode();
        }

        public void OnToggleAttachmentEditing()
        {
            if (HasActiveAction() && _actionState != FPSActionState.AttachmentEditing) return;

            _actionState = _actionState == FPSActionState.AttachmentEditing
                ? FPSActionState.None : FPSActionState.AttachmentEditing;

            if (_actionState == FPSActionState.AttachmentEditing)
            {
                _animator.CrossFade("InspectStart", 0.2f);
                return;
            }

            _animator.CrossFade("InspectEnd", 0.3f);
        }

        public void OnDigitAxis(InputValue value)
        {
            if (!value.isPressed || _actionState != FPSActionState.AttachmentEditing) return;
            GetActiveItem().OnAttachmentChanged((int)value.Get<float>());
        }
#endif
    }
}