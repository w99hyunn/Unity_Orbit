using UnityEngine;

namespace STARTING
{
    public class UIInteraction : MonoBehaviour
    {
        public void ResetPos()
        {
            GameManager.Instance.ResetPos();
        }

        public void ContinueGame()
        {
            GameManager.Instance.ContinueGame();
        }

        public void ChangePlayerStateDIE()
        {
            PlayerStats.Instance.playerState = PlayerState.DIE;
        }

        public void ChangePlayerStateIDLE()
        {
            PlayerStats.Instance.playerState = PlayerState.IDLE;
        }
    }
}