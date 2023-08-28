using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
	// to change - move BALL(fake movement) once ball reaches next cube centre, move main object



	private CustomInput input;




	[SerializeField] float rotateSpeed = 1.7f;
	[SerializeField] float moveSpeed = 3f;
	[SerializeField] float turnSpeed = 10f;

	[SerializeField] float jumpHeight = 1f;
	[SerializeField] float jumpSpeed = 3f;
	[SerializeField] int jumpDistance = 2;
	[SerializeField] ParticleSystem landParticle;



	[SerializeField] GameObject turnPoint;
	[SerializeField] GameObject movePoint;

	[SerializeField] GameObject ball;


	[SerializeField] GameObject parent;

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

	private void Awake()
	{
		input = new CustomInput();
	}
	private void OnEnable()
	{
		input.Enable();
	}
	private void OnDisable()
	{

		input.Disable();
	}
	void Start()
	{
		ballpos = movePoint.transform.localPosition;
	}

	// Update is called once per frame
	void Update()
	{
		HandleInput();

	}

	private void HandleInput()
	{
		bool turnRightHeld = input.Movement.TurnRight.ReadValue<float>() > 0.1f;
		bool turnLeftHeld = input.Movement.TurnLeft.ReadValue<float>() > 0.1f;
		if (turnLeftHeld && CanTurn())
		{
			Turn(-90);
		}
		if (turnRightHeld && CanTurn())
		{
			Turn(90);
		}

		if (CanMove())
		{
			TryMove();
		}


		if (Input.GetKeyDown(KeyCode.Space))
		{
			TryJump();
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
		bool isMoveHeld = input.Movement.Move.ReadValue<float>() > 0.1f;

		if (!isMoveHeld) return false;
		if (!grounded) return false;
		if (moving) return false;
		if (turning) return false;
		return true;
	}
	void TryMove()
	{
		if (grounded)
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
		}

		StartCoroutine(Move(turnPoint.transform.forward));
	}

	IEnumerator Move(Vector3 direction)
	{
		if (moving) yield break;
		moving = true;






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

		if (centreCollider.IsColliding)
		{

			transform.parent = centreCollider.currentCube;
			//ball.transform.position = Round(centreCollider.currentCube.position);
		}

		yield break;

	}

	private void RollBall(float t)
	{
		float rand = Random.Range(0, 1);
		ball.transform.RotateAround(turnPoint.transform.right, t / (turnSpeed + rand));
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
			movePoint.transform.SetPositionAndRotation(Vector3.Lerp(oldPosition, newPosition, t * moveSpeed), Quaternion.Lerp(oldRotation, newRotation, t * moveSpeed));
			t += Time.deltaTime;
			if (movePoint.transform.rotation == newRotation && movePoint.transform.position == newPosition) break;
			yield return new WaitForEndOfFrame();
		}

		turning = false;
		movePoint.transform.rotation = newRotation;
		yield break;

	}


	void TryJump()
	{
		if (!grounded) return;
		if (turning) return;
		grounded = false;

		if (moving || Input.GetKey(KeyCode.W))
		{
			StartCoroutine(JumpForwards());
		}
		else
		{
			StartCoroutine(JumpForwards());
		}


	}

	IEnumerator Jump()
	{




		Vector3 oldPosition = movePoint.transform.position;

		Vector3 newPosition = movePoint.transform.position += movePoint.transform.up * jumpHeight;

		float t = 0f;



		while (t <= 1)
		{
			movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * moveSpeed);
			t += Time.deltaTime;

			if (movePoint.transform.position == newPosition) { break; }
			yield return new WaitForEndOfFrame();
		}
		StartCoroutine(falltwo());

		transform.position = RoundVector(movePoint.transform.position);
		movePoint.transform.localPosition = ballpos;




		yield break;
	}


	IEnumerator JumpForwards()
	{
		Vector3 oldPosition = movePoint.transform.position;
		var distFromOrigin = Vector3.Distance(transform.position, oldPosition);

		Vector3 newHeight = movePoint.transform.position += movePoint.transform.up * jumpHeight;



		Debug.Log(distFromOrigin);

		Debug.Log("jump dist norm" + turnPoint.transform.forward * jumpDistance);

		Debug.Log("jump dist fixed" + turnPoint.transform.forward * (jumpDistance - distFromOrigin));

		Vector3 newPosition = movePoint.transform.position += turnPoint.transform.forward * (jumpDistance - distFromOrigin);



		float t = 0f;
		Vector3 testo = movePoint.transform.position;
		Vector3 test2 = movePoint.transform.position;
		while (t <= 1)
		{
			if (test2 != newHeight)
			{
				testo = Vector3.Lerp(oldPosition, newHeight * 2, (t) * jumpSpeed);
				test2 = testo;

				Debug.Log("Test2 " + test2 + " newheight " + newHeight);
				Debug.Log("up");

			}
			else
			{
				testo = Vector3.Lerp(newHeight, movePoint.transform.up, t * jumpSpeed);
				Debug.Log("down");
			}


			movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * moveSpeed);


			testo.x = movePoint.transform.position.x; testo.z = movePoint.transform.position.z;

			movePoint.transform.position = testo;
			//Debug.Log("Transform Pos" + movePoint.transform.position + "Added Pos" + testo + "New Pos " + newPosition);

			if (movePoint.transform.position == newPosition)
			{
				Debug.Log("land");
				break;
			}

			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		StartCoroutine(falltwo());


		transform.position = RoundVector(movePoint.transform.position);
		movePoint.transform.localPosition = ballpos;

		//StartCoroutine(Move(turnPoint.transform.forward * (float)jumpDistance));




		yield break;
	}

	IEnumerator falltwo()
	{

		while (!centreCollider.IsColliding)
		{
			if (centreCollider.IsColliding) break;

			Vector3 oldPosition = movePoint.transform.position;

			Vector3 newPosition = movePoint.transform.position -= movePoint.transform.up * jumpHeight;

			float t = 0f;
			while (t <= 1)
			{


				movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * jumpSpeed);
				if (movePoint.transform.position == newPosition) break;
				t += Time.deltaTime;


				yield return new WaitForEndOfFrame();
			}

			transform.position = RoundVector(movePoint.transform.position);
			movePoint.transform.localPosition = ballpos;
		}

		Instantiate(landParticle, ball.transform.position, Quaternion.identity); grounded = true;
		grounded = true;


		yield break;

	}
	void Fall()
	{
		if (grounded) return;
		Debug.Log("fall");

		StartCoroutine(Move(-movePoint.transform.up * jumpHeight));

		if (centreCollider.IsColliding)
		{
			grounded = true; Debug.Log("land");
			return;
		}

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
