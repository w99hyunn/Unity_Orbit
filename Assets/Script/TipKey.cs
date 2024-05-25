using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipKey : MonoBehaviour
{
    [SerializeField]
    private GameObject KeyPreset;

    [SerializeField]
    private TMP_Text TipText;

    [SerializeField]
    private TMP_Text TipKeys;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Reload()
    {
        gameObject.SetActive(true);
        TipText.text = "¿Á¿Â¿¸";
        TipKeys.text = "R";
    }
}
