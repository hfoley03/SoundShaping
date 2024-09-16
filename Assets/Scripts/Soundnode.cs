using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Soundnode
{
    public string name;
    public string pitch;
    public string rhythm;
    public string duration;
    public string tempo;
    public string instrument;
    public string musicStyle;
}

[System.Serializable]
public class SoundNodeList
{
    public Soundnode[] soundnodes;
}