using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.UX;
using UnityEngine.XR;
using System.Linq;

public class Draw_Line_3 : MonoBehaviour
{

    public GameObject brush;
    public GameObject brushBezier;
    public GameObject sphereNode;
    public GameObject wristRotationCube;
    public GameObject lineGroupParent;

    GameObject theWrist;

    LineRenderer currentLineRenderer;
    LineRenderer bezierLineRenderer;

    Vector3 lastPos;
    Quaternion lastQuat;
    Vector3 lastEuler;
    float lastAngle;


    [SerializeField]
    private float minDistance = 0.01f;
    public bool clicked = false;
    HandJointPose poseIndexTip;
    HandJointPose writstPoser;

    private enum Hand { LeftHand, RightHand };
    private Hand _hand;
    private XRNode node = XRNode.LeftHand;

    private bool isReadyPinch = false;
    private bool isPinch = false;
    private float pinchAmount;

    public float rotationThresholdSlider = 0.995f;

    private float duration = 0.5f;
    private float timer = 0f;

    private float durationPinchMissing = 0.5f;
    private float timerPinchMissing = 0f;
    private bool pinchVisible;

    private bool handVisible = false;

    private float durationNodeSpawn = 1f;
    private float timerNodeSpawn = 0f;
    private bool nodeSpawnAllowed = true;

    public int lineCounter = 0;
    public int segmentCounter = 0;


    private void Start()
    {
        //Debug.LogError("Draw_line Start Up");
        Debug.developerConsoleVisible = true;
        theWrist = Instantiate(wristRotationCube, this.transform);
    }


    private void Update()
    {
        theWrist.GetComponent<Renderer>().enabled = false;
        //Debug.Log("isReadyToPinch : " + isReadyPinch);
        //Debug.Log("isPinch : " + isPinch);

        if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Wrist, XRNode.LeftHand, out writstPoser))
        {
            node = XRNode.LeftHand;
            theWrist.GetComponent<Renderer>().enabled = false; //true to show
            theWrist.transform.position = writstPoser.Position;
            theWrist.transform.rotation = writstPoser.Rotation;
            handVisible = true;
            //Debug.Log("left");
        }
        else if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Wrist, XRNode.RightHand, out writstPoser))
        {
            node = XRNode.RightHand;
            theWrist.GetComponent<Renderer>().enabled = false; //true to show
            theWrist.transform.position = writstPoser.Position;
            theWrist.transform.rotation = writstPoser.Rotation;
            handVisible = true;
            //Debug.Log("Right");
        }
        else
        {
            handVisible = false;
        }

        if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip, node, out poseIndexTip))
        {
           // Debug.Log("indexTip");
        }

        if (XRSubsystemHelpers.HandsAggregator.TryGetPinchProgress(node, out isReadyPinch, out isPinch, out pinchAmount))
        {
            if (isPinch)
            {
                pinchVisible = true;
               // Debug.Log("is pinching");
            }
            else
            {
                pinchVisible = false;
            }

        }
        else if (pinchVisible && (handVisible && InteractionManager.Instance.getCurrentState().Equals(InteractionManager.InteractionState.Drawing))) //i am currently drawing but i cannot see pinch pose
        {
            theWrist.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
            pinchHidden();
        }
        else if (!handVisible && InteractionManager.Instance.getCurrentState().Equals(InteractionManager.InteractionState.Drawing))
        {
            //GameObject sphereNodeInstance = Instantiate(sphereNode);
            //sphereNodeInstance.transform.position = currentLineRenderer.GetPosition(currentLineRenderer.positionCount - 1);

            DrawnLineToBeziers();

            currentLineRenderer = null;
            clicked = false;
            InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Idle);
            pinchVisible = false;
            pinchAmount = 0.0f;
            Debug.Log("UnClicked");
        }

       // if (node == XRNode.RightHand) { Draw(); }
        if (( InteractionManager.Instance.getCurrentState().Equals(InteractionManager.InteractionState.Idle)) || ((InteractionManager.Instance.getCurrentState().Equals(InteractionManager.InteractionState.Drawing)))) { 
            Draw(); 
        } // if I am not sculpting I can draw


        Vector3.Distance(Camera.main.transform.position, writstPoser.Position);

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            timer = 0f;
           // Debug.Log("last angle: " + lastAngle);
           // Debug.Log("curr angle: " + writstPoser.Rotation.eulerAngles.z);

            degreeDiff(lastAngle, writstPoser.Rotation.eulerAngles.z, 30.0f);
        }

        if (!nodeSpawnAllowed)
        {
            timerNodeSpawn += Time.deltaTime;
            if (timerNodeSpawn >= durationNodeSpawn)
            {
                timerNodeSpawn = 0f;
                nodeSpawnAllowed = true;
                Debug.Log("nodeSpawnAllowed = true");
            }
        }


    }

    void pinchHidden()
    {
        timerPinchMissing += Time.deltaTime;
        if (timerPinchMissing >= 3.0f)
        {
            timerPinchMissing = 0f;
            pinchVisible = false;
            theWrist.GetComponent<Renderer>().material.color = new Color(0, 255, 255);

        }
    }


    void Draw()
    {
        if (pinchVisible && InteractionManager.Instance.getCurrentState().Equals(InteractionManager.InteractionState.Idle)) //I have just pinched, create brush
        {
            InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Drawing);
            CreateBrush();
        }
        if (pinchVisible && InteractionManager.Instance.getCurrentState().Equals(InteractionManager.InteractionState.Drawing)) //I conintue to pinch, keep drawing
        {
            Vector3 indexPose = poseIndexTip.Position;
            if (Vector3.Distance(indexPose, lastPos) > minDistance)
            {
                AddPoint(indexPose);
                lastPos = indexPose;

                if (currentLineRenderer.positionCount % 10 == 0 && nodeSpawnAllowed)
                {

                    if (degreeDiff(lastAngle, writstPoser.Rotation.eulerAngles.z, 30.0f))

                    {
                        GameObject sphereNodeInstance = Instantiate(sphereNode);
                        sphereNodeInstance.transform.position = poseIndexTip.Position;
                        nodeSpawnAllowed = false;
                        Debug.Log("Not Close Enough");


                    }
                }
            }
        }

        if (!pinchVisible && InteractionManager.Instance.getCurrentState().Equals(InteractionManager.InteractionState.Drawing)) // I have stopped pinching, stop drawing
        {
            //GameObject sphereNodeInstance = Instantiate(sphereNode);
            //sphereNodeInstance.transform.position = currentLineRenderer.GetPosition(currentLineRenderer.positionCount - 1);
            DrawnLineToBeziers();
            currentLineRenderer.forceRenderingOff = true;
            currentLineRenderer = null;
            clicked = false;
            InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Idle);

        }
    }

    // Update is called once per frame
    void CreateBrush()
    {
       // Debug.Log("Create Brush");

        GameObject brushInstance = Instantiate(brush);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();

        Vector3 indexPose = poseIndexTip.Position;

        //GameObject sphereNodeInstance = Instantiate(sphereNode);
        //sphereNodeInstance.transform.position = poseIndexTip.Position;

        lastPos = indexPose;
        lastQuat = writstPoser.Rotation;
        lastEuler = writstPoser.Rotation.eulerAngles;
        lastAngle = writstPoser.Rotation.eulerAngles.z;

        currentLineRenderer.SetPosition(0, indexPose);
        currentLineRenderer.SetPosition(1, indexPose);

        clicked = true;
        InteractionManager.Instance.setCurrentState(InteractionManager.InteractionState.Drawing);


    }

    void AddPoint(Vector3 pointPos)
    {
        currentLineRenderer.positionCount++;
        int posIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(posIndex, pointPos);
    }


    public bool degreeDiff(float previous, float current, float angleAmt)
    {
        float diff = 180 - Mathf.Abs(Mathf.Abs(previous - current) - 180);

        bool changed = diff > angleAmt;

        if (changed)
        {
            theWrist.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
        }
        else
        {
            theWrist.GetComponent<Renderer>().material.color = new Color(0, 255, 0);

        }
        lastAngle = writstPoser.Rotation.eulerAngles.z;
        return changed;
    }

    //Convert the line drawn by the user into one or more bezier curves
    void DrawnLineToBeziers()
    {
        // init parent 
        GameObject groupParent = Instantiate(lineGroupParent);
        groupParent.transform.name = "Line_" + lineCounter + "_parent";
        groupParent.GetComponent<LineGroupScript>().lineNumber = lineCounter;

        // get current line's points, conv to list
        currentLineRenderer.transform.SetParent(groupParent.transform);
        Vector3[] currentPoints = new Vector3[currentLineRenderer.positionCount];
        currentLineRenderer.GetPositions(currentPoints);
        List<Vector3> currentPointsList = currentPoints.ToList();

        // get the bez curve(s)
        CubicBezier[] curves = CurveFit.Fit(currentPointsList, 0.02f);
        groupParent.GetComponent<LineGroupScript>().setCurves(curves);

        for (int n = 0; n < curves.Length; n++)
        {
            Vector3[] bezPoints = BezierLineFunctions.BezierInterp(curves[n].p0, curves[n].p1, curves[n].p2, curves[n].p3, currentLineRenderer, 2);
            DrawBezier(bezPoints, groupParent);

            //NODE AT START OF EACH SEGEMENT
            GameObject sphereNodeInstanceP0 = Instantiate(sphereNode);
            sphereNodeInstanceP0.transform.position = curves[n].p0;
            sphereNodeInstanceP0.name = "NodeP0_" + "Line_" +lineCounter + "_Segment_" + segmentCounter;
            sphereNodeInstanceP0.transform.SetParent(groupParent.transform);
            sphereNodeInstanceP0.GetComponent<NodeSphere>().SetSegmentNumber(segmentCounter);
            sphereNodeInstanceP0.GetComponent<NodeSphere>().SetNodeNumber(0);

            //IF IF THE SEGMENT, ADD NODE TO END
            if (n == curves.Length - 1) {
                GameObject sphereNodeInstanceP3 = Instantiate(sphereNode);
                sphereNodeInstanceP3.transform.position = curves[n].p3;
                sphereNodeInstanceP3.name = "NodeP3_" + "Line_" + lineCounter + "_Segment_" + segmentCounter;
                sphereNodeInstanceP3.transform.SetParent(groupParent.transform);
                sphereNodeInstanceP3.GetComponent<NodeSphere>().SetSegmentNumber(segmentCounter);
                sphereNodeInstanceP3.GetComponent<NodeSphere>().SetNodeNumber(3);
            }
            segmentCounter++;
        }
        segmentCounter = 0;
        lineCounter++;
    }


    //CREATE A NEW LINE RENDERER FOR THE SEGMENT, SET POINTS OF BEZ CURVE AND GROUP TO PARENT
    void DrawBezier(Vector3[] bezSegmentPoints, GameObject groupParent) {
        GameObject brushInstance = Instantiate(brushBezier);
        bezierLineRenderer = brushInstance.GetComponent<LineRenderer>();
        bezierLineRenderer.name = "Line_" + lineCounter + "_Segment_" + segmentCounter;

        for (int k = 0; k < bezSegmentPoints.Length; k++)
        {
            if (k > 1)
            {
                bezierLineRenderer.positionCount++;
            }
            bezierLineRenderer.SetPosition(k, bezSegmentPoints[k]);
        }

        bezierLineRenderer.transform.SetParent(groupParent.transform);
        bezierLineRenderer = null;
    }

}
