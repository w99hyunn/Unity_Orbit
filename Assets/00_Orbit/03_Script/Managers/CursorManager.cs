using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public List<GameObject> objectsToDestroy = new List<GameObject>();
    public GameObject tooltip;
    public GameObject pauseMenuHotkey;


    private void Start()
    {
        CustomResume();
        objectsToDestroy.Add(GameManager.Instance.gameObject);
        objectsToDestroy.Add(PlayerStats.Instance.gameObject);
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

    private IEnumerator CheckPlayerState()
    {
        while (true)
        {
            if (PlayerStats.Instance.playerState == PlayerState.IDLE)
            {
                pauseMenuHotkey.SetActive(true);
                yield break; 
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void BackToMain()
    {
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
    public void DieGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenuHotkey.SetActive(false);
    }

    public void ContinueGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuHotkey.SetActive(true);
    }

    public void CustomResume()
    {
        PlayerStats.Instance.playerState = PlayerState.IDLE;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CustomPause()
    {
        PlayerStats.Instance.playerState = PlayerState.PAUSE;
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