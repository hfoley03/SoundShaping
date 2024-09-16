using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - testing NumberedBalls thingy

public class TestNumberedBalls : MonoBehaviour
{
	[Header( "Where to put spawned balls.")]
	public Transform EmitPoint;

	[Header( "Our prototype ball and stage.")]
	public NumberedBall NBPrototype;

	IEnumerator Start ()
	{
		for (int i = 0; i < 20; i++)
		{
			GameObject ball = null;
			NumberedBall stage = null;

			NBPrototype.Clone( out ball, out stage);
			ball.transform.position = EmitPoint.position;

			//ball.transform.localScale = Vector3.one * Random.Range( 0.5f, 2.5f);

			var rb = ball.gameObject.AddComponent<Rigidbody>();
			rb.angularVelocity = Random.onUnitSphere * Random.Range( -1.0f, 1.0f);
			rb.velocity = new Vector3( Random.Range(-1.0f, 1.0f),
				0,
				Random.Range( -1.0f, 1.0f));

			//			string label = ((char)Random.Range( 'A', 'Z' + 1)).ToString();
			string label = "1";

			stage.BallText.text = label;

			ball.transform.SetParent(transform);
			stage.transform.SetParent(transform);


			yield return new WaitForSeconds( 1.0f);
		}	
	}
}
