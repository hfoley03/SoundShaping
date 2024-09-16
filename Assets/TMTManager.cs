using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMTManager : MonoBehaviour
{
	private static TMTManager _instance;
	public static TMTManager Instance
	{
		get
		{
			if (_instance == null)
				Debug.LogError("TMTManager  is NUll!");

			return _instance;
		}
	}

	public GameObject TMTNode;
    private int TMTNodeCounter = 1;
	public Vector3 origin;

	private Vector3[] vectors;
	private GameObject[] TMT_Nodes;

	private string[] namesTestA;
	private string[] namesTestB;


	private int lastCorrectNode = 1;
	private int nextCorrectNode = 1;
	private int lastNodeHit = 0;

	private int currentStreak = 0;

	public int testAorTestB = 1;
	public int layout = 0;

	private SphereCollider sphereColliderToDisable;

	private int noteSelect = 1;
	private int incrementDecrement = +1;

	public int numBalls = 8;


	[Header("Where to put spawned balls.")]
	public Transform EmitPoint;

	[Header("Our prototype ball and stage.")]
	public NumberedBall NBPrototype;

	private void Awake()
	{
		_instance = this;
	}

	public void StartTMTA()
	{
		LogManager.Instance.tmt_numCorrectHitsStreak = 0;
		namesTestA = new string[10]{"1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
		namesTestB = new string[10] { "1", "A", "2", "B", "3", "C", "4", "D", "5", "E" };


		//origin = Camera.main.transform.position + new Vector3(0.0f, 0.0f, 1.0f);
		origin = Camera.main.transform.position + Camera.main.transform.forward * 0.7f;

		lastCorrectNode = 1;
		nextCorrectNode = 1;
		lastNodeHit = 0;
		noteSelect = 1;
		currentStreak = 0;
		LogManager.Instance.ResetForTmt();


		/*if (GameManager.Instance.whichTMT == 0)
        {
            vectors = new Vector3[25];
            TMT_Nodes = new GameObject[25];
            vectors[0] = new Vector3(4f, -6f, 0.0f);
            vectors[1] = new Vector3(8f, -4f, 0.0f);
            vectors[2] = new Vector3(11f, 10f, 0.0f);
            vectors[3] = new Vector3(14f, 1f, 0.0f);
            vectors[4] = new Vector3(14f, 15f, 0.0f);
            vectors[5] = new Vector3(6f, 15f, 0.0f);
            vectors[6] = new Vector3(-12f, 10f, 0.0f);
            vectors[7] = new Vector3(-6f, 8f, 0.0f);
            vectors[8] = new Vector3(-3f, 14f, 0.0f);
            vectors[9] = new Vector3(2f, 10f, 0.0f);
            vectors[10] = new Vector3(-15f, 14f, 0.0f);
            vectors[11] = new Vector3(-7f, 0f, 0.0f);
            vectors[12] = new Vector3(-11f, 0f, 0.0f);
            vectors[13] = new Vector3(-2f, -2f, 0.0f);
            vectors[14] = new Vector3(-16f, 6f, 0.0f);
            vectors[15] = new Vector3(6f, 6f, 0.0f);
            vectors[16] = new Vector3(8f, 2f, 0.0f);
            vectors[17] = new Vector3(2f, 1f, 0.0f);
            vectors[18] = new Vector3(12f, -8f, 0.0f);
            vectors[19] = new Vector3(-8f, -6f, 0.0f);
            vectors[20] = new Vector3(8f, -14f, 0.0f);
            vectors[21] = new Vector3(4f, -14f, 0.0f);
            vectors[22] = new Vector3(-13f, -6f, 0.0f);
            vectors[23] = new Vector3(-3f, -12f, 0.0f);
            vectors[24] = new Vector3(-12f, -14f, 0.0f);
            numBalls = 25;
        }*/

		/*		if(true)//if (GameManager.Instance.whichTMT == 1)
                {
                    TMT_Nodes = new GameObject[8];
                    vectors = new Vector3[8];
                    vectors[0] = new Vector3(4f, -6f, 0.0f);
                    vectors[1] = new Vector3(6f, 6f, 0.0f);
                    vectors[2] = new Vector3(11f, 10f, 0.0f);
                    vectors[3] = new Vector3(-3f, 14f, 0.0f);
                    vectors[4] = new Vector3(-11f, 0f, 0.0f);
                    vectors[5] = new Vector3(8f, -4f, 0.0f);
                    vectors[6] = new Vector3(8f, -14f, 0.0f);
                    vectors[7] = new Vector3(-3f, -12f, 0.0f);
                    numBalls = 8;

                }*/

		if (layout == 0)
        {
            vectors = new Vector3[10];
            TMT_Nodes = new GameObject[10];
            vectors[0] = new Vector3(4f, -6f, 0.0f);
            vectors[1] = new Vector3(14f, 1f, 0.0f);
            vectors[2] = new Vector3(6f, 15f, 0.0f);
            vectors[3] = new Vector3(14f, 15f, 0.0f);
            vectors[4] = new Vector3(-2f, -2f, 0.0f);
            vectors[5] = new Vector3(-6f, 8f, 0.0f);
            vectors[6] = new Vector3(-16f, 6f, 0.0f);
            vectors[7] = new Vector3(-13f, -6f, 0.0f);
            vectors[8] = new Vector3(-3f, -12f, 0.0f);
            vectors[9] = new Vector3(12f, -8f, 0.0f);
            numBalls = 10;
        }

        else if (layout == 1)
        {
            vectors = new Vector3[10];
            TMT_Nodes = new GameObject[10];
            vectors[0] = new Vector3(-12f, -14f, 0.0f);
            vectors[1] = new Vector3(-7f, 0f, 0.0f);
            vectors[2] = new Vector3(-3f, 14f, 0.0f);
            vectors[3] = new Vector3(14f, 15f, 0.0f);
            vectors[4] = new Vector3(4f, -6f, 0.0f);
            vectors[5] = new Vector3(-3f, -12f, 0.0f);
            vectors[6] = new Vector3(-12f, 10f, 0.0f);
            vectors[7] = new Vector3(6f, 15f, 0.0f);
            vectors[8] = new Vector3(14f, 1f, 0.0f);
            vectors[9] = new Vector3(8f, -14f, 0.0f);
            numBalls = 10;
        }

        else if (layout == 2)
        {
            vectors = new Vector3[10];
            TMT_Nodes = new GameObject[10];
            vectors[0] = new Vector3(-12f, -14f, 0.0f);
            vectors[1] = new Vector3(-7f, 0f, 0.0f);
            vectors[2] = new Vector3(-3f, 14f, 0.0f);
            vectors[3] = new Vector3(14f, 15f, 0.0f);
            vectors[4] = new Vector3(4f, -6f, 0.0f);
            vectors[5] = new Vector3(-3f, -12f, 0.0f);
            vectors[6] = new Vector3(-12f, 10f, 0.0f);
            vectors[7] = new Vector3(6f, 15f, 0.0f);
            vectors[8] = new Vector3(14f, 1f, 0.0f);
            vectors[9] = new Vector3(8f, -14f, 0.0f);
            numBalls = 10;
        }

       else if (layout == 3)
        {
            vectors = new Vector3[10];
            TMT_Nodes = new GameObject[10];
            vectors[0] = new Vector3(8f, -4f, 0.0f);
			vectors[1] = new Vector3(4f, -14f, 0.0f);
			vectors[2] = new Vector3(6f, 6f, 0.0f);
			vectors[3] = new Vector3(14f, 15f, 0.0f);
			vectors[4] = new Vector3(6f, 15f, 0.0f);
			vectors[5] = new Vector3(-7f, 0f, 0.0f);
			vectors[6] = new Vector3(-3f, 14f, 0.0f);
			vectors[7] = new Vector3(-15f, 14f, 0.0f);
			vectors[8] = new Vector3(-13f, -6f, 0.0f);
			vectors[9] = new Vector3(-3f, -12f, 0.0f);
			numBalls = 10;
        }



        for (int i = 0; i < numBalls; i++) //25
		{

			GameObject _TMTNode = Instantiate(TMTNode);
			//NODE AT START OF EACH LINE
			float scaleVal = 3.5f;
			_TMTNode.transform.position = (vectors[i] / 16f) * 0.2f;
			_TMTNode.transform.position = new Vector3(_TMTNode.transform.position.x * (3.0f/scaleVal), _TMTNode.transform.position.y * (2.0f/scaleVal), _TMTNode.transform.position.z); ;
			_TMTNode.transform.position = _TMTNode.transform.position + origin;
			_TMTNode.transform.rotation = Camera.main.transform.rotation;
			// EmitPoint.position;

			//Debug.Log(_TMTNode.transform.position);

			/*			if (testAorTestB == 0)
						{
							_TMTNode.name = namesTestA[i];
						}
						else 
						{
							_TMTNode.name = namesTestB[i];
						}*/

			_TMTNode.name = namesTestA[i];


			_TMTNode.transform.SetParent(transform);
			_TMTNode.GetComponent<tmtNode2>().nodeNumber = i + 1;
            _TMTNode.GetComponent<AudioSource>().clip = MusicManager.Instance.audioClips[noteSelect];
            noteSelecter();
           // Debug.Log(_TMTNode.transform.position);
			_TMTNode.transform.position = RoundVector(_TMTNode.transform.position, 2);
			//Debug.Log(_TMTNode.transform.position);

			TMT_Nodes[i] = _TMTNode;
		}



	}


	public void registerNodeHit(int nodeNumber)
    {
		//Debug.Log(nodeNumber);

		if (lastNodeHit != nodeNumber)
		{
			lastNodeHit = nodeNumber;

			if (nodeNumber == nextCorrectNode)
			{
				//Debug.Log("Correct Hit " + nodeNumber);
				TMT_Nodes[nodeNumber - 1].GetComponent<AudioSource>().Play();

				nextCorrectNode++;
				lastCorrectNode = nodeNumber;
				currentStreak++;
				UpdateStreak();

				if (lastCorrectNode == 10) {

					Debug.Log("TMT complete");
					LogManager.Instance.tmtStartStop = false;
					LogManager.Instance.MakeLogMessages();
					currentStreak = 0;

				}

				sphereColliderToDisable = TMT_Nodes[nodeNumber - 1].GetComponent<SphereCollider>();
				//TMT_Nodes[nodeNumber - 1].GetComponentInChildren<MeshRenderer>().material.color = new Color(0, 255, 0);
				TMT_Nodes[nodeNumber - 1].GetComponent<tmtNode2>().setNodeColor(1);
				if (nodeNumber - 2 > 0)
				{
					TMT_Nodes[nodeNumber - 2].GetComponent<SphereCollider>().enabled = false;
					//Debug.Log("disabling : " + TMT_Nodes[nodeNumber - 2].GetComponent<tmtNode2>().nodeNumber);
				}
			}
			else
			{
				nextCorrectNode = lastCorrectNode;
				LogManager.Instance.tmt_numIncorrectHits++;
				UpdateStreak();
				currentStreak = 0;

			}
		}
		else {
			if (TMT_Nodes[nodeNumber - 2].GetComponent<SphereCollider>().enabled == true)
            {
				LogManager.Instance.tmt_numIncorrectHits++;
			}


		}
    }

	private void UpdateStreak() {
		if (currentStreak > LogManager.Instance.tmt_numCorrectHitsStreak)
		{
			LogManager.Instance.tmt_numCorrectHitsStreak = currentStreak;
		}
	}



	public void noteSelecter()
	{
		if(noteSelect >= 8)
        {
			incrementDecrement = -1;
        }
		if(noteSelect <=1)
        {
			incrementDecrement = +1;
        }
		noteSelect = noteSelect + incrementDecrement;
	}

	public void tearDownTMT()
    {
		foreach (Transform child in this.transform)
		{
			Destroy(child.gameObject);
			EXTOSCManager.Instance.appendText(child.gameObject.name + " deleted");
		}
				
	}

	public Vector3 RoundVector(Vector3 vec, int decPlaces)
    {
		float mult = Mathf.Pow(10, decPlaces);
		vec.x = Mathf.Round(vec.x *mult) / mult;
		vec.y = Mathf.Round(vec.y * mult) / mult;
		vec.z = Mathf.Round(vec.z * mult) / mult;
		return vec;
	}

	public void PractitionerOverride(int nodenumber)
    {
		lastCorrectNode = nodenumber - 1;
		nextCorrectNode = nodenumber;
		lastNodeHit = nodenumber - 1;

		registerNodeHit(nodenumber);

		string logmsg = "practitioner override for tmt on number " + nodenumber;

		LogManager.Instance.AppendText(logmsg);
    }

}


