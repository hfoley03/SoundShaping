using UnityEngine;
using System.Collections;

public class SendPositionOnUpdate : MonoBehaviour {

	public OSC osc;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	  OscMessage message = new OscMessage();

        message.address = "/sendtest";
        message.values.Add(UnityEngine.Random.Range(0.0f, 1.0f));

        osc.Send(message);

/*        message = new OscMessage();
        message.address = "/UpdateX";
*//*        message.values.Add(transform.position.x);
        osc.Send(message);

        message = new OscMessage();
        message.address = "/UpdateY";
        message.values.Add(transform.position.y);
        osc.Send(message);

        message = new OscMessage();
        message.address = "/UpdateZ";
        message.values.Add(transform.position.z);*//*
        osc.Send(message);*/


    }


}
