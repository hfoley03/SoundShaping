using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_Player : MonoBehaviour
{
    public GameObject chordVoice;
    public AudioSource single_voice;
    public GameObject[] chordVoices;
    private int chordNumber = 0;

    private static Sample_Player _instance;
    public static Sample_Player Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("dample player Null!!");
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }


    void Start()
    {
        setChordNumber(MusicManager.Instance.currentChordNumber);
        Debug.Log("sample player chord number = ch" + chordNumber);
        chordVoices = new GameObject[7];
        for (int i = 0; i < 7; i++)
        {
            GameObject a_chord_voice = Instantiate(chordVoice);
            a_chord_voice.transform.SetParent(gameObject.transform);
            chordVoices[i] = a_chord_voice;
            //Debug.Log("created voice");
        }
        //Debug.Log("sample_player filled");

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playNote(int noteNumber)
    {
        //Debug.Log("trying to play note  " +  noteNumber);
        chordVoices[chordNumber].GetComponent<Chord_Voice>().playNote(noteNumber);
        LogManager.Instance.oe_numTimesNodesPlayed++;
        //chordVoices[0].GetComponent<Chord_Voice>().playNote(noteNumber);

    }

    public void setChordNumber(int chNumber)
    {
        chordNumber = chNumber;

    }


}
