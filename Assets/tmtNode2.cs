using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class tmtNode2 : MonoBehaviour
{
    public GameObject ball;
    private GameObject _ball;
    private MeshRenderer meshRenderer;
    public TextMeshPro text;
    public int nodeNumber = -1;

    void Start()
    {
        _ball = Instantiate(ball);
        meshRenderer = _ball.GetComponent<MeshRenderer>();
        _ball.transform.SetParent(transform);
        _ball.transform.position = gameObject.transform.position;
        TextMeshPro _text = Instantiate(text);
        _text.transform.SetParent(transform);
        _text.transform.position = gameObject.transform.position + new Vector3(0f, 0f, -0.025f);
        _text.SetText(gameObject.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.name == "TMT_Collider_Node(Clone)")
        {
            transform.parent.GetComponent<TMTManager>().registerNodeHit(nodeNumber);
        }
    }

    public void setNodeColor(int i)
    {
        if (i == 0)
        {
            meshRenderer.material.color = Color.grey;
        }
        if (i == 1)
        {
            meshRenderer.material.color = Color.green;
        }
    }
}
