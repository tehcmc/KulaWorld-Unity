using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotationOscillator : MonoBehaviour
{



	[SerializeField] float xSpeed = 1f;
	[SerializeField] float xScale = 1f;


	[SerializeField] float ySpeed = 1f;
	[SerializeField] float yScale = 0;

	[SerializeField] float zSpeed = 1f;
	[SerializeField] float zScale = 0;

	float t = 0f;
	// Start is called before the first frame update
	void Start()
	{
		//StartCoroutine(Move());

	}

	// Update is called once per frame
	void Update()
	{
		Move();
	}

	private void Move()
	{
		t += GameManager.Instance.gameTime;

		float xRot = Oscillate(t, xSpeed, xScale);
		float yRot = Oscillate(t, ySpeed, yScale);
		float zRot = Oscillate(t, zSpeed, zScale);

		transform.localEulerAngles = new Vector3(xRot, yRot, zRot);
	}

	float Oscillate(float time, float speed, float scale)
	{
		return Mathf.Cos(time * speed / Mathf.PI) * scale;
	}

}
