using Michsky.UI.Shift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public bool PauseMenu = false;
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
        if (PauseMenu == false)
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
        PauseMenu = true;
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
        PauseMenu = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CustomPause()
    {
        PauseMenu = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}