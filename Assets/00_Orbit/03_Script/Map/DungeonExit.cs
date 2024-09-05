using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

/// <summary>
/// 던전 퇴장 Trigger
/// </summary>
public class DungeonExit : MonoBehaviour
{
    public AudioClip exitSound;

    private bool isLoading = false;
    private bool isPlayerInTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            UIManager.Instance.interactionKeyEnable("던전 퇴장", "F");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F) && !isLoading)
            {
                HandleDungeonExit();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            UIManager.Instance.interactionKeyDisable();
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.F) && !isLoading)
        {
            HandleDungeonExit();
        }
    }

    private void HandleDungeonExit()
    {
        if (GameObject.FindWithTag("Arete") != null)
        {
            UIManager.Instance.ScriptText_Enable("아레테를 파괴하지 않으면 돌아갈 수 없어.");
        }
        else if (GameObject.FindWithTag("Enemy") != null)
        {
            UIManager.Instance.ScriptText_Enable("들킬 위험이 있겠어... 드론을 모두 제거하자.");
        }
        else
        {
            PlayerStats.Instance.playerState = PlayerState.LOADING;
            GameManager.Instance.PlaySound(exitSound);
            isLoading = true;
            DungeonManager.Instance.UpdateDungeonLoading("아레테가 파괴되었습니다.", GameManager.Instance.currentZoneName + " 구역이 해방됩니다!", "원래 있던 곳으로 돌아갑니다.", 1);
            UIManager.Instance.interactionKeyDisable();
            StartCoroutine(LoadDungeonSceneAfterDelay(3f));
        }
    }

    private IEnumerator LoadDungeonSceneAfterDelay(float delay)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("OutdoorsScene");
        asyncLoad.allowSceneActivation = false;

        yield return new WaitForSeconds(delay);

        asyncLoad.allowSceneActivation = true;
        asyncLoad.completed += OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        PlayerStats.Instance.playerState = PlayerState.IDLE;
        GameManager.Instance.SetPos(GameManager.Instance.LoadPlayerPosition());
        isLoading = false;
    }
}
