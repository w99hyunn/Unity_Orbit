using UnityEngine.SceneManagement;
using UnityEngine;
using Orbit_Character;
using System.Collections;
using Michsky.UI.Shift;
using TMPro;

public class DungeonEntrance : MonoBehaviour
{
    private string currentZoneName;
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
            currentZoneName = GameManager.Instance.currentZoneName;
            UIManager.Instance.TipKey_Enable("던전 입장", "F");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        bool isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);

        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F) && !isLoading)
            {
                if (isLiberated == true)
                {
                    UIManager.Instance.ScriptText_Enable("이미 해방된 구역이다. 굳이 들어갈 필요 없겠어.");
                }
                else
                {
                    isLoading = true;
                    DungeonLoadingScreenTimedEvent.StopIEnumerator();
                    Text1.text = "주어진 시간 내에";
                    Text2.text = "아레테를 파괴해야 합니다!";
                    Text3.text = "아레테를 파괴하여 지역을 해방시키세요.";
                    DungeonLoadingScreenAnimator.Play("Loading");
                    DungeonLoadingScreenTimedEvent.StartIEnumerator();
                    GameManager.Instance.SavePlayerPosition(other.transform.position);
                    UIManager.Instance.TipKey_Disable();
                    StartCoroutine(LoadDungeonSceneAfterDelay(3f));
                }
            }
        }
    }

    private IEnumerator LoadDungeonSceneAfterDelay(float delay)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("DungeonScene");
        asyncLoad.allowSceneActivation = false; // 씬 로드를 완료하지 않도록 설정

        yield return new WaitForSeconds(delay);

        asyncLoad.allowSceneActivation = true; // 지연 후 씬 로드를 완료
        asyncLoad.completed += OnSceneLoaded;
    }


    void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        PlayerController.Instance.SetPos(new Vector3(19.42f, 0, 9.09f));
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
