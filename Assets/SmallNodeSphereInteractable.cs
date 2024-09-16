using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;


public class SmallNodeSphereInteractable : NodeSphereInteractable
{
    [Obsolete]
    protected override void OnHoverEntered(XRBaseInteractor interactor)
    {
        base.OnHoverEntered(interactor);
        if (interactor.GetType().ToString() == "MixedReality.Toolkit.Input.GrabInteractor")
        {
            transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        }
    }
    [Obsolete]
    protected override void OnHoverExited(XRBaseInteractor interactor)
    {
        base.OnHoverExited(interactor);
        if (interactor.GetType().ToString() == "MixedReality.Toolkit.Input.GrabInteractor")
        {
            transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }
}
