using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TNT_Node :MonoBehaviour
{ 
    public GameObject ball;
    public TextMeshPro text;
    public GameObject cylinder;
    public GameObject transperentNode;
    public int nodeNumber = -1;

    public Material grey, green;
    private Material currentColour;

    private GameObject _ball;
    private TextMeshPro _text;
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.transform.position = new Vector3(0f, 1.6f, 0.2f);
        _ball = Instantiate(ball);
       // GameObject _transNode = Instantiate(transperentNode);
        TextMeshPro _text = Instantiate(text);

        meshRenderer = _ball.GetComponent<MeshRenderer>();

        ///GameObject _cylinder = Instantiate(cylinder);
        /*SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = _ball.transform.localScale.x * 1.5f;
        sphereCollider.center = gameObject.transform.position;
        sphereCollider.transform.SetParent(transform);*/



        //_transNode.transform.position = gameObject.transform.position;
        //_transNode.transform.SetParent(transform);
        



        _ball.transform.SetParent(transform);
        _text.transform.SetParent(transform);
       // _cylinder.transform.SetParent(transform);

        _ball.transform.position = gameObject.transform.position;
        _text.transform.position = gameObject.transform.position + new Vector3(0f, 0f, -0.0025f);
        //_cylinder.transform.position = gameObject.transform.position + new Vector3(0f, 0f, +0.02f);

        // _text.GetComponent<TextMeshProUGUI>().SetText("3");
        //_text.GetComponent<TextMeshPro>().SetText("2");
        _text.SetText(gameObject.name);

        Debug.Log("hi i am tmt node _ " + nodeNumber);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.collider is SphereCollider)
        Debug.Log(collision.collider.transform.name);
        if (collision.collider.transform.name == "TMT_Collider_Node(Clone)")

            {
                //Debug.Log("collison with mesh collider of the line, node: " + nodeNumber);
                transform.parent.GetComponent<TMTManager>().registerNodeHit(nodeNumber);
        }
        else
        {
            Debug.Log("unexpected collision at tmt node " + nodeNumber);
        }
    }

    public void setNodeColor(int i)
    {
        if(i == 0)
        {
            meshRenderer.material = grey;
        }
        if(i == 1)
        {
            meshRenderer.material = green;
        }

    }

}
