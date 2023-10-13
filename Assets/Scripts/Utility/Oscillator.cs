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
		float xPos, yPos, zPos;
		NewMethod(out xPos, out yPos, out zPos);

		transform.localPosition = new Vector3(xPos, yPos, zPos);

		void NewMethod(out float xPos, out float yPos, out float zPos)
		{
			xPos = Oscillate(t, xSpeed, xScale);
			yPos = Oscillate(t, ySpeed, yScale);
			zPos = Oscillate(t, zSpeed, zScale);
		}
	}

	float Oscillate(float time, float speed, float scale)
	{
		return Mathf.Cos(time * speed / Mathf.PI) * scale;
	}

}
