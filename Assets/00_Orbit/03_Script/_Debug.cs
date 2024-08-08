using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Debug : MonoBehaviour
{
    private CanvasGroup CanvasGroup;
    public PlayerStats playerStats;

    // Start is called before the first frame update
    void Start()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Tab))
        {
            CanvasGroup.alpha = 1;
        }
        else
        {
            CanvasGroup.alpha = 0;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            playerStats.TakeDamage(20);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            playerStats.UseMana(20);
        }
        if (Input.GetKeyDown(KeyCode.N)) // N키를 눌러 경험치 얻기 테스트
        {
            playerStats.GainExperience(50);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.SaveGame();
        }


    }

}
