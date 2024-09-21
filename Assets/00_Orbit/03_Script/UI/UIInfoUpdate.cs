using TMPro;
using UnityEngine;

public class UIInfoUpdate : MonoBehaviour
{
    public TMP_Text currentChipText;
    public TMP_Text xpDetailText;
    public TMP_Text healthDetailText;
    public TMP_Text manaDetailText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDetailText(string order, string text)
    {
        switch(order)
        {
            case "exp":
                xpDetailText.text = text;
                break;
            case "health":
                healthDetailText.text = text;
                break;
            case "mana":
                manaDetailText.text = text;
                break;
        }
        
    }
}
