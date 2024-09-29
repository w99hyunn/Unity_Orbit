using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class MainUISupport : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject continueDescription;
    public GameObject newDescription;
    public TMP_Text gameTimeText;
    public TMP_Text levelText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public TMP_Text lastModifiedText;
    public TMP_Text loadingPercentText;
    public TMP_Text multiConnectInfo;

    [Header("멀티플레이")]
    [Header("멀티플레이 Lock/Unlock")]
    public GameObject multiPlayLock;
    public GameObject multiPlayUnlock;

    [Header("호스트 오픈")]
    public TMP_InputField hostIpInput;
    public TMP_InputField hostPortInput;

    [Header("클라이언트 접속")]
    public TMP_InputField clientIpInput;
    public TMP_InputField clientPortInput;

    [Header("로그인")]
    public TMP_InputField idInput;
    public TMP_InputField pwInput;

    private string playerPrefsIDKey = "LoginID";
    private string playerPrefsPWKey = "LoginPW";
    private bool isSavingEnabled = true;

    [Header("회원가입")]
    public TMP_InputField registerIdInput;
    public TMP_InputField registerPwInput;
    public Button signUpBtn;

    private void Start()
    {
        idInput.onValueChanged.AddListener(OnIDValueChanged);
        pwInput.onValueChanged.AddListener(OnPWValueChanged);
    }

    /// <summary>
    /// 공통 - 로딩 퍼센트
    /// </summary>
    /// <param name="progressText"></param>
    public void LoadingProgress(string progressText)
    {
        loadingPercentText.text = progressText;
    }

    /// <summary>
    /// 싱글플레이 이어하기 or 새로하기
    /// </summary>
    /// <param name="index"></param>
    public void ContinueGame(bool index)
    {
        continueButton.SetActive(index);
        continueDescription.SetActive(index);
        newDescription.SetActive(!index);
    }

    /// <summary>
    /// 싱글플레이 이어하기 - 정보 불러와서 보여주기
    /// </summary>
    /// <param name="time"></param>
    /// <param name="level"></param>
    /// <param name="hp"></param>
    /// <param name="mp"></param>
    public void SaveFileInfo(string time, string level, string hp, string mp)
    {
        gameTimeText.text = time;
        levelText.text = level;
        hpText.text = hp;
        mpText.text = mp;
    }

    public void LastSaveDate(string date)
    {
        lastModifiedText.text = date;
    }

    /// <summary>
    /// 멀티플레이 버튼을 보이게 할건지 조건에 따른 표시
    /// </summary>
    /// <param name="index"></param>
    public void ShowMultiplay(bool index)
    {
        multiPlayUnlock.SetActive(index);
        multiPlayLock.SetActive(!index);
    }

    public void MultiConnectInfo(string text)
    {
        multiConnectInfo.text = text;
    }

    private void OnIDValueChanged(string newValue)
    {
        if (isSavingEnabled)
        {
            PlayerPrefs.SetString(playerPrefsIDKey, newValue);
            PlayerPrefs.Save();
        }
    }

    private void OnPWValueChanged(string newValue)
    {
        if (isSavingEnabled)
        {
            PlayerPrefs.SetString(playerPrefsPWKey, newValue);
            PlayerPrefs.Save();
        }
    }

    public void EnableSaving()
    {
        isSavingEnabled = true;
        LoadLoginInfo();
    }

    public void DisableSaving()
    {
        isSavingEnabled = false;
    }

    public void LoadLoginInfo()
    {
        if (PlayerPrefs.HasKey(playerPrefsIDKey))
        {
            string savedID = PlayerPrefs.GetString(playerPrefsIDKey);
            SetLoginID(savedID);
        }

        if (PlayerPrefs.HasKey(playerPrefsPWKey))
        {
            string savedPW = PlayerPrefs.GetString(playerPrefsPWKey);
            SetLoginPW(savedPW);
        }
    }

    public void ClearLoginInfo()
    {
        if (PlayerPrefs.HasKey(playerPrefsIDKey))
        {
            PlayerPrefs.DeleteKey(playerPrefsIDKey);
        }

        if (PlayerPrefs.HasKey(playerPrefsPWKey))
        {
            PlayerPrefs.DeleteKey(playerPrefsPWKey);
        }
        PlayerPrefs.Save();
    }

    public void TermsAgree(bool index)
    {
        signUpBtn.interactable = index;
    }

    public string GetHostIP() { return hostIpInput.text; }
    public string GetHostPort() { return hostPortInput.text; }
    public string GetClientIP() { return clientIpInput.text; }
    public string GetClientPort() { return clientPortInput.text; }
    public void SetLoginID(string loginID) { idInput.text = loginID; }
    public void SetLoginPW(string loginPW) { pwInput.text = loginPW; }
    public string GetLoginID() { return idInput.text; }
    public string GetLoginPW() { return pwInput.text; }
    public string GetRegisterID() { return registerIdInput.text; }
    public string GetRegisterPW() { return registerPwInput.text; }
}
