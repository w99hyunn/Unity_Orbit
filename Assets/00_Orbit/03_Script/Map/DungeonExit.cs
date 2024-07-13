using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using Orbit_Character;
using Michsky.UI.Shift;
using TMPro;

public class DungeonExit : MonoBehaviour
{
    public AudioClip exitSound;

    private Animator dungeonLoadingScreenAnimator;
    private TimedEvent dungeonLoadingScreenTimedEvent;

    private TMP_Text text1;
    private TMP_Text text2;
    private TMP_Text text3;

    private Animator imgAni;

    private bool isLoading = false;
    private bool playerInRange = false;

    private void Start()
    {
        GameObject dungeonLoadingScreen = GameObject.Find("DungeonLoadingScreen");
        dungeonLoadingScreenAnimator = dungeonLoadingScreen.GetComponent<Animator>();
        dungeonLoadingScreenTimedEvent = dungeonLoadingScreen.GetComponent<TimedEvent>();

        Transform loadingTransform = dungeonLoadingScreen.transform.Find("Loading");
        Transform imageTransform = loadingTransform.transform.Find("Image");
        text1 = imageTransform.Find("Text1")?.GetComponent<TMP_Text>();
        imgAni = loadingTransform.Find("Image")?.GetComponent<Animator>();
        text2 = imageTransform.Find("Text2")?.GetComponent<TMP_Text>();
        text3 = imageTransform.Find("Text3")?.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !isLoading)
        {
            HandleDungeonExit();
            GameManager.Instance.PlaySound(exitSound);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            UIManager.Instance.TipKey_Enable("던전 퇴장", "F");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            UIManager.Instance.TipKey_Disable();
        }
    }

    private void HandleDungeonExit()
    {
        if (GameObject.FindWithTag("Arete") == null)
        {
            isLoading = true;
            dungeonLoadingScreenTimedEvent.StopIEnumerator();
            text1.text = "아레테가 파괴되었습니다.";
            text2.text = GameManager.Instance.currentZoneName + " 구역이 해방됩니다!";
            text3.text = "원래 있던 곳으로 돌아갑니다.";
            dungeonLoadingScreenAnimator.Play("Loading");
            imgAni.Play("LFadeIn", 0, 0f);
            dungeonLoadingScreenTimedEvent.StartIEnumerator();
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
        Vector3 lastPosition = GameManager.Instance.LoadPlayerPosition();
        PlayerController.Instance.SetPos(lastPosition);
        isLoading = false;
    }
}
