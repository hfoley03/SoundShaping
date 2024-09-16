using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSphere : MonoBehaviour
{

    private Vector3 currentPoisition;
    private int segmentNumber;
    private int nodeNumber;
    private bool sculpting = false;

    public bool clicked = false;

    void Start()
    {
        currentPoisition = this.transform.position;
    }

    public void SetSegmentNumber(int number)
    {
        segmentNumber = number;
    }

    public int GetSegmentNumber()
    {
        return segmentNumber;
    }

    public void SetNodeNumber(int number)
    {
        nodeNumber = number;
    }

    public int GetNodeNumber()
    {
        return nodeNumber;
    }
}
