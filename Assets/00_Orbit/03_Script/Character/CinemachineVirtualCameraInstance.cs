using System;
using Cinemachine;
using UnityEngine;

namespace Orbit_Character
{
	public class CinemachineVirtualCameraInstance : MonoBehaviour
	{
		public static CinemachineVirtualCameraInstance Instance;
		
		public CinemachineVirtualCamera FirstPersonCamera;
		private bool _firstPerson;
		
		public float defaultFovFps = 60;
		private float defaultFovTps = 60;
		
		private float lastFov = 0;
		private float oldFov = 60;
		private float fovSmoothing = 0;
		private float fovSmoothT = 0;

		
		public CinemachineVirtualCameraInstance()
		{
			Instance = this;
		}
		
		private void Awake()
		{
			defaultFovFps = FirstPersonCamera.m_Lens.FieldOfView;
			//SetFirstPerson(_firstPerson);
		}

		private void Update()
		{
			if (fovSmoothT > 0)
			{
				fovSmoothT -= fovSmoothing * Time.deltaTime;
				// smooth fov change
				if (FirstPerson)
				{
					FirstPersonCamera.m_Lens.FieldOfView = Mathf.Lerp(oldFov, lastFov, 1 - fovSmoothT);
					if (fovSmoothT < 0.001f)
					{
						FirstPersonCamera.m_Lens.FieldOfView = lastFov;
						lastFov = 0;
					}
				}			
			}
		}

		public bool FirstPerson
		{
			get => _firstPerson;
			set
			{
				if (_firstPerson == value) return;
				_firstPerson = value;

			}
		}


		public void Follow(Transform target)
		{
			FirstPersonCamera.Follow = target;
		}
		
		public void SetFov(float fov, float fovTime = 0)
		{
			if (fov == lastFov) return;
			lastFov = fov;
			
			if (fovTime > 0)
			{
				fovSmoothT = 1;
				fovSmoothing = 1f / fovTime;
			}

			if (FirstPerson)
			{
				oldFov = FirstPersonCamera.m_Lens.FieldOfView;
				if (fovTime <= 0)
					FirstPersonCamera.m_Lens.FieldOfView = fov;
			}
		}

		public void ResetFov(float fovTime = 0) => 
			SetFov(FirstPerson ? defaultFovFps : defaultFovTps, fovTime);
	}
}