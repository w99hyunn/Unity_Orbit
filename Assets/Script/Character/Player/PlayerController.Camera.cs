using UnityEngine;

namespace Orbit_Character
{
	public partial class PlayerController
	{
        /*Camera*/

        [Header("Cinemachine")]
        private bool _firstPerson = true;
        public Transform FirstPersonCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;
        [Range(0f, 0.4f)]
        public float FirstPersonCameraRotationSmoothing = 0.3f;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _bodyRotate = 0f;

        private Transform _cameraTarget;

        public bool FirstPerson
        {
            get => _firstPerson;
            set
            {
                if (_firstPerson == value) return;
                SetFirstPerson(value);
            }
        }

        private void InitializeCamera()
        {
            _cameraTarget = FirstPersonCameraTarget;

            if (_isOwner)
            {
                CinemachineVirtualCameraInstance.Instance.Follow(_cameraTarget);
                CinemachineVirtualCameraInstance.Instance.FirstPerson = _firstPerson;
            }
            _cinemachineTargetYaw = _cameraTarget.rotation.eulerAngles.y;
        }

        private void SetFirstPerson(bool value)
        {
            _firstPerson = value;
            if (_isOwner)
                _cameraTarget = FirstPersonCameraTarget;

            var rot = _cameraTarget.rotation.eulerAngles;
            _cameraTarget.rotation = Quaternion.Euler(rot.x, _cinemachineTargetYaw, rot.z);

            if (_isOwner)
            {
                CinemachineVirtualCameraInstance.Instance.FirstPerson = value;
                CinemachineVirtualCameraInstance.Instance.Follow(_cameraTarget);
            }
        }
        private void CameraRotation(bool lockInput = false)
        {
            if (_isOwner && _firstPerson)
                _animator.Update(0);

            bool AllowInput = _isOwner && !lockInput;

            var oldYaw = _cinemachineTargetYaw;
            var oldPitch = _cinemachineTargetPitch;


            if (AllowInput)
                _cinemachineTargetYaw += MouseX;

            if (_firstPerson)
            {
                var euler = NormalizeAnglePos(transform.localRotation.eulerAngles.y);


                const float maxDifference = 90f;
                float difference = GetAngleBetweenAngles(_cinemachineTargetYaw, euler);

                if (difference > maxDifference)
                {
                    _bodyRotate += difference - maxDifference;
                    _bodyRotate = NormalizeAngle(_bodyRotate);
                    transform.rotation = Quaternion.Euler(0, _bodyRotate, 0);
                }
                else if (difference < -maxDifference)
                {
                    _bodyRotate += difference + maxDifference;
                    _bodyRotate = NormalizeAngle(_bodyRotate);
                    transform.rotation = Quaternion.Euler(0, _bodyRotate, 0);
                }

                _cinemachineTargetYaw = NormalizeAnglePos(_cinemachineTargetYaw);
            }

            if (AllowInput)
            {
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch - MouseY, BottomClamp, TopClamp);
            }
            else
            {
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
            }
            if (!_firstPerson)
            {
                _cameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
            }
            else
            {
                _cameraTarget.rotation = Quaternion.Lerp(
                    _cameraTarget.rotation,
                    Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0),
                    Time.deltaTime * (FirstPersonCameraRotationSmoothing / Time.smoothDeltaTime));
            }

            if (_hasAnimator)
            {
                Vector3 eulerDifference = new Vector3(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f) - transform.localRotation.eulerAngles;
                eulerDifference = NormalizeEulerAnglesXY(eulerDifference);

                _animator.SetFloat(_animIDHeadLookY, Mathf.Clamp(eulerDifference.y, -90f, 90f));
                _animator.SetFloat(_animIDHeadLookX, Mathf.Clamp(eulerDifference.x, -90f, 90f));
            }
        }


        /*Angle*/
        private Vector3 NormalizeEulerAngles(Vector3 eulerAngles)
        {
            eulerAngles.x = NormalizeAngle(eulerAngles.x);
            eulerAngles.y = NormalizeAngle(eulerAngles.y);
            eulerAngles.z = NormalizeAngle(eulerAngles.z);
            return eulerAngles;
        }

        private Vector3 NormalizeEulerAnglesXY(Vector3 eulerAngles)
        {
            eulerAngles.x = NormalizeAngle(eulerAngles.x);
            eulerAngles.y = NormalizeAngle(eulerAngles.y);
            //eulerAngles.z = NormalizeAngle(eulerAngles.z);
            return eulerAngles;
        }

        private Vector2 NormalizeEulerAngles(Vector2 eulerAngles)
        {
            eulerAngles.x = NormalizeAngle(eulerAngles.x);
            eulerAngles.y = NormalizeAngle(eulerAngles.y);
            return eulerAngles;
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 180f)
                angle -= 360f;
            while (angle < -180f)
                angle += 360f;
            return angle;
        }

        private static float NormalizeAnglePos(float angle) => (angle + 360f) % 360f;

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        public static float GetAngleBetweenAngles(float angleA, float angleB)
        {
            float clockwiseAngle = NormalizeAngle(angleB - angleA);
            float counterClockwiseAngle = NormalizeAngle(angleA - angleB);

            if (Mathf.Abs(clockwiseAngle) < Mathf.Abs(counterClockwiseAngle))
            {
                return clockwiseAngle;
            }

            return counterClockwiseAngle;
        }
    }
}