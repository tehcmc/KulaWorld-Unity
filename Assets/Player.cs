using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Player : MonoBehaviour
{
	// Start is called before the first frame update	


	[SerializeField] float rotateSpeed = 1.7f;
	[SerializeField] float moveSpeed = 3f;

	[SerializeField] GameObject leftColl;
	[SerializeField] GameObject rightColl;
	[SerializeField] GameObject frontColl;
	bool turning = false;
	bool moving = false;
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.A) && CanTurn())
		{
			StartCoroutine(RotateCamera(-90));
		}
		if (Input.GetKey(KeyCode.D) && CanTurn())
		{
			StartCoroutine(RotateCamera(90));
		}
		if (Input.GetKey(KeyCode.W) && CanMove())
		{

			StartCoroutine(Move());
		}

	}

	bool CanTurn()
	{
		if (moving) return false;
		if (turning) return false;
		return true;
	}
	bool CanMove()
	{
		if (moving) return false;
		if (turning) return false;
		return true;
	}
	IEnumerator RotateCamera(float val)
	{
		if (turning) { Debug.Log("turning"); yield return null; }

		turning = true;
		Debug.Log(turning);
		Quaternion oldRotation = transform.rotation;

		transform.Rotate(0, val, 0);
		var newRotation = transform.rotation;

		float t = 0f;

		while (t <= 1)
		{
			transform.rotation = Quaternion.Lerp(oldRotation, newRotation, t * rotateSpeed);
			t += Time.deltaTime * rotateSpeed;
			yield return new WaitForEndOfFrame();
		}
		turning = false;
		Debug.Log(turning);

		yield return null;
	}

	IEnumerator Move()
	{
		if (moving) yield return null;
		moving = true;

		Vector3 oldPosition = transform.position;

		Vector3 newPosition = transform.position += transform.forward;

		float t = 0f;
		while (t <= 1)
		{
			transform.position = Vector3.Lerp(oldPosition, newPosition, t);
			t += Time.deltaTime * moveSpeed;
			yield return new WaitForEndOfFrame();


		}
		moving = false;
		yield return null;
	}
}