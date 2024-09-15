using UnityEngine;

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

    public void ChangePlayerStateIDLE()
    {
        PlayerStats.Instance.playerState = PlayerState.IDLE;
    }
}
