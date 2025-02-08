using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{

    private static LineManager _instance;
    public static LineManager Instance
    {
        get 
        {
            if (_instance == null)
                Debug.LogError("Line Manager is NUll!");

            return _instance;
        }
    }

    private int counter = 0;
    private bool sculpting = false;
    private bool manipulating = false;

    public enum InteractionState
    { 
        Idle,
        Drawing, 
        Sculpting,
        Manipulating
    }

    private InteractionState currentState;
    

    private void Awake()
    {
        _instance = this;
        currentState = InteractionState.Idle;
        Debug.Log("hi from the line manager");    
    }

    public InteractionState getCurrentState()
    {
        return currentState;
    }

    public void setCurrentState(InteractionState state)
    {
        currentState = state;
    }
}
