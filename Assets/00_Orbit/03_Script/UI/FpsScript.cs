using Michsky.UI.Shift;
using TMPro;
using UnityEngine;

namespace STARTING
{
    public class FpsScript : MonoBehaviour
    {
        public TextMeshProUGUI fpsText;
        public GameObject FpsSwitch;
        [Header("Font Set")]
        public int size = 50;
        public Color color = Color.white;

        private float deltaTime = 0f;
        private SwitchManager SwitchManager;

        private void Start()
        {
            SwitchManager = FpsSwitch.GetComponent<SwitchManager>();
        }

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            if (SwitchManager.isOn)
            {
                float fps = 1.0f / deltaTime;
                if (Application.targetFrameRate == -1)
                {
                    fpsText.text = string.Format("FPS {0:0.} | vSync ", fps);
                }
                else
                {
                    fpsText.text = string.Format("FPS {0:0.} | ∞Ì¡§ {1:0} ", fps, Application.targetFrameRate);
                }
                fpsText.fontSize = size;
                fpsText.color = color;
            }
            else
            {
                fpsText.text = "";
            }

        }
    }
}