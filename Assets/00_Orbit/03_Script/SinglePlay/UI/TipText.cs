using UnityEngine;
using TMPro;

namespace NOLDA
{
    public class TipText : MonoBehaviour
    {
        public string[] tipTexts;

        private TextMeshProUGUI tmp;
        private int _randomNum;

        void OnEnable()
        {
            _randomNum = Random.Range(0, tipTexts.Length);
        }

        void Start()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            tmp.text = tipTexts[_randomNum];
        }
    }
}