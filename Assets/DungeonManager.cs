using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }

    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;
    public Image backgroundImageBase;
    public Sprite successBackground;
    public Sprite failureBackground;

    public UnityEvent onDungeonEnter;

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

    /* 
     * 던전 로딩 화면
     * UnityEvent로 애니메이션과 코루틴 처리
     */
    public void UpdateDungeonLoading(string t1, string t2, string t3, bool success = true)
    {
        onDungeonEnter.Invoke();

        if (true == success)
        {
            backgroundImageBase.sprite = successBackground;
        }
        else if (false == success)
        {
            backgroundImageBase.sprite = failureBackground;
        }

        text1.text = t1;
        text2.text = t2;
        text3.text = t3;
    }
}
