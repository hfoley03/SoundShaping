using UnityEngine;
using System.Collections;

public class ReceivePosition : MonoBehaviour {
    
   	public OSC osc;


	// Use this for initialization
	void Start () {
       osc.SetAddressHandler("/test", OnReceiveTest);

    }
	
	// Update is called once per frame
	void Update () {
	
	}

/*	void OnReceiveXYZ(OscMessage message){
		float x = message.GetInt(0);
         float y = message.GetInt(1);
		float z = message.GetInt(2);

		transform.position = new Vector3(x,y,z);
	}*/

    void OnReceiveTest(OscMessage message) {
        float x = message.GetFloat(0);
		transform.GetComponent<AudioSource>().volume = x;

		transform.GetComponent<AudioSource>().Play();

		Debug.Log(x);
    }



}
