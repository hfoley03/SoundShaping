using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class ChordTriplet
{
    public string Key;
    public string A;
    public string B;
}

[System.Serializable]
public class ChordTriplets
{
    public ChordTriplet[] chord_triplets;
}

public class JSONReader : MonoBehaviour
{
    public TextAsset textJSON;

    static string firstChordChoice = " ";
    static string secondChordChoice = " ";
    string filepath;
    static ChordTriplets chordsInJson;

    //static string jsonString = File.ReadAllText(path);
    //static ChordTriplets chordsInJson = JsonUtility.FromJson<ChordTriplets>(File.ReadAllText("Assets/StreamingAssets/no_gaps_n_grams_2_probabilities.json"));


    void Start()
    {
        /*        Debug.Log(jsonString);
                Debug.Log(SearchKey("ch4_ch5_ch4"));
                Debug.Log(jsonString.Length);*/
        filepath = Path.Combine(Application.persistentDataPath, "no_gaps_n_grams_2_probabilities.json");
        chordsInJson = JsonUtility.FromJson<ChordTriplets>(File.ReadAllText(filepath));
        //chordsInJson = JsonUtility.FromJson<ChordTriplets>(File.ReadAllText("Assets/StreamingAssets/no_gaps_n_grams_2_probabilities.json"));
    }

    public static Tuple<string, string> SearchKey(string query)
    {
        foreach (ChordTriplet chord_triplet in chordsInJson.chord_triplets)
        {
            if (query == chord_triplet.Key)
            {
                firstChordChoice = chord_triplet.A;
                secondChordChoice = chord_triplet.B;
                return new Tuple<string, string>(firstChordChoice, secondChordChoice);
            }
        }
        return null;
    }

}
