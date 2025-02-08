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
    public GameObject wristRotationCube;

    private HandJointPose leftPoseIndexTip, rightPoseIndexTip;
    private HandJointPose leftWritstPose, rightWritstPose;

    public GameObject leftDrawObject, rightDrawObject;
    public Draw_Line leftDrawLineScript, rightDrawLineScript;

    private int globalLineCounter = 0;
    private int globalGrandParentCounter = 0;
    public GameObject currentStateCube, leftStateCube, rightStateCube

    public bool finishedSculpting = true;
    private float coolDownTimer = 0.0f;
    private float coolDownAmount = 2.0f; //.25
    private int numberTimesSculptFinished = 0;

    private float lastAngleL = 0.0f;
    private float lastAngleR = 0.0f;

    private float duration = 0.5f;
    private float timer = 0f;

    private float durationNodeSpawn = 0.5f;
    private float timerNodeSpawn = 0f;
    private bool nodeSpawnAllowed = true;
    
    public float armLength = 0.5f;

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

    public InteractionState lhState, rhState;
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

        // true if we have not finished sculpting
        // wait a moment before being able to draw 
        if (finishedSculpting == false){ sculptCoolDown();}

        if (systemState != InteractionState.Sculpting && finishedSculpting == true)
        {
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

        // limiting how often nodes can be drawn
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

        // debugging purposes 
        currentStateCubeColour();
        leftStateCubeColour();
        rightStateCubeColour();

        allowOveralSolver = !(rhState == InteractionState.Sculpting && lhState == InteractionState.Sculpting);
    }

    void HandleHandInteraction(float pinchAmount, ref InteractionState handState, GameObject handDrawLineScript, GameObject TMT_Collider_Ball, ref Vector3 indexTipPos, GameObject handNode, ref bool nodeSpawnAllowed, ref InteractionState systemState, bool isRightHand)
    {
        float handY = Camera.main.transform.position.y - indexTipPos.y;

        if ((pinchAmount > 0.8f) && handState != InteractionState.Sculpting && (handY < armLength))  // Hand is pinching
        {
            // Update brush position
            Vector3 brushPosition = indexTipPos;

            if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
            {
                // fix z position if within certain dist of tmt nodes
                if (Vector3.Distance(TMTManager.Instance.origin, brushPosition) < TMT_brushSnapDist)
                {
                    brushPosition = new Vector3(brushPosition.x, brushPosition.y, (TMTManager.Instance.origin.z + 0.005f));
                }
                TMT_Collider_Ball.transform.position = brushPosition;
            }

            // Set brush position for drawing
            handDrawLineScript.SetPosition(brushPosition);

            if (finishedSculpting)
            {
                if (handState == InteractionState.Idle && systemState == InteractionState.Idle)
                {
                    // Create brush
                    handState = InteractionState.Drawing;
                    systemState = InteractionState.Drawing;
                    handDrawLineScript.CreateBrush();
                }
                else if (handState == InteractionState.Drawing)
                {
                    handDrawLineScript.AddPoint();
                    if (handDrawLineScript.currentLineRenderer.positionCount % 5 == 0 && nodeSpawnAllowed)
                    {
                        if (degreeDiff(handNode, isRightHand ? rightWristPose.Rotation.eulerAngles.z : leftWristPose.Rotation.eulerAngles.z, 30.0f))
                        {
                            handDrawLineScript.WristSplit();
                            nodeSpawnAllowed = false;
                        }
                    }
                }
            }
        }
        else if (handState == InteractionState.Drawing) // Hand was drawing but is no longer pinching
        {
            // Stop Drawing
            handState = InteractionState.Idle;
            systemState = InteractionState.Idle;
            handDrawLineScript.StopDrawing(false);
        }
    }

    void RightHand(float pinchAmount)
    {
        HandleHandInteraction(pinchAmount, ref rhState, rightDrawLineScript, TMT_Collider_Ball, rightPoseIndexTip.Position, rightHandNode, ref nodeSpawnAllowed, ref systemState, true);
    }

    void LeftHand(float pinchAmount)
    {
        HandleHandInteraction(pinchAmount, ref lhState, leftDrawLineScript, TMT_Collider_Ball, leftPoseIndexTip.Position, leftHandNode, ref nodeSpawnAllowed, ref systemState, false);
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


    void SetCubeColorBasedOnState(GameObject cube, InteractionState state)
    {
        Color color = Color.black;

        switch (state)
        {            
            case InteractionState.Drawing:
                color = Color.red;
                break;
            case InteractionState.Sculpting:
                color = Color.blue;
                break;
        }

        cube.GetComponent<Renderer>().material.color = color;
    }

    void currentStateCubeColour()
    {
        SetCubeColorBasedOnState(currentStateCube, currentState);
    }

    void rightStateCubeColour()
    {
        SetCubeColorBasedOnState(rightStateCube, rhState);
    }

    void leftStateCubeColour()
    {
        SetCubeColorBasedOnState(leftStateCube, lhState);
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

}
