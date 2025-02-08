using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioReactive : MonoBehaviour
{
    private Color offColor;
    private readonly Color onColor = Color.white;
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
            elapsedTime += Time.deltaTime;
            float transitionValue = Mathf.Clamp((1.0f - elapsedTime/2.0f), 0.0f, 0.5f);
            modMaterial(transitionValue);
            yield return null;
        }
    }
    void modMaterial(float transitionValue)
    {
        if(firstTime == true){
            offColor = gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
            firstTime = false;
        }
        Color lerpColor = Color.Lerp(offColor, onColor, transitionValue);
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", lerpColor);
    }
}
