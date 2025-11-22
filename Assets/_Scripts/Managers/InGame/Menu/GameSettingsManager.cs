using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

public class GameSettingsManager : MonoBehaviour 
{
    private GraphicsManager graphicsManager = new();

    [Header("Graphics")]
    [SerializeField] private SegmentedProgressBar resolutionsBar;
    [SerializeField] private SegmentedProgressBar qualitysBar;
    [SerializeField] private SegmentedProgressBar fullScreenBar;
    [SerializeField] private SegmentedProgressBar postProcessingbar;

    [Header("Audio")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;


    private void Awake()
    {
        GraphicsInit();
        AudioInit();
    }

    #region Graphics

    private void GraphicsInit()
    {
        graphicsManager.Init();
        resolutionsBar?.CreateSegments(graphicsManager.ResOptions.Count);
        qualitysBar?.CreateSegments(graphicsManager.Qualitys.Count);
        fullScreenBar?.CreateSegments(2);
        postProcessingbar?.CreateSegments(2);
    }

    private void LoadGraphicSettings()
    {
        //TODO : Load Graphic Settings
    }

    public void NextResolution()
    {
        graphicsManager.RezOptionNext();
        int index = graphicsManager.currentResolutionIndex;
        resolutionsBar?.HighlightSegment(index , () => graphicsManager.ResOptions[index]);
    }

    public void PrevResolution()
    {
        graphicsManager.RezOptionPrev();
        int index = graphicsManager.currentResolutionIndex;
        resolutionsBar?.HighlightSegment(index , () => graphicsManager.ResOptions[index]);
    }

    public void NextQuality()
    {
        graphicsManager.QualityOptionNext();
        int index = graphicsManager.currentQualityIndex;
        qualitysBar?.HighlightSegment(index , () => graphicsManager.ResOptions[index]);
    }

    public void PrevQuality()
    {
        graphicsManager.QualityOptionPrev();
        int index = graphicsManager.currentQualityIndex;
        qualitysBar?.HighlightSegment(index , () => graphicsManager.ResOptions[index]);
    }

    public void fullScreenOn()
    {
        graphicsManager.SetFullScreen(true);
        fullScreenBar?.HighlightSegment(0 , () => "ON");
    }

    public void fullScreenOff()
    {
        graphicsManager.SetFullScreen(false);
        fullScreenBar?.HighlightSegment(1 , () => "OFF");
    }

    public void PostProcessingOn()
    {
        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.renderPostProcessing = false;
        postProcessingbar?.HighlightSegment(0 , () => "ON");
        
    }

    public void PostProcessingOff()
    {
        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.renderPostProcessing = false;
        postProcessingbar?.HighlightSegment(1 , () => "OFF");
    }


    #endregion

    #region Audio
    
    private void AudioInit()
    {
        LoadAudioSaveSetting();

        masterSlider?.onValueChanged.AddListener(SetMaster);
        musicSlider?.onValueChanged.AddListener(SetMusic);
        sfxSlider?.onValueChanged.AddListener(SetSFX);
    }

    private void LoadAudioSaveSetting()
    {
        //TODo : Load AudioSettings
    }

    public void SetMaster(float volume)
    {
        mainMixer?.SetFloat("Master" , volume);
    }

    public void SetSFX(float volume)
    {
        mainMixer?.SetFloat("SFX" , volume);
    }

    public void SetMusic(float volume)
    {
        mainMixer?.SetFloat("Music" , volume);
    }

    #endregion
}