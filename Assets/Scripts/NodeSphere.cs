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

    // Start is called before the first frame update
    void Start()
    {
        currentPoisition = this.transform.position;
    }

    // Update is called once per frame
/*    void Update()
    {
        //Debug.Log(this.transform.parent.name);

        if (currentPoisition != this.transform.position)// (InteractionManager.Instance.getCurrentState().Equals(InteractionManager.InteractionState.Idle)))
        {

            if( InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Drawing)
            {
                //Debug.LogError("tRIED TO SCULPT BUT DRAWING");
                return;
            }

            if( InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Idle)
            {
                InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Sculpting);
            }

            if( InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Sculpting)
            {
                //Debug.Log("SCULPTING");
                currentPoisition = this.transform.position;
                this.transform.parent.GetComponent<LineGroupScript>().updateControlPoint(currentPoisition, segmentNumber, nodeNumber);
            }

            //InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Sculpting);
            //Debug.Log("Sphere Pos: " +  currentPoisition);
        }



    }*/

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
