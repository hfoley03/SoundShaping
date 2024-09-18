using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapSolver : MonoBehaviour
{
    public int thisNodeId = 0;
    private SmallNodeSphereInteractable smallScript;
    private NoteNodeSphereInteractable noteScript;
    public Component scriptToDoWorkOn;
    public float overlapSize = 0.0001f;
    public bool noteNodeType = false;
    // Start is called before the first frame update
    void Start()
    {
        if(transform.TryGetComponent<SmallNodeSphereInteractable>(out smallScript))
        {
            scriptToDoWorkOn = smallScript;
            overlapSize = 0.005f;
            noteNodeType = false;
            thisNodeId = GameManager.Instance.smallNodeID;
            GameManager.Instance.smallNodeID++;

        }
        else if(transform.TryGetComponent<NoteNodeSphereInteractable>(out noteScript))
        {
            scriptToDoWorkOn = noteScript;
            overlapSize = 0.015f;
            noteNodeType = true;
            thisNodeId = GameManager.Instance.noteNodeID;
            GameManager.Instance.noteNodeID++;
        }

    }

    void Update()
    {
        if (InteractionManager.Instance.getCurrentState() != InteractionManager.InteractionState.Drawing)
        {
            int numTouching = 0;
            Collider[] touchedNodes = new Collider[10];
            // check if transform overlaps with anything
            Collider[] colliders = Physics.OverlapSphere(transform.position, overlapSize, 1 << 6);
            if (colliders.Length > 1)
            {
                foreach (Collider col in colliders)
                {
                    // check if any overlapping objects are Nodes
                    if (col.tag == "Node")
                    {
                        touchedNodes[numTouching] = col;
                        numTouching++;
                    }
                }
            }

            // true if less than two hands are sculpting
            if (InteractionManager.Instance.allowOveralSolver)
            {
                // true if this is a note node
                if (noteNodeType)
                {
                    if (numTouching > 1)
                    {
                        foreach (Collider tCol in touchedNodes)
                        {
                            // true if the node touching this node has a lower ID number
                            if (tCol.GetComponent<OverlapSolver>().thisNodeId < thisNodeId)
                            {
                                // if our node's interactable script is enabled then disable it
                                if (transform.GetComponent<NoteNodeSphereInteractable>().enabled == true)
                                {
                                    transform.GetComponent<NoteNodeSphereInteractable>().enabled = false;
                                }
                            }
                            // else if we have the lower ID make sure we are enabled
                            else if (tCol.GetComponent<OverlapSolver>().thisNodeId > thisNodeId)
                            {
                                transform.GetComponent<NoteNodeSphereInteractable>().enabled = true;
                            }
                        }
                    }
                    else
                    {
                        // no nodes are touching us, make sure we are enabled, we might have been disabled
                        transform.GetComponent<NoteNodeSphereInteractable>().enabled = true;
                    }
                }
                else // else we are a small node 
                {
                    if (numTouching > 1)
                    {
                        foreach (Collider tCol in touchedNodes)
                        {
                            Debug.Log(tCol.GetComponent<OverlapSolver>().thisNodeId);
                            if (tCol.GetComponent<OverlapSolver>().thisNodeId < thisNodeId)
                            {
                                if (transform.GetComponent<SmallNodeSphereInteractable>().enabled == true)
                                {
                                    transform.GetComponent<SmallNodeSphereInteractable>().enabled = false;

                                }
                            }
                            else if (tCol.GetComponent<OverlapSolver>().thisNodeId > thisNodeId)
                            {
                                transform.GetComponent<SmallNodeSphereInteractable>().enabled = true;
                            }
                        }
                    }
                    else
                    {
                        transform.GetComponent<SmallNodeSphereInteractable>().enabled = true;
                    }
                }
            }

            // else both hands are sculpting, so possible delete situation
            else
            {
                if (numTouching > 1)
                {
                    foreach (Collider tCol in touchedNodes)
                    {
                        // true if we are not considering ourself
                        if (tCol.transform.name != transform.name)
                        {
                            // true if the Nodes belong to the same line grandparent 
                            if (tCol.gameObject.transform.parent.parent.name == transform.parent.parent.name)
                            {
                                Debug.Log("delete!!!");
                                InteractionManager.Instance.finishedSculpting = false;
                                InteractionManager.Instance.rhState = InteractionManager.InteractionState.Idle;
                                InteractionManager.Instance.lhState = InteractionManager.InteractionState.Idle;
                                LogManager.Instance.IncrementNumDelete();
                                Destroy(transform.parent.parent.gameObject);
                            }
                        }
                    }
                }

            }
        }
    }
}
