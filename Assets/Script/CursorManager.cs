using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public bool PauseMenu = false;


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
        Time.timeScale = 1f;
        PauseMenu = false;
        SceneManager.LoadScene("MainScene");
    }

    public void CustomResume()
    {
        PauseMenu = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CustomPause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PauseMenu = true;
    }
}