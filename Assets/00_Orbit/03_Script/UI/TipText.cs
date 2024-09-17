using UnityEngine;
using TMPro;

namespace STARTING
{
    public class TipText : MonoBehaviour
    {
        public string[] tipTexts;

        private TextMeshProUGUI tmp;
        private int randomNum;

        void OnEnable()
        {
            randomNum = Random.Range(0, tipTexts.Length);
        }

        void Start()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            tmp.text = tipTexts[randomNum];
        }
    }
}