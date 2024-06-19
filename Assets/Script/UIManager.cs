using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TMP_Text timeText;
    private float gameTime = 0f;
    private const float realSecondsPerGameDay = 3 * 60 * 60;
    private const float gameSecondsPerRealSecond = 24 * 60 * 60 / realSecondsPerGameDay;

    public float GameTime
    {
        get { return gameTime; }
        set { gameTime = value; }
    }

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

    private void Update()
    {
        UpdateGameTime();
    }

    private void UpdateGameTime()
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

        if (period == "오전" && hours == 0)
        {
            hours = 0;
        }
        else if (period == "오후" && hours == 0)
        {
            hours = 12;
        }
        else if (hours == 0)
        {
            hours = 12;
        }

        string timeFormatted = string.Format("{0} {1:D2}:{2:D2}", period, hours, minutes);

        timeText.text = timeFormatted;
    }
}
