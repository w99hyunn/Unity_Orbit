using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class VideoOption : MonoBehaviour
{
    FullScreenMode ScreenMode;

    //해상도
    [SerializeField]
    private TMP_Dropdown ResolutionDropdown;
    [SerializeField]
    private Toggle FullscreenBtn;

    List<Resolution> Resolutions = new List<Resolution>();
    int ResolutionNum;
    int PipelineNum;
    int ResolutionOptionNum = 0;

    //성능
    [SerializeField]
    private List<RenderPipelineAsset> RenderPipelineAssets;
    [SerializeField]
    private TMP_Dropdown PerformanceDropdown;


    void OnEnable()
    {
        /* 해상도, 전체화면 설정 */
        for (int i = 0; i <Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRate == 60 || Screen.resolutions[i].refreshRate == 144) // 144는 디버그용
            {
                Resolutions.Add(Screen.resolutions[i]);
            }
        }
        ResolutionDropdown.options.Clear();

        foreach (Resolution item in Resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = item.width + "x" + item.height + " @" + item.refreshRate + "hz";
            ResolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                ResolutionDropdown.value = ResolutionOptionNum;
            ResolutionOptionNum++;
        }

        ResolutionDropdown.RefreshShownValue();

        FullscreenBtn.isOn=Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
        /* 해상도, 전체화면 설정 end */

        /* 성능 설정 값 불러오기 */
        PerformanceDropdown.value = QualitySettings.GetQualityLevel();
        PerformanceDropdown.RefreshShownValue();

    }

    public void DropboxOptionChange(int x)
    {
        ResolutionNum = x;
    }

    public void FullScreenBtn(bool isFull)
    {
        ScreenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void SetPipeline(int value)
    {
        if (value < 0 || value >= RenderPipelineAssets.Count)
        {
            Debug.LogError("Invalid pipeline index.");
            return;
        }

        PipelineNum = value;
    }

    public void OkBtnClick() //적용하기
    {
        /* 해상도, 전체화면 */
        Screen.SetResolution(Resolutions[ResolutionNum].width,
            Resolutions[ResolutionNum].height,
            ScreenMode);

        /* 성능 */
        QualitySettings.SetQualityLevel(PipelineNum);
        QualitySettings.renderPipeline = RenderPipelineAssets[PipelineNum];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 
}
