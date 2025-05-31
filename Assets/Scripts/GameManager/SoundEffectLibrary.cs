using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundEffectLibrary : MonoBehaviour
{
    public SoundEffectGroup[] soundEffectList;

    private Dictionary<string, List<AudioClip>> soundDictionary;

    private void Awake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (SoundEffectGroup group in soundEffectList)
        {
            soundDictionary[group.mame] = group.soundEffects;
        }
    }

    public AudioClip GetRandomClip(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            List<AudioClip> clips = soundDictionary[name];
            if (clips.Count > 0)
            {
                return clips[Random.Range(0, clips.Count)];
            }
        }
        return null;
    }
}

[System.Serializable]
public class SoundEffectGroup
{
    public string mame;
    public List<AudioClip> soundEffects;
}
