using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using Orbit_Character;
using Michsky.UI.Shift;
using TMPro;

public class DungeonExit : MonoBehaviour
{
    private Animator DungeonLoadingScreenAnimator;
    private TimedEvent DungeonLoadingScreenTimedEvent;

    private TMP_Text Text1;
    private TMP_Text Text2;
    private TMP_Text Text3;

    private bool isLoading = false;

    private void Start()
    {
        GameObject dungeonLoadingScreen = GameObject.Find("DungeonLoadingScreen");
        DungeonLoadingScreenAnimator = dungeonLoadingScreen.GetComponent<Animator>();
        DungeonLoadingScreenTimedEvent = dungeonLoadingScreen.GetComponent<TimedEvent>();

        Transform loadingTransform = dungeonLoadingScreen.transform.Find("Loading");
        Text1 = loadingTransform.Find("Text1")?.GetComponent<TMP_Text>();
        Text2 = loadingTransform.Find("Text2")?.GetComponent<TMP_Text>();
        Text3 = loadingTransform.Find("Text3")?.GetComponent<TMP_Text>();
    }

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
                if (GameObject.FindWithTag("Arete") == null)
                {
                    isLoading = true;
                    DungeonLoadingScreenTimedEvent.StopIEnumerator();
                    Text1.text = "아레테가 파괴되었습니다.";
                    Text2.text = GameManager.Instance.currentZoneName + " 구역이 해방됩니다!";
                    Text3.text = "";
                    DungeonLoadingScreenAnimator.Play("Loading");
                    DungeonLoadingScreenTimedEvent.StartIEnumerator();
                    UIManager.Instance.TipKey_Disable();
                    StartCoroutine(LoadDungeonSceneAfterDelay(3f));
                }
                else
                {
                    UIManager.Instance.ScriptText_Enable("아레테를 파괴하지 않으면 돌아갈 수 없어.");
                }
            }
        }
    }
    private IEnumerator LoadDungeonSceneAfterDelay(float delay)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("OutdoorsScene");
        asyncLoad.allowSceneActivation = false; // 씬 로드를 완료하지 않도록 설정

        yield return new WaitForSeconds(delay);

        asyncLoad.allowSceneActivation = true; // 지연 후 씬 로드를 완료
        asyncLoad.completed += OnSceneLoaded;
    }


    void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        Vector3 lastPosition = GameManager.Instance.LoadPlayerPosition();
        PlayerController.Instance.SetPos(lastPosition);
        isLoading = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.TipKey_Disable();
        }
    }
}
