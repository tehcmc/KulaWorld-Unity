using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{

	[SerializeField] bool rotateX = false;
	[SerializeField] bool rotateY = false;
	[SerializeField] bool rotateZ = false;


	[SerializeField] float duration = 1f;

	// Start is called before the first frame update
	void Start()
	{

	}
	private void OnEnable()
	{
		StartCoroutine(RotateObject());

	}
	// Update is called once per frame
	void Update()
	{

	}

	void Move()
	{

	}
	void Rotate()
	{

	}

	IEnumerator RotateObject()
	{
		float startRotation = 0f;
		float endRotation = startRotation + 360f;
		float t = 0.0f;

		float xVal = 0;
		float yVal = 0;
		float zVal = 0;

		if (duration <= 0) yield return StartCoroutine(RotateObject());

		while (t < duration)
		{
			t += GameManager.Instance.gameTime;






			float lerpRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360;

			if (rotateX) xVal = lerpRotation;
			if (rotateY) yVal = lerpRotation;
			if (rotateZ) zVal = lerpRotation;
			//	float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;

			transform.localEulerAngles = new Vector3(xVal, yVal, zVal);
			yield return null;
		}
		yield return StartCoroutine(RotateObject());
	}
}
