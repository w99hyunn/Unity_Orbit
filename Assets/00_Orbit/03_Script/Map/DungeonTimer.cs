using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Michsky.UI.Shift;
using TMPro;

public class DungeonTimer : MonoBehaviour
{
    public TMP_Text timerText;

    [Header("던전 시간")]
    public float timeRemaining = 300f;
    private bool timerRunning = false;
    public bool destroyArete = false;

    [Header("던전 BGM")]
    private AudioSource audioSource;
    public AudioClip startDungeon;
    public AudioClip clearDungeon;

    void Start()
    {
        StartTimer();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = startDungeon;
        audioSource.loop = true;
        audioSource.Play();
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
                StartCoroutine(HandleFailure());
            }
        }
        if (true == destroyArete && audioSource.clip != clearDungeon)
        {
            audioSource.clip = clearDungeon;
            audioSource.Play();
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
        DungeonManager.Instance.UpdateDungeonLoading("시간이 초과되었습니다.", "아레테를 파괴하지 못했습니다!", "잠시 후 원래 있던 곳으로 돌아갑니다.", false);
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
        GameManager.Instance.SetPos(lastPosition);
    }

}
