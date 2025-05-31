using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager : MonoBehaviour
{
    public AudioClip homeBGM;
    public AudioClip gameBGM;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        audioSource.volume = savedVolume;
    }

    void Start()
    {
        PlaySceneBGM();
    }

    void PlaySceneBGM()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "HomeScreen")
        {
            PlayBGM(homeBGM);
        }
        else if (sceneName == "Game")
        {
            PlayBGM(gameBGM);
        }
    }

    public void UpdateVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }

    void PlayBGM(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        if (audioSource.clip == clip) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

}
