using UnityEngine.SceneManagement;
using UnityEngine;
using Orbit_Character;
using System.Collections;
using Michsky.UI.Shift;
using TMPro;

public class DungeonEntrance : MonoBehaviour
{
    public AudioClip entranceSound;

    private string currentZoneName;
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
            HandleDungeonEntrance();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentZoneName = GameManager.Instance.currentZoneName;
            UIManager.Instance.TipKey_Enable("던전 입장", "F");
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

    private void HandleDungeonEntrance()
    {
        bool isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);

        if (isLiberated)
        {
            UIManager.Instance.ScriptText_Enable("이미 해방된 구역이다. 굳이 들어갈 필요 없겠어.");
        }
        else
        {
            GameManager.Instance.PlaySound(entranceSound);
            isLoading = true;
            dungeonLoadingScreenTimedEvent.StopIEnumerator();
            text1.text = "주어진 시간 내에";
            text2.text = "아레테를 파괴해야 합니다!";
            text3.text = "아레테를 파괴하여 지역을 해방시키세요.";
            dungeonLoadingScreenAnimator.Play("Loading");
            imgAni.Play("LFadeIn", 0, 0f);
            dungeonLoadingScreenTimedEvent.StartIEnumerator();
            GameManager.Instance.SavePlayerPosition(GameObject.FindGameObjectWithTag("Player").transform.position);
            UIManager.Instance.TipKey_Disable();
            StartCoroutine(LoadDungeonSceneAfterDelay(3f));
        }
    }

    private IEnumerator LoadDungeonSceneAfterDelay(float delay)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("DungeonScene");
        asyncLoad.allowSceneActivation = false;

        yield return new WaitForSeconds(delay);

        asyncLoad.allowSceneActivation = true;
        asyncLoad.completed += OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        PlayerController.Instance.SetPos(new Vector3(19.42f, 0, 9.09f));
        isLoading = false;
    }
}
