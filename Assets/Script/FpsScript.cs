using Michsky.UI.Shift;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FpsScript : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float deltaTime = 0f;


    [Header(" # Font Set")]
    [SerializeField, Range(10, 50)]
    private int size = 50;
    [SerializeField]
    private Color color = Color.white;

    [SerializeField]
    private SwitchManager SwitchManager;
    [SerializeField]
    private GameObject FpsSwitch;

    private string text;

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
                text = string.Format("FPS {0:0.} | vSync ", fps);
            }
            else
            {
                text = string.Format("FPS {0:0.} | Å¸°Ù {1:0} ", fps, Application.targetFrameRate);
            }
            fpsText.text = text;
            fpsText.fontSize = size;
            fpsText.color = color;
        }
        else
        {
            fpsText.text = "";
        }

    }
}