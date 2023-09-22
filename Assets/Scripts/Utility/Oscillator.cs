using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Oscillator : MonoBehaviour
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

		float xPos = Oscillate(t, xSpeed, xScale);
		float yPos = Oscillate(t, ySpeed, yScale);
		float zPos = Oscillate(t, zSpeed, zScale);

		transform.localPosition = new Vector3(xPos, yPos, zPos);
	}

	float Oscillate(float time, float speed, float scale)
	{
		return Mathf.Cos(time * speed / Mathf.PI) * scale;
	}

}
