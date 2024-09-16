using MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LineInteractable : MixedReality.Toolkit.SpatialManipulation.ObjectManipulator
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Line Interactable: " + transform.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGazeHovered)
        {
            string msg = "GazeHovered " + Time.time + " " + transform.name + " Parent" + transform.parent.name;
            Debug.Log(msg);
            EyeTrackingFileMaker.Instance.appendText(msg);

        }

    
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        // Dynamic is effectively just your normal Update().
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            foreach (var interactor in interactorsHovering)
            {
                if (interactor is FuzzyGazeInteractor fuzzygaze)
                {
                    //log info
                    string msg = "ProcessInteractable Fuzzy " + Time.time + " " + transform.name + " Parent" + transform.parent.name;
                    Debug.Log(msg);
                    EyeTrackingFileMaker.Instance.appendText(msg);
                    return;
                }

                if (interactor is GazeInteractor gaze)
                {
                    //log info
                    string msg = "ProcessInteractable gaze " + Time.time + " " + transform.name + " Parent" + transform.parent.name;
                    Debug.Log(msg);
                    EyeTrackingFileMaker.Instance.appendText(msg);
                    return;
                }
            }
        }
    }

    protected override void OnHoverEntered(XRBaseInteractor interactor)
    {
        base.OnHoverEntered(interactor);

        Debug.Log(interactor.name + " " + interactor.GetType());
    }



}
