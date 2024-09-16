using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestText : MonoBehaviour
{
    public TMP_Text myText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myText.text = "global: " + InteractionManager.Instance.getCurrentState().ToString() + "      r_hand:  " + InteractionManager.Instance.rhState.ToString()
             + "      l_hand:  " + InteractionManager.Instance.lhState.ToString();
    }
}
