using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Slider HPBar;

    private float enemyMaxHP = 10;
    public float enemyCurrentHP = 0;

    // Start is called before the first frame update
    void Start()
    {
        InitEnemyHP();
    }

    // Update is called once per frame
    void Update()
    {
        HPBar.value = enemyCurrentHP / enemyMaxHP;
    }

    private void InitEnemyHP()
    {
        enemyCurrentHP = enemyMaxHP;
    }


}
