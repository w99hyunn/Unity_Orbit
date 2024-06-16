using UnityEngine;

namespace Orbit_Character
{
	public partial class PlayerController : MonoBehaviour
	{
		public GameObject spawnPos;
        private void Awake()
        {
			_controller = GetComponent<CharacterController>();
		}

        private void Start()
		{
            _controller = GetComponent<CharacterController>();
            InitializeNetworking();
			InitializeGun();
			InitialiseAnimations();
			InitializeInput();
		}
		
		private new void OnEnable()
		{			
			ResetAmmo();

			if (_isOwner && _possesed)
			{
				CinemachineVirtualCameraInstance.Instance.Follow(_cameraTarget);
				CinemachineVirtualCameraInstance.Instance.gameObject.SetActive(true);
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

        public void SetPos()
        {
            _controller.enabled = false;  // Move 대신 transform.position을 설정하려면 일시적으로 비활성화
            transform.position = spawnPos.transform.position;
            _controller.enabled = true;  // 다시 활성화
        }        
    }
}