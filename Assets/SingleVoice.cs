using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SingleVoice : MonoBehaviour
{
    int chordNumber = -1;
    int noteNumber = -1;
    int sourceIndex = 0;

    private AudioSource[] audioSources;
    int numberOfSources = 4;

    public AudioMixer theMixer;
    public AudioMixerGroup theChannel;

    private bool setupComplete;


    void Start()
    {
        //chordNumber = MusicManager.Instance.chordsSet;
        audioSources = new AudioSource[numberOfSources];
        for (int i = 0; i < numberOfSources; i++)
        {

            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.playOnAwake = false;
            newAudioSource.loop = false;
            newAudioSource.outputAudioMixerGroup = theChannel;
            audioSources[i] = newAudioSource;
        }
        setupComplete = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (setupComplete) 
        { 
            if(noteNumber >= 0  &&  chordNumber >= 0)
            {
                for (int i = 0; i < numberOfSources; i++)
                {
                    audioSources[i].clip = MusicManager.Instance.audioClips[noteNumber + chordNumber * 9];
                }
                setupComplete = false;
            }
        }

    }


    public void SetClip(int note, int _chordNumber)
    {
        noteNumber = note;
        chordNumber = _chordNumber;
    }

    public void PlayVoice() {
        audioSources[sourceIndex].Play();
        sourceIndex++;
        if (sourceIndex >= numberOfSources) {
            sourceIndex = 0;
        }
    }
}
