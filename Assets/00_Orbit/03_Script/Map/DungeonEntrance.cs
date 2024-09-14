using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

/// <summary>
/// 던전 입장 Trigger 스크립트
/// 최대한 던전 매니저에서 중앙 처리
/// </summary>
public class DungeonEntrance : MonoBehaviour
{
    public AudioClip entranceSound;
    public HintTrigger hintTrigger;

    private bool isLoading = false;
    private bool isPlayerInTrigger = false;

    private string currentZoneName;
    private Collider playerCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            playerCollider = other;
            currentZoneName = GameManager.Instance.currentZoneName;
            UIManager.Instance.interactionKeyEnable("던전 입장", "F");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            playerCollider = null;
            UIManager.Instance.interactionKeyDisable();
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.F) && !isLoading)
        {
            HandleDungeonEntrance(playerCollider);
        }
    }
    private void HandleDungeonEntrance(Collider other)
    {
        bool isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);

        if (isLiberated)
        {
            UIManager.Instance.ScriptText_Enable("이미 해방된 구역이다. 굳이 들어갈 필요 없겠어.");
        }
        else if (true == hintTrigger.IsEnemyInTrigger())
        {
            UIManager.Instance.ScriptText_Enable("근처에 감시 드론이 있어 들어가면 들킬 위험이 있겠어.");
        }
        else
        {
            PlayerStats.Instance.playerState = PlayerState.LOADING;
            GameManager.Instance.PlaySound(entranceSound);
            isLoading = true;
            UIManager.Instance.DungeonLoading("주어진 시간 내에", "아레테를 파괴해야 합니다!", "아레테를 파괴하여 지역을 해방시키세요.");
            GameManager.Instance.SavePlayerPosition(other.transform.position);
            UIManager.Instance.interactionKeyDisable();
            StartCoroutine(LoadDungeonSceneAfterDelay(3f));
        }
    }

    private IEnumerator LoadDungeonSceneAfterDelay(float delay)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("DungeonScene", LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        yield return new WaitUntil(() => asyncLoad.progress >= 0.9f);

        asyncLoad.allowSceneActivation = true;

        yield return new WaitUntil(() => asyncLoad.isDone);

        //yield return new WaitForSeconds(delay);

        asyncLoad.completed += OnSceneLoaded;
        SceneManager.UnloadSceneAsync("WorldScene");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("DungeonScene"));
    }

    private void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        PlayerStats.Instance.playerState = PlayerState.IDLE;
        GameManager.Instance.SetPos(new Vector3(-31.353f, 7.588f, 3.23f));
        isLoading = false;
    }
}
