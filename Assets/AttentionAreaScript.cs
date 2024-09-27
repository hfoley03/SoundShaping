using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentionAreaScript : MonoBehaviour
{
    private static AttentionAreaScript _instance;
    public static AttentionAreaScript Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("AttentionAreaScript is NUll!");
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        Debug.Log("hi from the AttentionAreaScript");
    }

    public GameObject cubeOfAttention;
    public GameObject sphereOfAttention;

    GameObject cube;
    GameObject sphere;
    public GameObject head;
    float timer = 0.0f;
    float duration = 1.0f;

    public float totalAttentionTime = 0f;
    float lastUpdateTime = 0f;

    void Start()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.0f;
    }

    void Update()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.1f;
        transform.rotation = Camera.main.transform.rotation;

    }

    public int ObjectDetectionforAttentionOE() {
        Vector3 halfDim = new Vector3(0.43f, 0.29f, 2.0f);
        Collider[] hitColliders = Physics.OverlapBox(transform.position, halfDim, transform.rotation, 1 << 6);
        return hitColliders.Length;
    }

    public int ObjectDetectionforAttentionTMT()
    {
        Vector3 halfDim = new Vector3(0.43f, 0.29f, 2.0f);
        Collider[] hitColliders = Physics.OverlapBox(transform.position, halfDim, transform.rotation, 1 << 3);
        return hitColliders.Length;
    }


}
