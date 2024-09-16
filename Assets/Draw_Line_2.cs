using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.UX;
using UnityEngine.XR;

public class Draw_Line_2 : MonoBehaviour
{

    public GameObject brush;
    public GameObject sphereNode;
    public GameObject wristRotationCube;

    GameObject theWrist;

    LineRenderer currentLineRenderer;

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

    private bool isReadyPinch, isPinch;
    private float pinchAmount;

    public float  rotationThresholdSlider  = 0.995f;

    private float duration = 0.5f;
    private float timer = 0f;

    private void Start()
    {
        Debug.LogError("Draw_line Start Up");
        Debug.developerConsoleVisible = true;

        theWrist = Instantiate(wristRotationCube, this.transform);

    }


/*    private void OnGUI()
    {
        if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Wrist, node, out writstPoser)){

            float diff = Mathf.Abs(Quaternion.Dot(writstPoser.Rotation, lastQuat));


            GUI.Box(new Rect(10, 10, 300, 20), writstPoser.Rotation.eulerAngles.ToString());
            GUI.Box(new Rect(10, 30, 300, 20), rotationThresholdSlider.ToString());

        }

    }*/

    private void Update()
    {
        theWrist.GetComponent<Renderer>().enabled = false;

        if (HandsUtils.GetSubsystem().TryGetJoint(TrackedHandJoint.IndexTip, XRNode.LeftHand, out poseIndexTip))
        {
            node = XRNode.LeftHand;
            XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Wrist, node, out writstPoser);
            //Debug.Log("Wrist L: " + writstPoser.Rotation.eulerAngles);
            theWrist.GetComponent<Renderer>().enabled = true;
            theWrist.transform.position = writstPoser.Position;
            theWrist.transform.rotation = writstPoser.Rotation;


            if (XRSubsystemHelpers.HandsAggregator.TryGetPinchProgress(node, out isReadyPinch, out isPinch, out pinchAmount))
            {
               // Debug.Log("pinchy: " + pinchAmount);

            }
        }
        else if (HandsUtils.GetSubsystem().TryGetJoint(TrackedHandJoint.IndexTip, XRNode.RightHand, out poseIndexTip))
        {
            node = XRNode.RightHand;
            XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Wrist, node, out writstPoser);
            //Debug.Log("Wrist R: " + writstPoser.Rotation.eulerAngles);
            theWrist.GetComponent<Renderer>().enabled = true;
            theWrist.transform.position = writstPoser.Position;
            theWrist.transform.rotation = writstPoser.Rotation;

            if (XRSubsystemHelpers.HandsAggregator.TryGetPinchProgress(node, out isReadyPinch, out isPinch, out pinchAmount))
            {
               // Debug.Log("pinchy: " + pinchAmount);
            }
        }
        else if (clicked) {
/*            GameObject sphereNodeInstance = Instantiate(sphereNode);
            sphereNodeInstance.transform.position = pose.Position;*/
            currentLineRenderer = null;
            clicked = false;
            pinchAmount = 0.0f;
            Debug.Log("UnClicked");
        }

        Draw();

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            timer = 0f;
            Debug.Log("last angle: " + lastAngle);
            Debug.Log("curr angle: " + writstPoser.Rotation.eulerAngles.z);

            degreeDiff90(lastAngle, writstPoser.Rotation.eulerAngles.z, 60.0f);
        }


    }

    void Draw()
    {
        if (pinchAmount > 0.9 && !clicked) //I have just pinched, create brush
        {
            CreateBrush();

        }
        if (pinchAmount > 0.9 && clicked) //I conintue to pinch, keep drawing
        {
            Vector3 indexPose = poseIndexTip.Position;
            if (Vector3.Distance(indexPose, lastPos) > minDistance)
            {
                AddPoint(indexPose);
                lastPos = indexPose;

                if (currentLineRenderer.positionCount%5 == 0)
                {

                    Debug.LogError(writstPoser.Rotation);

                    if (degreeDiff90(lastAngle, writstPoser.Rotation.eulerAngles.z, 60.0f))

                    {
                        GameObject sphereNodeInstance = Instantiate(sphereNode);
                        sphereNodeInstance.transform.position = poseIndexTip.Position;
                        Debug.Log("Not Close Enough");


                    }
                    /*
                                        GameObject sphereNodeInstance = Instantiate(sphereNode);
                                        sphereNodeInstance.transform.position = pose.Position;*/

                    lastQuat = writstPoser.Rotation;
                    lastAngle = writstPoser.Rotation.eulerAngles.x;

                }

            }
        }
        if (pinchAmount < 0.8 && clicked) // I have stopped pinching, stop drawing
        {
/*            GameObject sphereNodeInstance = Instantiate(sphereNode);
            sphereNodeInstance.transform.position = pose.Position;*/
            currentLineRenderer = null;
            clicked = false;
        }
    }

    // Update is called once per frame
    void CreateBrush()
    {
        Debug.Log("Create Brush");

        GameObject brushInstance = Instantiate(brush);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();

        Vector3 indexPose = poseIndexTip.Position;

        GameObject sphereNodeInstance = Instantiate(sphereNode);
        sphereNodeInstance.transform.position = poseIndexTip.Position;

        lastPos = indexPose;
        lastQuat = writstPoser.Rotation;
        lastEuler = writstPoser.Rotation.eulerAngles;
        lastAngle = writstPoser.Rotation.eulerAngles.z;

        currentLineRenderer.SetPosition(0, indexPose);
        currentLineRenderer.SetPosition(1, indexPose);

        clicked = true;

    }

    void AddPoint(Vector3 pointPos)
    {
        currentLineRenderer.positionCount++;
        int posIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(posIndex, pointPos);
    }

    public static bool isApproximate(Quaternion q1, Quaternion q2, float precision)
    {
        Debug.LogError(Mathf.Abs(Quaternion.Dot(q1, q2)));
        return Mathf.Abs(Quaternion.Dot(q1, q2)) < precision;
    }

    public bool degreeDiff90(float previous, float current, float angleAmt)
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

    public void setRotationThresholdSlider(float inputVal) {
        rotationThresholdSlider = inputVal;
        Debug.Log("rotationThresholdSlider: " + rotationThresholdSlider);
    }


    public void testSet(Slider slider)
    {
        Debug.Log(slider.Value);
        setRotationThresholdSlider(slider.Value);
    }

    private void createNode() {
        
    }

}
