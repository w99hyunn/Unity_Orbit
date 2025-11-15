using Michsky.UI.Shift;
using TMPro;
using UnityEngine;

namespace NOLDA
{
    public class FpsScript : MonoBehaviour
    {
        public TMP_Text fpsText;
        public GameObject FpsSwitch;

        private SwitchManager SwitchManager;

        private float _deltaTime = 0f;

        private void Start()
        {
            SwitchManager = FpsSwitch.GetComponent<SwitchManager>();
        }

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

            if (SwitchManager.isOn)
            {
                float fps = 1.0f / _deltaTime;
                if (Application.targetFrameRate == -1)
                {
                    fpsText.text = string.Format("FPS {0:0.} | vSync ", fps);
                }
                else
                {
                    fpsText.text = string.Format("FPS {0:0.} | ∞Ì¡§ {1:0} ", fps, Application.targetFrameRate);
                }
            }
            else
            {
                fpsText.text = "";
            }
        }
    }
}