using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// @kurtdekker - both an "Engine" and a runner.
//
// How to use:
//	-	Set one up stage and it will be dead because of the Actual flag
//	-	for how to set up, study what's in the demo scene. It's all needed.
//	-	Use it to call Clone() and you'll be given a nice fresh "live" one

public class NumberedBall : MonoBehaviour
{
	bool Actual;

	[Header( "Everything here will be cloned:")]

	[Header( "Make one on disk.")]
	public RenderTexture renderTexture;

	[Header( "Put one in scene, connect the RT above")]
	public Camera StageCamera;

	[Header( "In your world space canvas")]
	public Image BallImage;
	public Text BallText;

	[Header( "Sphere set up with unlit material with RT link")]
	public GameObject BallGeometry;

	// produces a sequences of "Far aaway" positions
	// if you prefer, use layers to keep everybody
	// else from seeing these stages.
	static Vector3 CursorPosition = new Vector3( 500, 0, 0);
	static Vector3 GetNextPosition()
	{
		var result = CursorPosition;
		CursorPosition += Vector3.up * 5.0f;
		return result;
	}

	public void Clone( out GameObject ball, out NumberedBall stage)
	{
		// make a full copy, we need the stage, etc.
		stage = Instantiate<NumberedBall>( this);

		// we are one of the 'live" ones
		stage.gameObject.SetActive( true);
		stage.Actual = true;

		// rip the ball off so it floats free
		ball = stage.BallGeometry;
		ball.transform.SetParent( null);

		stage.transform.position = GetNextPosition();

		var renderer = ball.GetComponent<Renderer>();
		// clone the material
		var material = new Material(renderer.material);
		// clone the RT
		var rtCopy = Instantiate<RenderTexture>( renderTexture);
		material.mainTexture = rtCopy;
		renderer.material = material;

		stage.StageCamera.targetTexture = rtCopy;


	}

	void Start()
	{
		if (!Actual)
		{
			gameObject.SetActive(false);
		}
	}
}
