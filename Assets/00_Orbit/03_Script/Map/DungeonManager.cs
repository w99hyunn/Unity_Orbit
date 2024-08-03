using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Michsky.UI.Shift;
using TMPro;

public class DungeonManager : MonoBehaviour
{
    private Animator dungeonTimeOutScreenAnimator;
    private TimedEvent dungeonTimeOutScreenTimedEvent;

    private TMP_Text text1;
    private TMP_Text text2;
    private TMP_Text text3;

    private Animator imgAni;

    public TMP_Text timerText;
    [Header("던전 시간")]
    public float timeRemaining = 300f;
    private bool timerRunning = false;
    public bool destroyArete = false;

    void Start()
    {
        GameObject dungeonTimeOutScreen = GameObject.Find("DungeonTimeOutScreen");
        dungeonTimeOutScreenAnimator = dungeonTimeOutScreen.GetComponent<Animator>();
        dungeonTimeOutScreenTimedEvent = dungeonTimeOutScreen.GetComponent<TimedEvent>();

        Transform loadingTransform = dungeonTimeOutScreen.transform.Find("Loading");
        Transform imageTransform = loadingTransform.transform.Find("Image");
        text1 = imageTransform.Find("Text1")?.GetComponent<TMP_Text>();
        imgAni = loadingTransform.Find("Image")?.GetComponent<Animator>();
        text2 = imageTransform.Find("Text2")?.GetComponent<TMP_Text>();
        text3 = imageTransform.Find("Text3")?.GetComponent<TMP_Text>();

        StartTimer();
    }

    void Update()
    {
        if (timerRunning && !(destroyArete))
        {
            if (timeRemaining > 1)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerText();
            }
            else
            {
                timerRunning = false;
                timeRemaining = 0;
                text1.text = "시간이 초과되었습니다.";
                text2.text = "아레테를 파괴하지 못했습니다!";
                text3.text = "잠시 후 원래 있던 곳으로 돌아갑니다.";
                StartCoroutine(HandleFailure());
            }
        }
    }

    void StartTimer()
    {
        timerRunning = true;
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    IEnumerator HandleFailure()
    {
        dungeonTimeOutScreenTimedEvent.StopIEnumerator();
        dungeonTimeOutScreenAnimator.Play("Loading");
        imgAni.Play("LFadeIn", 0, 0f);
        dungeonTimeOutScreenTimedEvent.StartIEnumerator();

        yield return new WaitForSeconds(0f);

        StartCoroutine(LoadOutdoorsSceneAfterDelay(3f));
    }

    private IEnumerator LoadOutdoorsSceneAfterDelay(float delay)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("OutdoorsScene");
        asyncLoad.allowSceneActivation = false;

        yield return new WaitForSeconds(delay);

        asyncLoad.allowSceneActivation = true;
        asyncLoad.completed += OnSceneLoaded;
    }

    void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        Vector3 lastPosition = GameManager.Instance.LoadPlayerPosition();
        //0803 PlayerController.Instance.SetPos(lastPosition);
    }
}
