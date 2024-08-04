using Michsky.UI.Shift;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TMP_Text timeText;

    public GameObject ZoneName;
    public GameObject lockBack;
    public GameObject unlockBack;
    public TMP_Text zoneNameText; // 변수명 변경
    public TMP_Text minimapZoneNameText; // 변수명 변경
    public TMP_Text liberatedText; // 변수명 변경

    public TMP_Text currentBulletText;
    public TMP_Text maxBulletText;

    [Header("팁가이드")]
    public GameObject TipKey;
    private TMP_Text TipText;
    private TMP_Text TipKeys;

    [Header("스크립트 텍스트")]
    public GameObject scriptText;

    private float gameTime = 0f; // 21600
    private const float realSecondsPerGameDay = 3 * 60 * 60;
    private const float gameSecondsPerRealSecond = 24 * 60 * 60 / realSecondsPerGameDay;
    public Light sun; // SUN 오브젝트

    private void Start()
    {
        Transform TipTextTransform = TipKey.transform.Find("TipText");
        TipText = TipTextTransform.gameObject.GetComponent<TMP_Text>();

        Transform TipKeysTransform = TipKey.transform.Find("Tipkey");
        TipKeys = TipKeysTransform.gameObject.GetComponent<TMP_Text>();

        GameObject lightObject = GameObject.Find("Directional Light");
        //directionalLight = lightObject.GetComponent<Light>();
    }

    public void TipKey_Enable(string Title, string Key)
    {
        TipKey.SetActive(true);
        TipText.text = Title;
        TipKeys.text = Key;
    }

    public void TipKey_Disable()
    {
        TipKey.SetActive(false);
    }

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
        UpdateSunRotation();
    }

    public void UpdateGameTime()
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

    private void UpdateSunRotation()
    {
        float hours = gameTime / 3600f;

        //(05:00) = 0도, (12:00) = 90도, (18:00) = 180도
        //0도에서 180도로 선형 보간
        float rotationAngle = 0f;

        if (hours >= 5f && hours <= 18f)
        {
            rotationAngle = ((hours - 5f) / 13f) * 180f;
        }
        else if (hours < 5f)
        {
            rotationAngle = ((hours + 19f) / 13f) * 180f; // 오후 6시 이후 ~ 오전 5시 전
        }
        else if (hours > 18f)
        {
            rotationAngle = ((hours - 19f) / 13f) * 180f; // 오후 6시 이후 ~ 오전 5시 전
        }

        sun.transform.rotation = Quaternion.Euler(rotationAngle, 0, 0);
    }

    /* 존 이름 & 해방여부 업데이트 */

    private Coroutine deactivateCoroutine;

    public void UpdateZoneInfo(string zoneName, bool isLiberated)
    {
        ZoneName.SetActive(false);

        minimapZoneNameText.text = zoneName;
        zoneNameText.text = zoneName;

        if (isLiberated)
        { //해방됨
            unlockBack.SetActive(true);
            lockBack.SetActive(false);
        }
        else
        {
            unlockBack.SetActive(false);
            lockBack.SetActive(true);
        }

        liberatedText.text = isLiberated ? "해방됨" : "해방되지 않음";
        ZoneName.SetActive(true);

        // 기존 코루틴이 있으면 중지
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }

        // 새로운 코루틴 시작
        deactivateCoroutine = StartCoroutine(DeactivateZoneNameAfterDelay(6f));
    }

    private IEnumerator DeactivateZoneNameAfterDelay(float delay)
    {
        // delay 동안 대기
        yield return new WaitForSeconds(delay);
        // ZoneName 비활성화
        ZoneName.SetActive(false);
    }

    /* 가운데 하단 스크립트 텍스트 */

    private Coroutine deactivateScriptCoroutine;

    public void ScriptText_Enable(string text)
    {
        scriptText.SetActive(false);

        scriptText.GetComponent<TMP_Text>().text = text;
        scriptText.SetActive(true);

        // 기존 코루틴이 있으면 중지
        if (deactivateScriptCoroutine != null)
        {
            StopCoroutine(deactivateScriptCoroutine);
        }

        // 새로운 코루틴 시작
        deactivateScriptCoroutine = StartCoroutine(DeactivateScriptAfterDelay(6f));
    }

    private IEnumerator DeactivateScriptAfterDelay(float delay)
    {
        // delay 동안 대기
        yield return new WaitForSeconds(delay);
        // ZoneName 비활성화
        scriptText.SetActive(false);
    }

    public void ScriptText_Disable()
    {
        scriptText.SetActive(false);
    }

    public void GameOverUI()
    {
        Animator GameOverScreenAnimator;

        GameObject gameOverScreen = GameObject.Find("GameOverScreen");
        GameOverScreenAnimator = gameOverScreen.GetComponent<Animator>();

        GameOverScreenAnimator.Play("Loading");
    }

    public void CurrentBulletUpdate(int current)
    {
        currentBulletText.text = current.ToString();
    }

    public void MaxBulletUpdate(int max)
    {
        maxBulletText.text = max.ToString();
    }
}
