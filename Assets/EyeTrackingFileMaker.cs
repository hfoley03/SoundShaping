using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using MixedReality.Toolkit.Input;

/*string path = Path.Combine(Application.persistentDataPath, "MyFile.txt");
string myText = "this is some text";

using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write)) 
{
    using (var writer = new StreamWriter(file))
    {
        writer.Write("come on you");
    }
}
*/

public class EyeTrackingFileMaker : MonoBehaviour
{
    [SerializeField]
    private FuzzyGazeInteractor gazeInteractor;

    [SerializeField]
    private GameObject hitPointDisplayer;

    private float duration = 0.5f;
    private float timer = 0f;

    private float totalAttentionTime = 0.0f;

    private static EyeTrackingFileMaker _instance;
    public static EyeTrackingFileMaker Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("EyeTrackingFileMaker is NUll!");
            return _instance;
        }
    }

    string myText = "this is some text";

    string[] lines = { "line one is this line", "line two is this line", "line three obviously" };

    private void Awake()
    {
        _instance = this;
        Debug.Log("hi from the eye tracking logger");
    }

    void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, "MyFile.txt");

        Debug.Log("Log eye tracker start");


        hitPointDisplayer = Instantiate(hitPointDisplayer);
        hitPointDisplayer.GetComponent<Renderer>().enabled = true;

        using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            using (var writer = new StreamWriter(file))
            {
                writer.Write("OSC Tracking Data");

/*                foreach(string line in lines)
                {
                    writer.WriteLine(line);
                }*/

            }
        }
    }

    private void Update()
    {

        var ray = new Ray(gazeInteractor.rayOriginTransform.position, gazeInteractor.rayOriginTransform.forward * 3);
        if (Physics.Raycast(ray, out var hit))
        {
            hitPointDisplayer.transform.position = hit.point;
            string msg = "hit " + hit.collider.name;// Time.time + " " + transform.name + " Parent" + transform.parent.name;
           // Debug.Log(msg);
            appendText(msg);
        }

    }

    public void appendText(string newText)
    {
        string path = Path.Combine(Application.persistentDataPath, "MyFile.txt");
        using (StreamWriter outputFile = new StreamWriter(path, true))
        {
            outputFile.WriteLine(newText);
        }
    }
}

