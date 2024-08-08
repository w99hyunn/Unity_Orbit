using Michsky.UI.Shift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public bool pauseMenu = false;
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

    void Update()
    {
        if (pauseMenu == false)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void BackToMain()
    {
        pauseMenu = true;
        SceneManager.LoadScene("MainScene");

        /* 메인화면 이동시 DontDestroyOnLoad 오브젝트들 삭제함
         * 계속 추가 되어야함 */
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("Player"));
        Destroy(GameObject.Find("UIManager"));
        Destroy(GameObject.Find("Pause Menu Manager"));
        Destroy(GameObject.Find("QualityManager"));
        Destroy(GameObject.Find("CursorManager"));
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
}