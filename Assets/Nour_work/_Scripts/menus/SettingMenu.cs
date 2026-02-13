using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer mainAudioMixer;
    public Slider volumeSlider;

    [Header("Graphics")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("Gameplay")]
    public Slider sensitivitySlider;

    Resolution[] resolutions;

    IEnumerator Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        yield return null; 
        
        float savedVol = PlayerPrefs.GetFloat("volume", 0.75f);
        volumeSlider.value = savedVol;
        SetVolume(savedVol);
        
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        fullscreenToggle.isOn = isFullscreen;

        float savedSens = PlayerPrefs.GetFloat("sensitivity", 0.5f);
        sensitivitySlider.value = savedSens;
        SetSensitivity(savedSens);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("volume", volume);

        float dbVolume = 0;

        if (volume <= 0.0001f)
        {
            dbVolume = -80f; 
        }
        else
        {
            dbVolume = Mathf.Log10(volume) * 20; 
        }

        mainAudioMixer.SetFloat("MasterVolume", dbVolume); 
    }
    

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
        
        var playerController = FindAnyObjectByType<StarterAssets.ThirdPersonController>();
        if (playerController != null)
        {
            playerController.UpdateSensitivity();
        }
    }
}