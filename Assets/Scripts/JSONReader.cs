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

    void Start()
    {
        filepath = Path.Combine(Application.persistentDataPath, "no_gaps_n_grams_2_probabilities.json");
        chordsInJson = JsonUtility.FromJson<ChordTriplets>(File.ReadAllText(filepath));
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
