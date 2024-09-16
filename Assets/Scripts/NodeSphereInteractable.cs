using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class NodeSphereInteractable : MixedReality.Toolkit.SpatialManipulation.ObjectManipulator
{
    private Vector3 currentPoisition;
    private int segmentNumber;
    private int nodeNumber;
    private int lineNumber;

    private GameObject line1 = null;
    private GameObject line2 = null;
    private LineRenderer lineRenderer1 = null;
    private LineRenderer lineRenderer2 = null;

    private LineGroupScript middleNodeNextLineParentComp = null;

    private bool meshColliderEnabled = true;
    public enum NodeType
    {
        StartNoteNode,
        MiddleNoteNode,
        EndNoteNode,
        SmallNode
    }

    private NodeType nodeType;

    private string handed;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.drawnVisible)
        {

            if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Drawing || InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Idle)
            {
               // Debug.Log("i am drawing piss off");
                AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;

                //InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Idle);

                return;
            }

            if (IsGrabSelected)
            {


                if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Sculpting)
                {
                    InteractionManager.Instance.finishedSculpting = false;

                    if (line1 == null)
                    {
                        // linesSetUp();
                        Debug.Log("line1 is null not allowed to move");
                        AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;
                        return;
                    }


                    switch (nodeType)
                    {

                        case NodeType.StartNoteNode:
                            // attached to one linenumber seg number 0
                            //this.transform.parent.GetComponent<LineGroupScript>().updateControlPoint(currentPoisition, segmentNumber, nodeNumber, false);

                            AllowedManipulations = MixedReality.Toolkit.TransformFlags.Move;
                            // currentPoisition = this.transform.position;
                            this.transform.parent.GetComponent<LineGroupScript>().updateControlPointsStartNode(lineRenderer1, this.transform.position, segmentNumber, nodeNumber, false);

                            break;

                        case NodeType.MiddleNoteNode:
                            // attached to this line number + seg number and the next line number seg 0
                            // this.transform.parent.GetComponent<LineGroupScript>().updateControlPoint(currentPoisition, segmentNumber, nodeNumber, false);
                            AllowedManipulations = MixedReality.Toolkit.TransformFlags.Move;
                            // currentPoisition = this.transform.position;
                            this.transform.parent.GetComponent<LineGroupScript>().updateControlPointsEndNode(lineRenderer1, this.transform.position, segmentNumber, nodeNumber, false);
                            middleNodeNextLineParentComp.updateControlPointsStartNode(lineRenderer2, this.transform.position, 0, 0, false);

                            break;

                        case NodeType.EndNoteNode:
                            // attached to the this line number segment number
                            //this.transform.parent.GetComponent<LineGroupScript>().updateControlPoint(currentPoisition, segmentNumber, nodeNumber, false);
                            AllowedManipulations = MixedReality.Toolkit.TransformFlags.Move;
                            //currentPoisition = this.transform.position;
                            this.transform.parent.GetComponent<LineGroupScript>().updateControlPointsEndNode(lineRenderer1, this.transform.position, segmentNumber, nodeNumber, false);

                            break;

                        case NodeType.SmallNode:
                            // attached to this line number at seg number this plus seg number + 1
                            //this.transform.parent.GetComponent<LineGroupScript>().updateControlPoint(currentPoisition, segmentNumber, nodeNumber, false);
                            AllowedManipulations = MixedReality.Toolkit.TransformFlags.Move;
                            //currentPoisition = this.transform.position;
                            this.transform.parent.GetComponent<LineGroupScript>().updateControlPointsSmallNode(lineRenderer2, lineRenderer1, this.transform.position, segmentNumber, nodeNumber, false);

                            break;

                    }
                }


            }
            else
            {
                AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;

                if (transform.TryGetComponent<Rigidbody>(out Rigidbody rigi))
                {
                    rigi.velocity = Vector3.zero;
                };
            }
        }
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Sculpting)
        {
            base.ProcessInteractable(updatePhase);
        }

    }


    public void linesSetUp()
    {
        string bezLineRendererName;
        switch (nodeType)
        {

            case NodeType.StartNoteNode:
                // attached to one linenumber seg number 0
                bezLineRendererName = "Line_" + lineNumber + "_Segment_" + segmentNumber;
                line1 = GameObject.Find(bezLineRendererName);
                lineRenderer1 = line1.GetComponent<LineRenderer>();
               // Debug.Log(line1.name);
               // Debug.Log(lineRenderer1.name);
                break;

            case NodeType.MiddleNoteNode:
                // attached to this line number + seg number and the next line number seg 0
                bezLineRendererName = "Line_" + lineNumber + "_Segment_" + segmentNumber;
                line1 = GameObject.Find(bezLineRendererName);
                lineRenderer1 = line1.GetComponent<LineRenderer>();
               // Debug.Log(line1.name);

                bezLineRendererName = "Line_" + (lineNumber + 1) + "_Segment_" + 0;
                line2 = GameObject.Find(bezLineRendererName);
                lineRenderer2 = line2.GetComponent<LineRenderer>();
               // Debug.Log(line2.name);
               // Debug.Log(lineRenderer2.name);

                int nextLineNumber = lineNumber + 1;
                string name = "Line_" + nextLineNumber + "_parent";
                Transform temp = this.transform.parent.parent.Find(name); // BAD FIND
                middleNodeNextLineParentComp = temp.GetComponent<LineGroupScript>();
                break;

            case NodeType.EndNoteNode:
                // attached to the this line number segment number
                bezLineRendererName = "Line_" + lineNumber + "_Segment_" + segmentNumber;
                line1 = GameObject.Find(bezLineRendererName);
                lineRenderer1 = line1.GetComponent<LineRenderer>();
              //  Debug.Log(line1.name);
                break;

            case NodeType.SmallNode:
                // attached to this line number at seg number this plus seg number + 1

                bezLineRendererName = "Line_" + lineNumber + "_Segment_" + (segmentNumber - 1);
                line1 = GameObject.Find(bezLineRendererName);
                lineRenderer1 = line1.GetComponent<LineRenderer>();
             //   Debug.Log(line1.name);

                bezLineRendererName = "Line_" + lineNumber + "_Segment_" + segmentNumber;
                line2 = GameObject.Find(bezLineRendererName);
                lineRenderer2 = line2.GetComponent<LineRenderer>();
              //  Debug.Log(line2.name);
             //   Debug.Log(lineRenderer2.name);
                break;
        }

       // Debug.Log(" set up complete " +  transform.name);

    }

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        if (GameManager.Instance.drawnVisible)
        {
            base.OnSelectExited(interactor);

            AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;


            if (interactor.GetType().Name == "GrabInteractor")
            {


                // currentPoisition = this.transform.position;

                if (meshColliderEnabled)
                {
                    switch (nodeType)
                    {

                        case NodeType.StartNoteNode:
                            this.transform.parent.GetComponent<LineGroupScript>().updateControlPointsStartNode(lineRenderer1, this.transform.position, segmentNumber, nodeNumber, false);

                            this.transform.parent.GetComponent<LineGroupScript>().destroyOldMeshMakeNewMesh(lineRenderer1, line1.GetComponent<MeshCollider>(), line1);
                            // Debug.Log("grab select exit UPDATE LINE");

                            break;

                        case NodeType.MiddleNoteNode:
                            this.transform.parent.GetComponent<LineGroupScript>().updateControlPointsEndNode(lineRenderer1, this.transform.position, segmentNumber, nodeNumber, false);
                            middleNodeNextLineParentComp.updateControlPointsStartNode(lineRenderer2, this.transform.position, 0, 0, false);

                            this.transform.parent.GetComponent<LineGroupScript>().destroyOldMeshMakeNewMesh(lineRenderer1, line1.GetComponent<MeshCollider>(), line1);
                            middleNodeNextLineParentComp.destroyOldMeshMakeNewMesh(lineRenderer2, line2.GetComponent<MeshCollider>(), line2);
                            // Debug.Log("grab select exit UPDATE LINE");

                            break;

                        case NodeType.EndNoteNode:
                            this.transform.parent.GetComponent<LineGroupScript>().updateControlPointsEndNode(lineRenderer1, this.transform.position, segmentNumber, nodeNumber, false);

                            this.transform.parent.GetComponent<LineGroupScript>().destroyOldMeshMakeNewMesh(lineRenderer1, line1.GetComponent<MeshCollider>(), line1);
                            //Debug.Log("grab select exit UPDATE LINE");

                            break;

                        case NodeType.SmallNode:
                            this.transform.parent.GetComponent<LineGroupScript>().updateControlPointsSmallNode(lineRenderer2, lineRenderer1, this.transform.position, segmentNumber, nodeNumber, false);
                            this.transform.parent.GetComponent<LineGroupScript>().destroyOldMeshMakeNewMesh(lineRenderer1, line1.GetComponent<MeshCollider>(), line1);
                            this.transform.parent.GetComponent<LineGroupScript>().destroyOldMeshMakeNewMesh(lineRenderer2, line2.GetComponent<MeshCollider>(), line2);
                            // Debug.Log("grab select exit UPDATE LINE");

                            break;
                    }
                }
            }
        }
    }

    protected override void OnHoverEntering(XRBaseInteractor interactor)
    {
        if (GameManager.Instance.drawnVisible)
        {


            //Debug.LogWarning("On hover entering ");
          //  Debug.LogWarning(interactor.interactablesHovered.Count);
           

            base.OnHoverEntering(interactor);



            if (interactor.GetType().Name == "GrabInteractor")
            {

                //Debug.Log("on hover enter");
                // Debug.Log("current state: " + InteractionManager.Instance.getCurrentState());
                // Debug.Log("left state: " + InteractionManager.Instance.lhState);
                // Debug.Log("right state: " + InteractionManager.Instance.rhState);
                if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Idle)
                {
                    //Debug.Log("state was idle: moving allowed");
                    //InteractionManager.Instance.finishedSculpting = false;
                    InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Sculpting);
                    if (interactor.transform.parent.name.Contains("Right"))
                    {
                        handed = "right";
                        InteractionManager.Instance.rhState = InteractionManager.HandState.Sculpting;
                    }
                    else
                    {
                        handed = "left";
                        InteractionManager.Instance.lhState = InteractionManager.HandState.Sculpting;
                    }

                    // im allowed to sculpt so normal behaviour


                    //currentPoisition = this.transform.position;


                }

                else if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Drawing) //SAYS THIS
                {
                    AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;
                    //Debug.Log("state is drawing: no moving allowed");

                }
                else if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Sculpting)
                {
                    // Debug.Log("state was sculpt, continue?");
                }
            }
        }
    }




    protected override void OnHoverExited(XRBaseInteractor interactor)
    {
        //Debug.LogWarning("On hover exited");
        if (GameManager.Instance.drawnVisible)
        {


            if (interactor.GetType().Name == "GrabInteractor")
            {
                if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Sculpting && !IsGrabSelected)
                {
                    
                    //InteractionManager.Instance.finishedSculpting = false;

                    InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Idle);
                    if (handed == "left")
                    {
                        InteractionManager.Instance.lhState = InteractionManager.HandState.Idle;
                    }
                    else
                    {
                        InteractionManager.Instance.rhState = InteractionManager.HandState.Idle;
                    }

                }
            }
            base.OnHoverExited(interactor);
        }
    }

    [Obsolete]
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
       // Debug.LogWarning("On select entered");
        if (GameManager.Instance.drawnVisible)
        {


            if (interactor.GetType().Name == "GrabInteractor")
            {

                // Debug.Log("on select enter");
                // Debug.Log("current state: " + InteractionManager.Instance.getCurrentState());
                // Debug.Log("left state: " + InteractionManager.Instance.lhState);
                // Debug.Log("right state: " + InteractionManager.Instance.rhState);

                // depending on which hand is interateracting check if that hand is idle, if it is great set it to sculpt

                if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Idle)
                {
                    // Debug.Log("state was idle: moving allowed");
                    LogManager.Instance.IncrementNumSculpt();
                    InteractionManager.Instance.finishedSculpting = false;
                    InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Sculpting);
                    if (interactor.transform.parent.name.Contains("Right"))
                    {
                        handed = "right";
                        InteractionManager.Instance.rhState = InteractionManager.HandState.Sculpting;
                    }
                    else
                    {
                        handed = "left";
                        InteractionManager.Instance.lhState = InteractionManager.HandState.Sculpting;
                    }
                    base.OnSelectEntered(interactor);
                }

                else if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Drawing) //SAYS THIS
                {
                    AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;
                    Debug.Log("to idel");
                    if (InteractionManager.Instance.rhState != InteractionManager.HandState.Drawing)
                    {
                        
                        InteractionManager.Instance.rhState = InteractionManager.HandState.Idle;
                    }
                    if (InteractionManager.Instance.lhState != InteractionManager.HandState.Drawing)
                    {
                        InteractionManager.Instance.lhState = InteractionManager.HandState.Idle;
                    }
                }
                else if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Sculpting)
                {
                    //Debug.Log("on select entered: already sculpting?");
                    InteractionManager.Instance.finishedSculpting = false;
                    LogManager.Instance.IncrementNumSculpt();

                    if (interactor.transform.parent.name.Contains("Right"))
                    {
                        if (InteractionManager.Instance.rhState == InteractionManager.HandState.Idle)
                        {
                            InteractionManager.Instance.rhState = InteractionManager.HandState.Sculpting;
                        }
                    }
                    else if (interactor.transform.parent.name.Contains("Left"))
                    {
                        if (InteractionManager.Instance.lhState == InteractionManager.HandState.Idle)
                        {
                            InteractionManager.Instance.lhState = InteractionManager.HandState.Sculpting;
                        }
                    }
                    base.OnSelectEntered(interactor);
                }
            }
        }
    }

    [Obsolete]
    protected override void OnSelectExiting(XRBaseInteractor interactor)
    {
       // Debug.LogWarning("On select exiting");
        if (GameManager.Instance.drawnVisible)
        {

            base.OnSelectExiting(interactor);

            AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;


            if (interactor.GetType().Name == "GrabInteractor")
            {
                //Debug.Log("onselectexit");
                // Debug.Log("current state: " + InteractionManager.Instance.getCurrentState());
                // Debug.Log("left state: " + InteractionManager.Instance.lhState);
                //  Debug.Log("right state: " + InteractionManager.Instance.rhState);

                if (InteractionManager.Instance.getCurrentState() == InteractionManager.InteractionState.Sculpting)
                {

                    InteractionManager.Instance.finishedSculpting = false;  ///surely this should be true
                    Debug.Log("to idel");
                    InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Idle);
                    if (handed == "left")
                    {
                        InteractionManager.Instance.lhState = InteractionManager.HandState.Idle;
                        InteractionManager.Instance.leftIsPinch = false;

                    }
                    else
                    {
                        InteractionManager.Instance.rhState = InteractionManager.HandState.Idle;
                        InteractionManager.Instance.rightIsPinch = false;
                    }

                }
            }
        }
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

    public void SetLineNumber(int number)
    {
        lineNumber = number;
    }

    public void setLine1(GameObject line_1) { line1 = line_1; }
    public GameObject getLine1() { return line1; }

    public void setLine2(GameObject line_2) { line2 = line_2; }
    public GameObject getLine2() { return line2; }

    public void SetNotdeType(NodeType nodeType_) { nodeType = nodeType_; }

    public NodeType GetNodeType() { return nodeType; }

}