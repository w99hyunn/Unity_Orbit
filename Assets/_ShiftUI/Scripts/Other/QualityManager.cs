using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using STARTING;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Michsky.UI.Shift
{
    public class QualityManager : MonoBehaviour
    {
        [Header("Audio")]
        public AudioMixer mixer;
        public SliderManager masterSlider;
        public SliderManager musicSlider;
        public SliderManager sfxSlider;

        [Header("Resolution")]
        public GameObject FrameObject;
        private bool preferSelector = false;
        public HorizontalSelector resolutionSelector;
        public TMP_Dropdown resolutionDropdown;
        [System.Serializable]
        public class DynamicRes : UnityEvent<int> { }
        public DynamicRes clickEvent;

        private float SaveFrameRate;

        List<Resolution> uniqueResolutions;
        List<string> options = new List<string>();

        public HDAdditionalCameraData cameraData;

        //FPSRate
        public GameObject FPSRate;

        public Volume globalVolume;

        [Header("감도 조절")]
        public Demo.Scripts.Runtime.Character.FPSControllerSettings settings;

        [Header("움직이는 UI")]
        public bool isMovementUI;

        private void Awake()
        {
            if (SceneManager.GetActiveScene().name != "WorldScene_Multi")
            {
                cameraData = FindAnyObjectByType<HDAdditionalCameraData>();
            }
        }

        void Start()
        {
            if (SceneManager.GetActiveScene().name == "WorldScene_Multi")
            {
                StartCoroutine(FindLocalPlayer());
            }
            if ("true" == PlayerPrefs.GetString("moveUISwitch"))
            {
                isMovementUI = true;
            }
            else if ("false" == PlayerPrefs.GetString("moveUISwitch"))
            {
                isMovementUI = false;
            }

            mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat(masterSlider.sliderTag + "SliderValue")) * 20);
            mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat(musicSlider.sliderTag + "SliderValue")) * 20);
            mixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat(sfxSlider.sliderTag + "SliderValue")) * 20);

            uniqueResolutions = GetUniqueResolutions();

            if (preferSelector == true)
            {
                if (resolutionDropdown != null) { resolutionDropdown.gameObject.SetActive(false); }
                if (resolutionSelector != null) { resolutionSelector.gameObject.SetActive(true); }
                else { return; }

                resolutionSelector.itemList.RemoveRange(0, resolutionSelector.itemList.Count);

                int currentResolutionIndex = -1;
                for (int i = 0; i < uniqueResolutions.Count; i++)
                {
                    string option = uniqueResolutions[i].width + "x" + uniqueResolutions[i].height;
                    options.Add(option);

                    resolutionSelector.CreateNewItem(options[i]);
                    if (uniqueResolutions[i].width == Screen.currentResolution.width
                        && uniqueResolutions[i].height == Screen.currentResolution.height)
                    {
                        currentResolutionIndex = i;
                        resolutionSelector.index = currentResolutionIndex;
                    }
                }

                if (currentResolutionIndex == 0) { resolutionSelector.index = resolutionSelector.itemList.Count - 1; }
                resolutionSelector.UpdateUI();
            }
            else
            {
                if (resolutionSelector != null) { resolutionSelector.gameObject.SetActive(false); }
                if (resolutionDropdown != null) { resolutionDropdown.gameObject.SetActive(true); }
                else { return; }

                resolutionDropdown.ClearOptions();

                List<string> options = new List<string>();

                int currentResolutionIndex = 0;
                for (int i = 0; i < uniqueResolutions.Count; i++)
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                    option.text = uniqueResolutions[i].width + "x" + uniqueResolutions[i].height;
                    resolutionDropdown.options.Add(option);

                    if (uniqueResolutions[i].width == Screen.width
                        && uniqueResolutions[i].height == Screen.height)
                    {
                        currentResolutionIndex = i;
                    }
                }

                resolutionDropdown.value = currentResolutionIndex;
                resolutionDropdown.RefreshShownValue();
                resolutionDropdown.onValueChanged.RemoveAllListeners();
                resolutionDropdown.onValueChanged.AddListener(SetResolution);
            }
        }

        private IEnumerator FindLocalPlayer()
        {
            while (GameManager_Multi.Instance.player == null)
            {
                yield return null;
            }
            cameraData = GameManager_Multi.Instance.player.GetComponentInChildren<HDAdditionalCameraData>();
        }

        List<Resolution> GetUniqueResolutions()
        {
            Resolution[] allResolutions = Screen.resolutions;
            HashSet<(int, int)> uniqueResolutionSet = new HashSet<(int, int)>();
            List<Resolution> uniqueResolutionList = new List<Resolution>();

            foreach (Resolution res in allResolutions)
            {
                var resolutionTuple = (res.width, res.height);
                float aspectRatio = (float)res.width / res.height;

                // Check if the aspect ratio is 16:9 or 16:10
                if ((Mathf.Approximately(aspectRatio, 16f / 9f)) || (Mathf.Approximately(aspectRatio, 16f / 10f)))
                {
                    if (uniqueResolutionSet.Add(resolutionTuple))
                    {
                        uniqueResolutionList.Add(res);
                    }
                }
            }

            return uniqueResolutionList;
        }


        public void UpdateResolution()
        {
            clickEvent.Invoke(resolutionSelector.index);
        }

        public void SetResolution(int resolutionIndex)
        {
            Screen.SetResolution(uniqueResolutions[resolutionIndex].width,
                uniqueResolutions[resolutionIndex].height,
                Screen.fullScreen);
        }


        public void AntiAliasingSet(int index) //안티앨리어싱 <테스트 후 확인 완료>
        {
            switch (index)
            {
                case 0:
                    cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
                    break;
                case 1:
                    cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;
                    break;
                case 2:
                    cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
                    break;
                case 3:
                    cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.Low;
                    break;
                case 4:
                    cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.Medium;
                    break;
                case 5:
                    cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.High;
                    break;

                default:
                    Debug.LogWarning("Invalid anti-aliasing option. Using default: None.");
                    cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
                    break;
            }
        }

        // DLSS ON/OFF
        public void DLSS(bool index)
        {
            cameraData.allowDynamicResolution = index;
        }

        public void VsyncSet(int index) // 수직동기화 <테스트 후 확인 완료>
        {
            QualitySettings.vSyncCount = index;
            if (index == 1) //수직동기화 ON시 프레임고정 초기화
            {
                Application.targetFrameRate = -1;
                FrameObject.SetActive(false);
            }
            else
            {
                FrameObject.SetActive(true);
                Application.targetFrameRate = (int)SaveFrameRate;
            }

        }

        public void ShowFPSEnable() //FPS 레이트 켜기
        {
            FPSRate.SetActive(true);
        }

        public void ShowFPSDisable() //FPS 레이트 끄기
        {
            FPSRate.SetActive(false);
        }

        public void FixFrameRate(float framenum) //프레임 고정 <테스트 후 확인 완료>
        {
            SaveFrameRate = framenum;
            if (QualitySettings.vSyncCount == 0)
            {
                Application.targetFrameRate = (int)SaveFrameRate;
            }
            if (QualitySettings.vSyncCount == 1)
            {
                Application.targetFrameRate = -1;
            }
        }

        public void TextureSet(int index) //텍스처 품질 <테스트 후 확인 완료>
        {
            QualitySettings.globalTextureMipmapLimit = index;
        }
        /*
        public void FieldOfViewSet(float index) //FOV 값 조절 <테스트 후 확인 완료>
        {
            mainCamera.fieldOfView = index;
        }*/

        public void AnisotropicFilteringEnable() //이방성 필터링 <테스트 후 확인 완료>
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        }

        public void AnisotropicFilteringDisable() //이방성 필터링 비활성화 <테스트 후 확인 완료>
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        }

        public void MotionBlurEnable(bool index) // 모션블러 <테스트 후 확인 완료>
        {
            VolumeProfile profile = globalVolume.sharedProfile;

            if (profile.TryGet<MotionBlur>(out MotionBlur motionBlur))
            {
                motionBlur.active = index;
            }
        }

        /* 레이트레이싱 관련 */
        public void EnableRayTracing()
        {
            QualitySettings.SetQualityLevel(1);
        }

        public void DisableRayTracing()
        {
            QualitySettings.SetQualityLevel(0);
        }

        /* 그림자 On/Off 동작 O */
        public void ShadowsSet(bool index)
        {
            VolumeProfile profile = globalVolume.sharedProfile;

            if (profile.TryGet<HDShadowSettings>(out HDShadowSettings hdShadowSettings))
            {
                hdShadowSettings.active = index;
            }
        }

        /* 비네트 On/Off 동작 O */
        public void VignetteSet(bool index)
        {
            VolumeProfile profile = globalVolume.sharedProfile;

            if (profile.TryGet<Vignette>(out Vignette vignette))
            {
                vignette.active = index; // isActive는 Vignette를 활성화(true) 또는 비활성화(false)하는 변수입니다.
            }
        }

        //마우스 감도
        public void SensitivitySpeedSet(float index)
        {
            settings.sensitivity = index;
        }

        // 움직이는 UI On/Off
        public void MoveUISet(bool index)
        {
            isMovementUI = index;
        }

        /* ------------------------------------------------------------------------------ */

        public void ShadowResolutionSet(int index) //그림자 품질<<<<적용X>>>>
        {
            if (index == 3)
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            else if (index == 2)
                QualitySettings.shadowResolution = ShadowResolution.High;
            else if (index == 1)
                QualitySettings.shadowResolution = ShadowResolution.Medium;
            else if (index == 0)
                QualitySettings.shadowResolution = ShadowResolution.Low;
        }



        public void ShadowsCascasedSet(int index) //그림자 카스케이드<<<<적용X>>>>
        {
            QualitySettings.shadowCascades = index;
        }


        public void SoftParticleSet(int index) //부드러운 파티클 <<<<적용X>>>>
        {
            QualitySettings.softParticles = index == 1;
        }

        public void ReflectionSet(int index) //실시간 반사 <<<<적용X>>>>
        {
            QualitySettings.realtimeReflectionProbes = index == 1;
        }


        /* ------------------------------------------------------------------------------ */

        public void VolumeSetMaster(float volume)
        {
            mixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }

        public void VolumeSetMusic(float volume)
        {
            mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        }

        public void VolumeSetSFX(float volume)
        {
            mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }

        public void SetOverallQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void WindowFullscreen()
        {
            Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }

        public void WindowBorderless()
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }

        public void WindowWindowed()
        {
            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

    }
}
