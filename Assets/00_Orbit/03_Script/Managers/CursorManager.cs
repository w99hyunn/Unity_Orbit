using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public bool pauseMenu = false;
    public List<GameObject> objectsToDestroy = new List<GameObject>();
    public GameObject tooltip;
    public static CursorManager Instance { get; private set; }


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

    private void Start()
    {
        CustomResume();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            ShowTooltip();
        }
        else
        {
            HideTooltip();
        }
    }

    public void BackToMain()
    {
        pauseMenu = true;
        SceneManager.LoadScene("MainScene");

        DestroyObjectsInList();
    }

    public void DestroyObjectsInList()
    {
        foreach (GameObject obj in objectsToDestroy)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        objectsToDestroy.Clear();
    }

    public void CustomResume()
    {
        pauseMenu = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CustomPause()
    {
        pauseMenu = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ShowTooltip()
    {
        tooltip.SetActive(true);
    }
    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }
}