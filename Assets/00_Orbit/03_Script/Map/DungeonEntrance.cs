using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

/* 
 * 던전 입장 Trigger 스크립트
 * 최대한 던전 매니저에서 중앙 처리
 */

public class DungeonEntrance : MonoBehaviour
{
    public AudioClip entranceSound;

    private bool isLoading = false;
    private string currentZoneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentZoneName = GameManager.Instance.currentZoneName;
            UIManager.Instance.TipKey_Enable("던전 입장", "F");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F) && !isLoading)
            {
                HandleDungeonEntrance(other);
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

    private void HandleDungeonEntrance(Collider other)
    {
        bool isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);

        if (isLiberated)
        {
            UIManager.Instance.ScriptText_Enable("이미 해방된 구역이다. 굳이 들어갈 필요 없겠어.");
        }
        else
        {
            GameManager.Instance.PlaySound(entranceSound);
            isLoading = true;
            DungeonManager.Instance.UpdateDungeonLoading("주어진 시간 내에", "아레테를 파괴해야 합니다!", "아레테를 파괴하여 지역을 해방시키세요.");
            GameManager.Instance.SavePlayerPosition(other.transform.position);
            UIManager.Instance.TipKey_Disable();
            StartCoroutine(LoadDungeonSceneAfterDelay(3f));
        }
    }

    private IEnumerator LoadDungeonSceneAfterDelay(float delay)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("DungeonScene");
        asyncLoad.allowSceneActivation = false;

        yield return new WaitForSeconds(delay);

        asyncLoad.allowSceneActivation = true;
        asyncLoad.completed += OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        GameManager.Instance.SetPos(new Vector3(-31.353f, 7.588f, 3.23f));
        isLoading = false;
    }
}
