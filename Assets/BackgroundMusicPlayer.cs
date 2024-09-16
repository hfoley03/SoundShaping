using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMusicPlayer : MonoBehaviour
{
    public AudioClip[] bgAudioClips;
    public AudioClip bgAudioClip;
    public AudioSource[] bgAudioSources;
    public AudioMixerGroup theChannel;
    private static BackgroundMusicPlayer _instance;
    public static BackgroundMusicPlayer Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("BackgroundMusicPlayer is NUll!");
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
       Debug.Log("hi fromBackgroundMusicPlayer");
    }
    void Start()
    {
        bgAudioSources = new AudioSource[bgAudioClips.Length];
        for (int i = 0; i < bgAudioClips.Length; i++)
        {
            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.clip = bgAudioClips[i];
            newAudioSource.playOnAwake = false;
            newAudioSource.loop = false;
            newAudioSource.outputAudioMixerGroup = theChannel;
            bgAudioSources[i] = newAudioSource;
        }
    }
}
