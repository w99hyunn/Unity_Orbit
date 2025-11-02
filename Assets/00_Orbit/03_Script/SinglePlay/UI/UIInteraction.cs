using UnityEngine;

namespace STARTING
{
    public class UIInteraction : MonoBehaviour
    {
        public virtual void ResetPos()
        {
            GameManager.Instance.ResetPos();
        }

        public virtual void ContinueGame()
        {
            GameManager.Instance.ContinueGame();
        }

        public virtual void ChangePlayerStateDIE()
        {
            PlayerStats.Instance.playerState = PlayerState.DIE;
        }

        public virtual void ChangePlayerStateIDLE()
        {
            PlayerStats.Instance.playerState = PlayerState.IDLE;
        }
    }
}