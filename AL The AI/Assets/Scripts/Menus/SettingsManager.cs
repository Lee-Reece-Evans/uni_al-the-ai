using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown graphicsDropDown;
    [SerializeField] private TMP_Dropdown resolutionsDropDown;
    [SerializeField] private Toggle fullscreen;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private AudioMixer audioMixer;

    private List<Resolution> resolutions;

    void Start()
    {
        // set graphics level
        graphicsDropDown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());

        Resolution[] allResolutions = Screen.resolutions; // get a full list of resolutions

        List<string> resOptions = new List<string>();
        resolutions = new List<Resolution>(); // initialise my resolution list
        int currentResIndex = 0;
        for (int i = 0; i < allResolutions.Length; i++)
        {
            if (allResolutions[i].refreshRate >= 60) // only include refresh rate of 60 +
            {
                resolutions.Add(allResolutions[i]);

                string option = allResolutions[i].width + " x " + allResolutions[i].height + " " + allResolutions[i].refreshRate;
                resOptions.Add(option);

                if (allResolutions[i].width == PlayerPrefs.GetInt("screenwidth", 1920) && allResolutions[i].height == PlayerPrefs.GetInt("screenheight", 1080))
                {
                    currentResIndex = resolutions.Count - 1; // set resolution 
                }
            }
        }
        resolutionsDropDown.AddOptions(resOptions);
        resolutionsDropDown.SetValueWithoutNotify(currentResIndex);
        resolutionsDropDown.RefreshShownValue();

        // set fullscreen toggle
        fullscreen.SetIsOnWithoutNotify(Screen.fullScreen);

        // set music / sound values
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value) * 20);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1);
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(soundSlider.value) * 20);
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(graphicsDropDown.value, true);
        SFXManager2D.instance.PlaySelectSFX();
    }

    public void SetFullScreen(bool isFullScreen)
    {
        SFXManager2D.instance.PlaySelectSFX();
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        SFXManager2D.instance.PlaySelectSFX();
        Resolution resolution = resolutions[resolutionsDropDown.value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("screenwidth", resolution.width);
        PlayerPrefs.SetInt("screenheight", resolution.height);
    }

    public void SetMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    public void SetSoundVolume()
    {
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(soundSlider.value) * 20);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
    }
}
