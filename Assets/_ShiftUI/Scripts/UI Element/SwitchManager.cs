using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using STARTING;
using UnityEngine.Rendering.HighDefinition;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Michsky.UI.Shift
{
    public class SwitchManager : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Each switch must have a different tag")]
        public string switchTag = "Switch";
        public bool isOn = true;
        public bool saveValue = true;
        public bool invokeAtStart = true;

        [Header("Events")]
        public UnityEvent OnEvents;
        public UnityEvent OffEvents;

        [HideInInspector] public Animator switchAnimator;
        Button switchButton;

        void OnEnable()
        {
            Setup();
        }

        public void Setup()
        {
            if (switchAnimator == null) { switchAnimator = gameObject.GetComponent<Animator>(); }
            if (switchButton == null) { switchButton = gameObject.GetComponent<Button>(); switchButton.onClick.AddListener(AnimateSwitch); }

            if (saveValue == true)
            {
                if (PlayerPrefs.GetString(switchTag + "Switch") == "")
                {
                    if (isOn == true)
                    {
                        switchAnimator.Play("Switch On");
                        isOn = true;
                        PlayerPrefs.SetString(switchTag + "Switch", "true");
                    }

                    else
                    {
                        switchAnimator.Play("Switch Off");
                        isOn = false;
                        PlayerPrefs.SetString(switchTag + "Switch", "false");
                    }
                }

                else if (PlayerPrefs.GetString(switchTag + "Switch") == "true")
                {
                    switchAnimator.Play("Switch On");
                    isOn = true;
                }

                else if (PlayerPrefs.GetString(switchTag + "Switch") == "false")
                {
                    switchAnimator.Play("Switch Off");
                    isOn = false;
                }
            }

            else
            {
                if (isOn == true) { switchAnimator.Play("Switch On"); isOn = true; }
                else { switchAnimator.Play("Switch Off"); isOn = false; }
            }

            if (SceneManager.GetActiveScene().name != "MainScene")
            {
                if (STARTING.UIManager.Instance.playMode == STARTING.PlayMode.MULTI)
                {
                    StartCoroutine(MultiSetting());
                }
                else
                {
                    if (invokeAtStart == true && isOn == true) { OnEvents.Invoke(); }
                    if (invokeAtStart == true && isOn == false) { OffEvents.Invoke(); }
                }
            }
            else
            {
                if (invokeAtStart == true && isOn == true) { OnEvents.Invoke(); }
                if (invokeAtStart == true && isOn == false) { OffEvents.Invoke(); }
            }
        }

        private IEnumerator MultiSetting()
        {
            while (FindAnyObjectByType<QualityManager>().cameraData == null)
            {
                yield return null;
            }
            if (invokeAtStart == true && isOn == true) { OnEvents.Invoke(); }
            if (invokeAtStart == true && isOn == false) { OffEvents.Invoke(); }
        }

        public void AnimateSwitch()
        {
            if (isOn == true)
            {
                switchAnimator.Play("Switch Off");
                isOn = false;
                OffEvents.Invoke();
                if (saveValue == true) { PlayerPrefs.SetString(switchTag + "Switch", "false"); }
            }

            else
            {
                switchAnimator.Play("Switch On");
                isOn = true;
                OnEvents.Invoke();
                if (saveValue == true) { PlayerPrefs.SetString(switchTag + "Switch", "true"); }
            }
        }
    }
}