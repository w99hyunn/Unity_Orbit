using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;

public class TipText : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private int RandomNum;
    [SerializeField]
    private string[] TipTexts = new string[] { "현 지구는 AI(인공지능)의 실효지배를 받고 있으며, 인간은 허용된 행위(놀기, 먹기 등)를 제외한 지식을 쌓는 일체의 행위를 할 수 없습니다.",
        "지식을 갖고 깨어있는 인간은 AI의 적으로 간주됩니다.",
        "각 구역별로 AI를 움직일 수 있게 하는 원동력인 아레테가 존재합니다. 아레테를 파괴하여 AI를 멈춰야합니다.",
        "깨어있는 지식인을 표방하는 자들이 모인 이리스 그룹의 조력자들은 주인공에게 쿨타임마다 도움을 줄 수 있는 스킬을 제공합니다."};


    void OnEnable()
    {
        RandomNum = Random.Range(0, TipTexts.Length);
    }

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = TipTexts[RandomNum];
    }

}
