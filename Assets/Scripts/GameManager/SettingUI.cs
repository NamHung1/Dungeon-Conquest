using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingUI : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeText;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        UpdateVolumeText(savedVolume);

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();

        UpdateVolumeText(value);

        BgmManager bgmManager = FindObjectOfType<BgmManager>();
        if (bgmManager != null)
        {
            bgmManager.UpdateVolume(value);
        }
    }

    void UpdateVolumeText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100);
        volumeText.text = percent + "%";
    }
}
