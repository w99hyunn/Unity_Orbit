using Mirror;
using System.Collections;

namespace STARTING
{
    public class UIInteraction_Multi : UIInteraction
    {
        private PlayerStats_Multi playerStats;

        private void Start()
        {
            StartCoroutine(FindLocalPlayer());
        }

        private IEnumerator FindLocalPlayer()
        {
            while (NetworkClient.localPlayer == null)
            {
                yield return null;
            }

            playerStats = NetworkClient.localPlayer.gameObject.GetComponent<PlayerStats_Multi>();
        }

        public override void ResetPos()
        {
            GameManager_Multi.Instance.ResetPos();
        }

        public override void ContinueGame()
        {
            GameManager_Multi.Instance.ContinueGame();
        }

        public override void ChangePlayerStateDIE()
        {
            playerStats.playerState = PlayerState_Multi.DIE;
        }

        public override void ChangePlayerStateIDLE()
        {
            playerStats.playerState = PlayerState_Multi.IDLE;
        }
    }
}