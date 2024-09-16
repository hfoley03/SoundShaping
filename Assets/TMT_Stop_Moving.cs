using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMT_Stop_Moving : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
