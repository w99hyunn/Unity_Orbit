using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

/* 
 * 던전 퇴장 Trigger 스크립트
 * 최대한 던전 매니저에서 중앙 처리
 */

public class DungeonExit : MonoBehaviour
{
    public AudioClip exitSound;

    private bool isLoading = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.TipKey_Enable("던전 퇴장", "F");
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
            UIManager.Instance.TipKey_Disable();
        }
    }

    private void HandleDungeonExit()
    {
        if (GameObject.FindWithTag("Arete") == null)
        {
            GameManager.Instance.PlaySound(exitSound);
            isLoading = true;
            DungeonManager.Instance.UpdateDungeonLoading("아레테가 파괴되었습니다.", GameManager.Instance.currentZoneName + " 구역이 해방됩니다!", "원래 있던 곳으로 돌아갑니다.");
            UIManager.Instance.TipKey_Disable();
            StartCoroutine(LoadDungeonSceneAfterDelay(3f));
        }
        else
        {
            UIManager.Instance.ScriptText_Enable("아레테를 파괴하지 않으면 돌아갈 수 없어.");
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
        GameManager.Instance.SetPos(GameManager.Instance.LoadPlayerPosition());
        isLoading = false;
    }
}
