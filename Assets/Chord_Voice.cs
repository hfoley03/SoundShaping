using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class Chord_Voice : MonoBehaviour
{

    int chordNumber = 0;
    private AudioSource[] audioSources;
    int numberOfNotes = 9;
    public AudioMixer theMixer;
    public AudioMixerGroup theChannel;


    public SingleVoice singleVoice;
    SingleVoice aSingleVoice;
    private SingleVoice[] voices;

    //Assets/Audio/basicSounds/cel_ch1_0_1_2023_12.wav
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(MusicManager.Instance.chordsSet);
/*
        AudioMixer myMixer = Resources.Load<AudioMixer>("MyMixer");
        if (myMixer == null)
        {
            Debug.LogError("no mixer");
        }
        AudioMixerGroup[] channels = myMixer.FindMatchingGroups("DrawnNodes");
        if (channels.Length == 0)
        {
            Debug.LogError("no channels");
        }*/
        chordNumber = MusicManager.Instance.chordsSet;
        audioSources = new AudioSource[numberOfNotes];

        voices = new SingleVoice[numberOfNotes];

        for (int i = 0; i < numberOfNotes; i++) {


            //SingleVoice newSingleVoice = gameObject.AddComponent<SingleVoice>();
            aSingleVoice = Instantiate(singleVoice, this.transform);
            aSingleVoice.SetClip(i, chordNumber);
            voices[i] = aSingleVoice;

/*            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.clip = MusicManager.Instance.audioClips[i +  chordNumber * 9]; 
            newAudioSource.playOnAwake = false;
            newAudioSource.loop = false;
            newAudioSource.outputAudioMixerGroup = theChannel;
            audioSources[i] = newAudioSource;*/
        }
       // Debug.Log("im the chord voice " + MusicManager.Instance.chordsSet);

        MusicManager.Instance.chordsSet++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playNote(int noteNum)
    {
       // audioSources[noteNum].Play();
        voices[noteNum].PlayVoice();
    }
}
