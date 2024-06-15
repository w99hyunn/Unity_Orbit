using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("게임 내 시간")]
    public TMP_Text timeText; // UI Text 컴포넌트에 연결
    private float gameTime = 0f; // 게임 내 시간(초)
    private const float realSecondsPerGameDay = 3 * 60 * 60; // 3시간(현실 초)
    private const float gameSecondsPerRealSecond = 24 * 60 * 60 / realSecondsPerGameDay; // 현실의 1초에 해당하는 게임 시간
 

    private AudioSource audioSource;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        InGameTimeUpdate();
    }

    public void InGameTimeUpdate()
    {
        gameTime += Time.deltaTime * gameSecondsPerRealSecond;

        if (gameTime >= 24 * 60 * 60)
        {
            gameTime -= 24 * 60 * 60;
        }

        int hours = (int)(gameTime / 3600) % 24;
        int minutes = (int)(gameTime % 3600 / 60);

        string period = hours >= 12 ? "오후" : "오전";
        hours = hours % 12;

        // 오전 12시(자정)와 오후 12시(정오)의 예외 처리
        if (period == "오전" && hours == 0)
        {
            hours = 0; // 오전 12시는 0시로 표시
        }
        else if (period == "오후" && hours == 0)
        {
            hours = 12; // 오후 12시는 12시로 표시
        }
        else if (hours == 0)
        {
            hours = 12; // 0시를 12시로 변환
        }

        string timeFormatted = string.Format("{0} {1:D2}:{2:D2}", period, hours, minutes);

        timeText.text = timeFormatted;
    }

    public void GameOver()
    {

    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
