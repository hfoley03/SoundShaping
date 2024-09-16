using MixedReality.Toolkit;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWithHand : MonoBehaviour
{

    private LineRenderer line;
    private Vector3 previousPosition;
    private IHandsAggregatorSubsystem aggregator;
    private ArticulatedHandController ahc;
    public GameObject go;
    private MRTKRayInteractor rayThing; 

    [SerializeField]
    private float minDistance = 0.01f;

    private void Start()
    {
        /*        aggregator = XRSubsystemHelpers.GetFirstRunningSubsystem<IHandsAggregatorSubsystem>();
                Debug.Log(aggregator);*/

        ahc = go.GetComponent<ArticulatedHandController>();
        Debug.Log("YESSSSS");
        Debug.Log(ahc.currentControllerState);

        rayThing = go.GetComponentInChildren<MRTKRayInteractor>();
        Debug.Log("YESSSSS");
        Debug.Log(rayThing.rayEndPoint);

        line = GetComponent<LineRenderer>();
        previousPosition = transform.position;
        line.positionCount = 1;
    }

    private void Update()
    {
        //Debug.Log(ahc.currentControllerState.uiPressInteractionState.value); //seems to show pinch amount 0 being none
        if (ahc.currentControllerState.uiPressInteractionState.value > 0.5) {
            //            Debug.Log(ahc.currentControllerState.position);
            //            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(ahc.currentControllerState.position.normalized);
            //Vector3 currentPosition = (ahc.currentControllerState.position);
            Vector3 currentPosition = (rayThing.rayEndPoint);


            Debug.Log(currentPosition);
/*            currentPosition.y = currentPosition.y + 1.6f;
            Debug.Log("currpos");
            Debug.Log(currentPosition);*/


            if (Vector3.Distance(currentPosition, previousPosition) > minDistance) 
            { 
                if (previousPosition == transform.position)
                {
                    line.SetPosition(0, currentPosition);

                }
                else 
                {
                    line.positionCount++;
                    line.SetPosition(line.positionCount - 1, currentPosition);
                }
                      

                        previousPosition = currentPosition;
             }
        }
    }

}
