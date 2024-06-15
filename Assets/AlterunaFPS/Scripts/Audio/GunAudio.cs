using UnityEngine;

namespace AlterunaFPS
{
	public class GunAudio : MonoBehaviour
	{
		public RandomPitch ReloadSfx;
		public RandomPitch FireSfx;
		public PlayerController PlayerController;
		
		public void PlayReloadSfx() => ReloadSfx.Play();
		public void PlayFireSfx() => FireSfx.Play();

		public void UpdateReloadText() => PlayerController.Success_Reload();
	}
}