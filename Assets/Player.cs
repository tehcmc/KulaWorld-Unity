using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Player : MonoBehaviour
{
	// to change - move BALL(fake movement) once ball reaches next cube centre, move main object








	[SerializeField] float rotateSpeed = 1.7f;
	[SerializeField] float moveSpeed = 3f;
	[SerializeField] float turnSpeed = 10f;

	[SerializeField] GameObject turnPoint;
	[SerializeField] GameObject movePoint;

	[SerializeField] GameObject ball;



	[SerializeField] MoveCollider frontCollider;
	[SerializeField] MoveCollider leftCollider;
	[SerializeField] MoveCollider rightCollider;
	[SerializeField] MoveCollider bottomCollider;
	[SerializeField] MoveCollider centreCollider;

	[SerializeField] bool turning = false;
	[SerializeField] bool moving = false;
	[SerializeField] bool grounded = true;
	bool jumping = false;
	Vector3 ballpos;

	void Start()
	{
		ballpos = movePoint.transform.localPosition;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.A) && CanTurn())
		{
			Turn(-90);
		}
		if (Input.GetKey(KeyCode.D) && CanTurn())
		{
			Turn(90);
		}

		if (CanMove())
		{
			TryMove();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Jump();
		}


		if (!grounded)
		{
			Fall();
		}
	}

	bool CanTurn()
	{
		if (!grounded) return false;
		if (moving) return false;
		if (turning) return false;
		return true;
	}
	bool CanMove()
	{
		if (!Input.GetKey(KeyCode.W)) return false;
		if (!grounded) return false;
		if (moving) return false;
		if (turning) return false;
		return true;
	}
	void TryMove()
	{
		if (frontCollider.IsColliding)
		{
			//transform up + 1 transform forward + 1 rotate -90	

			StartCoroutine(RotateUp(turnPoint.transform.right, -90));


			return;
		}

		if (!bottomCollider.IsColliding)
		{
			if (leftCollider.IsColliding || rightCollider.IsColliding)
			{

				return;
			}

			StartCoroutine(RotateAround(turnPoint.transform.right, 90));
			return;

		}
		StartCoroutine(Move(turnPoint.transform.forward));
	}

	IEnumerator Move(Vector3 direction)
	{
		if (moving) yield break;
		moving = true;
		if (centreCollider.IsColliding)
		{


			//ball.transform.position = Round(centreCollider.currentCube.position);
		}






		Vector3 oldPosition = movePoint.transform.position;

		Vector3 newPosition = movePoint.transform.position += direction;

		float t = 0f;



		while (t <= 1)
		{
			RollBall(1);

			movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * moveSpeed);
			t += Time.deltaTime;

			if (movePoint.transform.position == newPosition) { break; }
			yield return new WaitForEndOfFrame();
		}

		moving = false;
		transform.position = RoundVector(movePoint.transform.position);

		movePoint.transform.localPosition = ballpos;
		yield break;

	}

	private void RollBall(float t)
	{
		ball.transform.RotateAround(turnPoint.transform.right, t / turnSpeed);
	}

	void Turn(float val)
	{
		if (turning) return;

		StartCoroutine(Rotate(0, 0, val));

	}



	IEnumerator Rotate(float x, float y, float z)
	{
		turning = true;
		Quaternion oldRotation = turnPoint.transform.rotation;

		turnPoint.transform.Rotate(x, z, y);

		var newRotation = turnPoint.transform.rotation;

		float t = 0f;

		while (t <= 1)
		{
			turnPoint.transform.rotation = Quaternion.Lerp(oldRotation, newRotation, t * rotateSpeed);
			t += Time.deltaTime * rotateSpeed;
			if (turnPoint.transform.rotation == newRotation) break;
			yield return new WaitForEndOfFrame();
		}
		turning = false;
		turnPoint.transform.rotation = newRotation;

		yield return null;
	}


	IEnumerator RotateAround(Vector3 axis, float angle)
	{

		turning = true;
		Quaternion oldRotation = movePoint.transform.rotation;
		var pos = movePoint.transform.position;




		movePoint.transform.RotateAround(pos, axis, angle);
		var newRotation = movePoint.transform.rotation;

		float t = 0f;

		while (t <= 1)
		{
			RollBall(1);
			movePoint.transform.rotation = Quaternion.Lerp(oldRotation, newRotation, t * moveSpeed);
			t += Time.deltaTime;

			if (movePoint.transform.rotation == newRotation) break;
			yield return new WaitForEndOfFrame();
		}
		turning = false;

		movePoint.transform.rotation = newRotation;
		yield return null;
	}

	IEnumerator RotateUp(Vector3 axis, float angle)
	{
		//transform up + 1 transform forward + 1 rotate -90	


		turning = true;
		Quaternion oldRotation = movePoint.transform.rotation;

		Vector3 oldPosition = movePoint.transform.position;

		Vector3 addPos = turnPoint.transform.forward + turnPoint.transform.up;
		Vector3 newPosition = movePoint.transform.position + addPos;






		movePoint.transform.RotateAround(oldPosition, axis, angle);
		var newRotation = movePoint.transform.rotation;

		float t = 0f;

		while (t <= 1)
		{
			RollBall(1);
			movePoint.transform.rotation = Quaternion.Lerp(oldRotation, newRotation, t * moveSpeed);
			movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * moveSpeed);

			t += Time.deltaTime;

			if (movePoint.transform.rotation == newRotation && movePoint.transform.position == newPosition) break;


			yield return new WaitForEndOfFrame();
		}
		Debug.Log($"Fin: {movePoint.transform.rotation}");
		turning = false;
		movePoint.transform.rotation = newRotation;
		Debug.Log($"ROUNDED: {movePoint.transform.rotation}");
		yield break;

	}


	void Jump()
	{
		if (!grounded) return;



		if (moving || Input.GetKey(KeyCode.W))
		{

			StartCoroutine(Move(transform.forward * 2));
		}
		else
		{
			StartCoroutine(Move(transform.up));
		}

		grounded = false;
	}

	void Fall()
	{
		if (grounded) return;


		StartCoroutine(Move(-transform.up));
		if (centreCollider.IsColliding) grounded = true; return;

	}


	void SnapMove()
	{
		transform.position += transform.forward;
	}

	float RoundTo(float value, float multipleOf)
	{
		return Mathf.Round(value / multipleOf) * multipleOf;
	}
	Vector3 RoundVector(Vector3 value)
	{
		if (value == Vector3.zero) return value;

		return new Vector3(Mathf.Round(value.x), MathF.Round(value.y), MathF.Round(value.z));
	}

	Quaternion RoundQuat(Quaternion value, float multipleOf)
	{

		return Quaternion.Euler(SnapRotation((int)value.x), SnapRotation((int)value.y), SnapRotation((int)value.z));
	}


	public float SnapRotation(int rotation)
	{
		// Calculate the remainder when dividing the rotation by 360
		int remainder = rotation % 360;

		// Snap the remainder to the nearest 90 degree step
		int snappedRotation = (int)(Math.Round((double)remainder / 90) * 90);

		// Handle negative values
		if (snappedRotation < -270)
			snappedRotation += 360;

		return (float)snappedRotation;
	}
}
