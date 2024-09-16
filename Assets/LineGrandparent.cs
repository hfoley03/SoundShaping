using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGrandparent : MonoBehaviour
{

    public int grandParentNumber = -1;
    public bool TMT_enabled = true;
    private float duration = 0.5f;
    private float timer = 0f;

    private bool tickTimer = true;

    public bool pinch = true;
    public bool setup = false;

    void Start()
    {
        if (GameManager.Instance.gameMode == GameManager.GameMode.TMT)
        {
            gameObject.layer = 3;
        }
        if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
        {
            gameObject.layer = 6;
        }
        //Debug.Log("Hello from Grandad + " + grandParentNumber + " layer " + gameObject.layer);
    }

    // Update is called once per frame
    void Update()  //change this to a co routine and remove update
    {
        if (setup)
        {
            if (tickTimer)
            {
                timer += Time.deltaTime;
                if (timer >= duration)
                {
                    timer = 0f;
                    tickTimer = false;
                    if (GameManager.Instance.gameMode == GameManager.GameMode.OpenEnded)
                    {
                        CheckChildren();
                    }
                    //setupChildrenForSuccess();
                }
            }
        }
    }

    void CheckChildren()
    {
        Debug.Log("check kids");
        for (int i = 0; i < transform.childCount; i++)
        {
            //Debug.Log(transform.GetChild(i).childCount);

            if (transform.GetChild(0).childCount < 2)
            {
                Destroy(gameObject);
            }
            else 
            {
                Transform parent = transform.GetChild(i);

                for (int j = 0; j < parent.childCount; j++)
                {
                    NodeSphereInteractable comp;
                    if (parent.GetChild(j).TryGetComponent<NodeSphereInteractable>(out comp))
                    {
                        comp.linesSetUp();
                    }
                }
            }


        }
        setup = false;
    }

    public void setupChildrenForSuccess() {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform parent = transform.GetChild(i);
            Debug.Log(parent.name);
            Debug.Log(parent.childCount);
            for(int j = 0; j < parent.childCount; j++)
            {
                NodeSphereInteractable comp;
                if(parent.GetChild(j).TryGetComponent<NodeSphereInteractable>(out comp))
                {
                    Debug.Log("YESSSS");
                    comp.linesSetUp();
                }
            }
        }
    }


    private void nodeSetups()
    {

    }
}
