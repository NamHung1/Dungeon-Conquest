using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    public Slider volumeSlider;

    private static SoundEffectManager instance;
    private static AudioSource audioSource;
    private static SoundEffectLibrary soundEffectLibrary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    public static void Play(string name)
    {
        AudioClip clip = soundEffectLibrary.GetRandomClip(name);
        if(clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(volumeSlider.value);
    }
}
