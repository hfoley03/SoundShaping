using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioReactive : MonoBehaviour
{
    public Material noteNodeMaterialOff;
    public Material noteNodeMaterialOn;

    private Color offColor;
    private Color onColor;
    private bool firstTime = true;




    private Coroutine timerCoroutine;

    public void startTimer() {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(TimerCoroutine());
        
    }

    private IEnumerator TimerCoroutine()
    {
        float duration = 2.0f;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration) {
            //Debug.Log("timer: " + elapsedTime);
            elapsedTime += Time.deltaTime;
            float colourTrans = Mathf.Clamp((1.0f - elapsedTime/2.0f), 0.0f, 0.5f);
            modMaterial(colourTrans);
            yield return null;
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        onColor = Color.white;
    }



    void modMaterial(float swell_)
    {
        if(firstTime == true)
        {
            offColor = gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
            firstTime = false;
        }
        Color lerpColor = Color.Lerp(offColor, onColor, swell_);
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", lerpColor);

    }



}
