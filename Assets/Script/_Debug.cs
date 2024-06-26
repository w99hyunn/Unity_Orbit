using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Debug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerStats.Instance.TakeDamage(20);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerStats.Instance.UseMana(20);
        }
        if (Input.GetKeyDown(KeyCode.N)) // N키를 눌러 경험치 얻기 테스트
        {
            PlayerStats.Instance.GainExperience(50);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.SaveGame();
        }
    }
}
