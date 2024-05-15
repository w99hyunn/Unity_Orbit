using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // 다른 스크립트에서 쉽게 접근이 가능하도록 static
    public static bool GameIsPaused = false;

    [SerializeField]
    private GameObject pauseMenuCanvas;

    [SerializeField]
    private GameObject GameHUDCanvas;


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();

            }
            else
            {
                Pause();
            }
        }

        if (GameIsPaused == false)
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

    public void Resume()
    {
        pauseMenuCanvas.SetActive(false);
        GameHUDCanvas.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameIsPaused = true;
        pauseMenuCanvas.SetActive(true);
        GameHUDCanvas.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ToSettingMenu()
    {
        Debug.Log("아직 미구현입니다...");
    }

    public void ToMain()
    {
        Debug.Log("아직 미구현입니다...");
        //Time.timeScale = 1f;
        //SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("아직 미구현입니다...");
        Application.Quit();
    }
}