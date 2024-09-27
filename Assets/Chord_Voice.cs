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

    void Start()
    {
        chordNumber = MusicManager.Instance.chordsSet;
        audioSources = new AudioSource[numberOfNotes];
        voices = new SingleVoice[numberOfNotes];

        for (int i = 0; i < numberOfNotes; i++) {
            aSingleVoice = Instantiate(singleVoice, this.transform);
            aSingleVoice.SetClip(i, chordNumber);
            voices[i] = aSingleVoice;
        }
        MusicManager.Instance.chordsSet++;
    }

    public void playNote(int noteNum)
    {
       // audioSources[noteNum].Play();
        voices[noteNum].PlayVoice();
    }
}
