using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using MixedReality.Toolkit.Input;

public class NoteNodeSphereInteractable : NodeSphereInteractable
{
    private int noteNumber = -1;
    protected override void Awake()
    {
        base.Awake();
        noteNumber = UnityEngine.Random.Range(1, 8);
        Sample_Player.Instance.playNote(noteNumber);
        LogManager.Instance.oe_numNoteNodes++;
    }

    [Obsolete]
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        if (GameManager.Instance.drawnVisible)
        {

            base.OnSelectEntered(interactor);

            if (interactor.GetType().ToString() == "MixedReality.Toolkit.Input.GrabInteractor")
            {

                Sample_Player.Instance.playNote(noteNumber);
                gameObject.GetComponent<AudioReactive>().startTimer();

                /*EyeTrackingFileMaker.Instance.appendText(Time.time.ToString());
                EyeTrackingFileMaker.Instance.appendText(transform.name.ToString());*/
            }
        }
    }

    [Obsolete]
    protected override void OnHoverEntered(XRBaseInteractor interactor)
    {
        if (GameManager.Instance.drawnVisible)
        {
            base.OnHoverEntered(interactor);
            if (!isSelected)
            {
                if (interactor.GetType().ToString() == "MixedReality.Toolkit.Input.GrabInteractor")
                {
                    Sample_Player.Instance.playNote(noteNumber);
                    gameObject.GetComponent<AudioReactive>().startTimer();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {   
        Debug.Log("my  name: " + transform.name);
        Debug.Log(collision.gameObject.transform.parent.parent.name == transform.parent.parent.name);

        if (collision.gameObject.transform.parent.parent.name == transform.parent.parent.name)
        {
            if (collision.gameObject.transform.name.StartsWith("Node"))
            {
                if (InteractionManager.Instance.lhState == InteractionManager.HandState.Sculpting && InteractionManager.Instance.rhState == InteractionManager.HandState.Sculpting) //(InteractionManager.Instance.lhState == InteractionManager.HandState.Sculpting && 
                {
                    InteractionManager.Instance.finishedSculpting = false;
                    InteractionManager.Instance.rhState = InteractionManager.HandState.Idle;
                    InteractionManager.Instance.lhState = InteractionManager.HandState.Idle;
                    Debug.Log("delete line");
                    LogManager.Instance.IncrementNumDelete();
                    Destroy(transform.parent.parent.gameObject);
                }
            }
        }
    }
}
