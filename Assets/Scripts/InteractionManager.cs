using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.UX;
using UnityEngine.XR;
using System.Linq;

public class InteractionManager : MonoBehaviour
{
    private static InteractionManager _instance;
    public static InteractionManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Interaction Manager is NUll!");

            return _instance;
        }
    }

    private XRNode leftHandNode = XRNode.LeftHand;
    private XRNode rightHandNode = XRNode.RightHand;

    private bool leftIsReadyPinch = false;
    public bool leftIsPinch = false;
    private float leftPinchAmount = 0.0f;
    private bool rightIsReadyPinch = false;
    public bool rightIsPinch = false;
    private float rightPinchAmount = 0.0f;

    public GameObject handSphere;
    HandJointPose leftPoseIndexTip;
    HandJointPose leftWritstPose;
    HandJointPose rightPoseIndexTip;
    HandJointPose rightWritstPose;

    public GameObject rightDrawObject;

    public Draw_Line rightDrawLineScript;

    public GameObject leftDrawObject;

    public Draw_Line leftDrawLineScript;

    private int globalLineCounter = 0;
    private int globalGrandParentCounter = 0;
    public GameObject currentStateCube;
    public GameObject leftStateCube;
    public GameObject rightStateCube;

    public bool finishedSculpting = true;
    private float coolDownTimer = 0.0f;
    private float coolDownAmount = 2.0f; //.25

    private float lastAngleL = 0.0f;
    private float lastAngleR = 0.0f;

    private float duration = 0.5f;
    private float timer = 0f;

    private float durationNodeSpawn = 0.5f;
    private float timerNodeSpawn = 0f;
    private bool nodeSpawnAllowed = true;

    private int numberTimesSculptFinished = 0;

    public float armLength = 0.5f;

    public GameObject wristRotationCube;

    public GameObject TMT_Collider_Ball;
    GameObject _TMT_Collider_Ball;
    [SerializeField]
    public float TMT_brushSnapDist = 0.3f;

    public bool allowOveralSolver = true;

    public enum InteractionState
    {
        Idle,
        Drawing,
        Sculpting,
    }

    public InteractionState rhState;
    public InteractionState lhState;
    private InteractionState systemState;

    private void Awake()
    {
        _instance = this;
        systemState = InteractionState.Idle;
        rhState = InteractionState.Idle;
        lhState = InteractionState.Idle;
        Debug.Log("hi from the InteractionManager");
    }
    void Start()
    {
        rightDrawLineScript = rightDrawObject.GetComponent<Draw_Line>();
        leftDrawLineScript = leftDrawObject.GetComponent<Draw_Line>();
        TMT_Collider_Ball = Instantiate(TMT_Collider_Ball);
    }

    void Update()
    {
        XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Wrist, leftHandNode, out leftWritstPose);
        XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Wrist, rightHandNode, out rightWritstPose);

        XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip, leftHandNode, out leftPoseIndexTip);
        XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip, rightHandNode, out rightPoseIndexTip);

        XRSubsystemHelpers.HandsAggregator.TryGetPinchProgress(leftHandNode, out leftIsReadyPinch, out leftIsPinch, out leftPinchAmount);
        XRSubsystemHelpers.HandsAggregator.TryGetPinchProgress(rightHandNode, out rightIsReadyPinch, out rightIsPinch, out rightPinchAmount);

        if (finishedSculpting == false)
        {
            // if is true if we have not finished sculpting
            // just stopped sculpting wait a moment before being able to draw 
            sculptCoolDown();
        }

        if (systemState != InteractionState.Sculpting && finishedSculpting == true)
        {
            //true if idle or drawing & finishee sculpting
                RightHand(rightPinchAmount);
                LeftHand(leftPinchAmount);
        }

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            //check wrist angle every interval of timer
            timer = 0f;
            degreeDiff(rightHandNode, rightWritstPose.Rotation.eulerAngles.z, 30.0f);
            degreeDiff(leftHandNode, leftWritstPose.Rotation.eulerAngles.z, 30.0f);
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

        currentStateCubeColour();
        leftStateCubeColour();
        rightStateCubeColour();

        if(rhState == InteractionState.Sculpting && lhState == InteractionState.Sculpting)
        {
            //block overlapsolvers
            allowOveralSolver = false;
        }
        else 
        {
            //allow
            allowOveralSolver = true;
        }
    }



    void RightHand(float pinchAmount)
    {
        float handY = Camera.main.transform.position.y - rightPoseIndexTip.Position.y;

        if ((pinchAmount > 0.8f) && rhState != InteractionState.Sculpting && (handY < armLength))  // rightIsPinch
        {
            Vector3 brushPosition = rightPoseIndexTip.Position;

            if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
            {
                float dist = Vector3.Distance(TMTManager.Instance.origin, brushPosition);
                if (dist < TMT_brushSnapDist)
                {
                    brushPosition = new Vector3(rightPoseIndexTip.Position.x, rightPoseIndexTip.Position.y, (TMTManager.Instance.origin.z + 0.005f));
                }
                TMT_Collider_Ball.transform.position = brushPosition;
            }

            rightDrawLineScript.SetPosition(brushPosition);

            if (finishedSculpting)
            {
                if (rhState == InteractionState.Idle && systemState == InteractionState.Idle)
                {
                    //create brush
                    rhState = InteractionState.Drawing;
                    systemState = InteractionState.Drawing;
                    rightDrawLineScript.CreateBrush();
                }
                else if (rhState == InteractionState.Drawing)
                {
                    rightDrawLineScript.AddPoint();

                    if (rightDrawLineScript.currentLineRenderer.positionCount % 5 == 0 && nodeSpawnAllowed)
                    {

                        if (degreeDiff(rightHandNode, rightWritstPose.Rotation.eulerAngles.z, 30.0f))
                        {
                            rightDrawLineScript.WristSplit();
                            nodeSpawnAllowed = false;
                        }
                    }
                }
            }
        }
        else if (rhState == InteractionState.Drawing) //right hand was drawing but is no longer pinching 
        {
            //Stop Drawing
            rhState = InteractionState.Idle;
            systemState = InteractionState.Idle;
            rightDrawLineScript.StopDrawing(false);
        }
    }

    void LeftHand(float pinchAmount)
    {
        float handY = Camera.main.transform.position.y - leftPoseIndexTip.Position.y;

        if ((pinchAmount > 0.8f) && lhState != InteractionState.Sculpting && (handY < armLength))
            Vector3 brushPos = leftPoseIndexTip.Position;
            if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
            {
                float dist = Vector3.Distance(TMTManager.Instance.origin, brushPos);
                if (dist < TMT_brushSnapDist)
                {
                    brushPos = new Vector3(leftPoseIndexTip.Position.x, leftPoseIndexTip.Position.y, (TMTManager.Instance.origin.z + 0.005f));
                }
                TMT_Collider_Ball.transform.position = brushPos;
            }

            leftDrawLineScript.SetPosition(brushPos); // + offsetVec

            if (finishedSculpting)
            {
                if (lhState == InteractionState.Idle && systemState == InteractionState.Idle)
                {
                    //create brush
                    lhState = InteractionState.Drawing;
                    systemState = InteractionState.Drawing;
                    leftDrawLineScript.CreateBrush();
                }
                else if (lhState == InteractionState.Drawing)
                {

                    leftDrawLineScript.AddPoint();

                    if (leftDrawLineScript.currentLineRenderer.positionCount % 5 == 0 && nodeSpawnAllowed)
                    {
                        if (degreeDiff(leftHandNode, leftWritstPose.Rotation.eulerAngles.z, 30.0f))
                        {

                            leftDrawLineScript.WristSplit();
                            nodeSpawnAllowed = false;
                        }
                    }
                }
            }
        }
        else if (lhState == InteractionState.Drawing) //right hand was drawing but is no longer pinching 
        {
            //Stop Drawing
            lhState = InteractionState.Idle;
            currentState = InteractionState.Idle;
            leftDrawLineScript.StopDrawing(false);
        }
    }

    public InteractionState getCurrentState()
    {
        return currentState;
    }

    public void setCurrentState(InteractionState state)
    {
        currentState = state;
    }

    public void incrementGlobalLineCounter()
    {
        globalLineCounter++;
    }

    public int getGlobalLineCounter()
    {
        return globalLineCounter;
    }

    public void incrementGlobalGrandParentCounter()
    {
        globalGrandParentCounter++;
    }

    public int getGlobalGrandParentCounter()
    {
        return globalGrandParentCounter;
    }


    void currentStateCubeColour()
    {
        switch (currentState)
        {
            case InteractionState.Idle:
                //set col;
                currentStateCube.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
                break;
            case InteractionState.Drawing:
                //set coll
                currentStateCube.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                break;
            case InteractionState.Sculpting:
                currentStateCube.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                break;
        }
    }

    void rightStateCubeColour()
    {
        switch (rhState)
        {
            case InteractionState.Idle:
                //set col;
                rightStateCube.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
                break;
            case InteractionState.Drawing:
                //set coll
                rightStateCube.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                break;
            case InteractionState.Sculpting:
                rightStateCube.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                break;
        }
    }

    void leftStateCubeColour()
    {
        switch (lhState)
        {
            case InteractionState.Idle:
                //set col;
                leftStateCube.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
                break;
            case InteractionState.Drawing:
                //set coll
                leftStateCube.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                break;
            case InteractionState.Sculpting:
                leftStateCube.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                break;
        }
    }

    // timer used to temp block drawing  after sculpting
    void sculptCoolDown()
    {
        coolDownTimer += Time.deltaTime;
        if (coolDownTimer >= coolDownAmount)
        {
            coolDownTimer = 0f;
            finishedSculpting = true;
            Debug.Log("sculpt cool dow over");
        }
    }

    public bool degreeDiff(XRNode thisNode, float current, float angleAmt)
    {
        float previous = 0.0f;
        if (thisNode.IsLeftHand())
        {
            previous = lastAngleL;
        }
        else if (thisNode.IsRightHand())
        {
            previous = lastAngleR;
        }

        float diff = 180 - Mathf.Abs(Mathf.Abs(previous - current) - 180); 
        bool changed = diff > angleAmt;

        if (changed)
        {
            if (thisNode.IsLeftHand())
            {
                lastAngleL = leftWritstPose.Rotation.eulerAngles.z;
            }
            else if (thisNode.IsRightHand())
            {
                lastAngleR = rightWritstPose.Rotation.eulerAngles.z;
            }
        }
        return changed;
    }
    /*
    void HeadHandAngle()
    {
    Vector3 headToHand = rightPoseIndexTip.Position - Camera.main.transform.position; 
    Vector3 headToControl = 
    }
    */


}
