using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Text.RegularExpressions;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Music Manager is NUll!");

            return _instance;
        }
    }

    public int[] chordHistory = new int[4];
    int chordHistoryLength = 1;

    public AudioClip[] audioClips;

    public int chordsSet = 0;
    public int currentChordNumber = 0;

    public AudioMixer mixer;

    // controls how often current chord is updated 
    private float duration = 2.2857f * 4.0f;
    private float timer = 0f;

    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        setMasterVolume(1.0f);
        setDrawnNodesVolume(1.0f);
        setBackingTrackVolume(1.0f);
        setTMTVolume(1.0f);

        chordHistory[0] = -1;
        chordHistory[1] = -1;
        chordHistory[2] = -1;
        chordHistory[3] = currentChordNumber;

        BackgroundMusicPlayer.Instance.bgAudioSources[currentChordNumber].Play();
/*        if (BackgroundMusicPlayer.Instance.bgAudioSources[currentChordNumber].isPlaying)
        {
            LogManager.Instance.AppendText("chord_bg_should be playing?");
        }*/
       // Debug.Log(BackgroundMusicPlayer.Instance.bgAudioSources[currentChordNumber].clip.name);
        getChordHistory(chordHistoryLength);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            timer = 0f;
            currentChordNumber = GetNextChord();
            Sample_Player.Instance.setChordNumber(currentChordNumber);
            UpdateChordHistory();
            BackgroundMusicPlayer.Instance.bgAudioSources[currentChordNumber].Play();
        }
    }
    public float setMasterVolume(float vol)
    {
        float scaledvol = scale(0.0f, 1.0f, -80.0f, 0.0f, vol); 
        mixer.SetFloat("MasterVolume", scaledvol);
        float newVolValue;
        mixer.GetFloat("MasterVolume", out newVolValue);
        return newVolValue;

    }

    public void setDrawnNodesVolume(float vol)
    {
        float scaledvol = scale(0.0f, 1.0f, -80.0f, 0.0f, vol);
        mixer.SetFloat("DrawnNodesVolume", scaledvol);
    }

    public void setBackingTrackVolume(float vol)
    {
        float scaledvol = scale(0.0f, 1.0f, -80.0f, 0.0f, vol);
        mixer.SetFloat("BackingTrackVolume", scaledvol);
    }

    public void setTMTVolume(float vol)
    {
        float scaledvol = scale(0.0f, 1.0f, -80.0f, 0.0f, vol);
        mixer.SetFloat("TMTVolume", scaledvol);
    }

    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

    public void UpdateChordHistory()
    {
        chordHistory[0] = chordHistory[1];
        chordHistory[1] = chordHistory[2];
        chordHistory[2] = chordHistory[3];
        chordHistory[3] = currentChordNumber;

        if (chordHistoryLength < 4)
        {
            chordHistoryLength++;
        }
    }

    public string getChordHistory(int historyLength)
    {
        string chordHistoryString = "";
        if (chordHistoryLength == 1) { chordHistoryString = "ch" + (chordHistory[3] +1); }
        if (chordHistoryLength == 2) { chordHistoryString = "ch" + (chordHistory[3] +1) + "_ch" + (chordHistory[2] +1) ; }
        if (chordHistoryLength == 3) { chordHistoryString = "ch" + (chordHistory[3] +1) + "_ch" + (chordHistory[2] +1) + "_ch" + (chordHistory[1] +1); }
        if (chordHistoryLength == 4) { chordHistoryString = "ch" + (chordHistory[3] +1) + "_ch" + (chordHistory[2] +1) + "_ch" + (chordHistory[1] +1) + "_ch" + (chordHistory[0] +1); }
        //Debug.Log(chordHistoryString);
        return chordHistoryString;
    }


    public int GetNextChord()
    {
        System.Tuple<string, string> chordOptions;
        int chord_option = -1;
        string fullChordHist = getChordHistory(chordHistoryLength);

        while (true)
        {
            chordOptions = JSONReader.SearchKey(fullChordHist);
            if (chordOptions != null)
            {
               // Debug.Log("End query : " + fullChordHist);
                break;
            }
            else
            {
                fullChordHist = fullChordHist.Substring(4);
            }
        }

        Regex regex = new Regex(@"\d+");

        if (Random.Range(0.0f, 1.0f) > 0.5f)
        {
            Match match = regex.Match(chordOptions.Item1);
            return (int.Parse(match.Value) - 1);
 
        }
        else {
            Match match = regex.Match(chordOptions.Item2);
            return ( int.Parse(match.Value) -1 );
        }
    }
}
