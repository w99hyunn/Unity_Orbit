using UnityEngine;

namespace Orbit_Character
{
	public partial class PlayerController : MonoBehaviour
	{
        private AudioSource audioSource;
        public static event System.Action OnPlayerControllerInitialized;
        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

		}

        private void Start()
		{
            audioSource = GetComponent<AudioSource>();
            _controller = GetComponent<CharacterController>();
            InitializeNetworking();
			InitializeGun();
			InitialiseAnimations();
			InitializeInput();

            OnPlayerControllerInitialized?.Invoke();
        }
		
		private void OnEnable()
		{			
			ResetAmmo();

			if (_isOwner && _possesed)
			{
				CinemachineVirtualCameraInstance.Instance.Follow(_cameraTarget);
				CinemachineVirtualCameraInstance.Instance.gameObject.SetActive(true);
			}
		}

        /*Audio*/
        public void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                audioSource.spatialBlend = 1f;
                audioSource.PlayOneShot(clip);
            }
        }

        /* network */
        private bool _isOwner = true;
        private bool _isHost = false;
        private bool _possesed;

        private void InitializeNetworking()
        {
            _isOwner = true;
            _isHost = true;
            OnPossession();
        }

        private void OnPossession()
		{
			InitializeCamera();
			InitializeHealth();
		}


		private void Update()
		{
            ProcessInput();
            JumpAndGravity();
			GroundedCheck();
			Move();
		}

		private void LateUpdate()
		{
			bool lockInput = LockCameraPosition || CursorManager.Instance.PauseMenu;
			CameraRotation(lockInput);
			GunAction(lockInput);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Grounded ? 
				new Color(0.0f, 1.0f, 0.0f, 0.35f) : 
				new Color(1.0f, 0.0f, 0.0f, 0.35f);

			Vector3 pos = transform.position;

			Gizmos.DrawSphere(
				new Vector3(pos.x, pos.y - GroundedOffset, pos.z),
				GroundedRadius);
		}

        public void SetPos(Vector3 pos)
        {
            _controller.enabled = false;  // Move 대신 transform.position을 설정하려면 일시적으로 비활성화
            transform.position = pos;
            _controller.enabled = true;  // 다시 활성화
        }

        public void ResetPos()
        {
            _controller.enabled = false;  // Move 대신 transform.position을 설정하려면 일시적으로 비활성화
            transform.position = new Vector3(0, 1, 10);
            _controller.enabled = true;  // 다시 활성화
        }
    }
}