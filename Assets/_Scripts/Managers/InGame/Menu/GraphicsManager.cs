using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GraphicsManager
{
    [Header("GraphicsSetting")]
    [SerializeField] private Resolution[] Resolutions;
    [SerializeField] public List<string> Qualitys {get; private set;} = new List<string>();
    [SerializeField] public List<string> ResOptions {get; private set;} = new List<string>();
    private int minScreenWidth = 1080;

    [SerializeField] public int currentQualityIndex;
    [SerializeField] public int currentResolutionIndex = 0;
    private int prevQualityIndex;
    private int prevResIndex;

    public void Init()
    {
        GetResolutions();
    }

    void GetResolutions()
    {
        Resolutions = Screen.resolutions;

        for (int i = 0; i < Resolutions.Length; i++)
        {
            if(Resolutions[i].width >= 800)
            {
                string resolutionOption = Resolutions[i].width.ToString();

                ResOptions.Add(resolutionOption);

                if(Resolutions[i].width == Screen.width && Resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }
        }
        Qualitys = QualitySettings.names.ToList();
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height , true);
    }

    public void RezOptionNext()
    {
        SetResolution(currentResolutionIndex++);
    }

    public void RezOptionPrev()
    {
        SetResolution(currentResolutionIndex--);
    }

    private void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void QualityOptionNext()
    {
        SetQuality(currentQualityIndex++);
    }

    public void QualityOptionPrev()
    {
        SetQuality(currentQualityIndex--);
    }

    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
    }
}

[Serializable]
public class GraphicSettings
{
    public int resolutionIndex;
    public int qualityIndex;
    
    [Range(0f ,1f)] public float shadowStrength; 
}
