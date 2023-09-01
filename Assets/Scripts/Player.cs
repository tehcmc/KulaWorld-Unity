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

	[SerializeField] Vector3 height;

	[SerializeField] GameObject turnPoint;
	[SerializeField] GameObject cameraTurn;
	float originalAngle = 0f;
	[SerializeField] GameObject movePoint;

	[SerializeField] MoveCollider ball;


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
		originalAngle = cameraTurn.transform.rotation.eulerAngles.x;
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
		if (centreCollider.currentCube) centreCollider.currentCube.Occupied = true;
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


		if (input.Movement.Jump.IsPressed())
		{
			TryJump();
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{

			TiltCamera(-30);

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

			StartCoroutine(Move(turnPoint.transform.forward, 1f));
		}
		else
		{
			RollBall(1);
		}

	}

	IEnumerator Move(Vector3 direction, float distance)
	{
		if (moving) yield break;
		moving = true;




		Vector3 oldPosition = movePoint.transform.position;

		Vector3 newPosition = movePoint.transform.position += direction * distance;

		float t = 0f;

		if (centreCollider.currentCube && Vector3.Distance(centreCollider.currentCube.transform.position, movePoint.transform.position) != 0)
		{
			//	moving = false;
			//	yield return StartCoroutine(RecentreBall());
		}




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

		if (centreCollider.IsColliding) transform.parent = centreCollider.currentCube.transform;

		yield break;

	}

	IEnumerator RecentreBall()
	{

		if (moving) yield break;
		moving = true;


		Debug.Log("Recentre");

		Vector3 oldPosition = movePoint.transform.position;

		Vector3 newPosition = centreCollider.currentCube.transform.position;

		float t = 0f;

		while (t <= 1)
		{
			RollBall(1);

			movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * moveSpeed);
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		moving = false;

		transform.position = RoundVector(movePoint.transform.position);

		movePoint.transform.localPosition = ballpos;

		if (centreCollider.IsColliding) transform.parent = centreCollider.currentCube.transform;

		yield break;

	}







	private void RollBall(float t)
	{
		float rand = Random.Range(0, 1);
		ball.gameObject.transform.RotateAround(turnPoint.transform.right, t / (turnSpeed + rand));
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

		transform.parent = null;
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
		if (!grounded) yield break;
		grounded = false;
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
		StartCoroutine(Fall());



		yield break;

	}

	//f(t) = -h*t*(t-1)
	IEnumerator JumpForwards()
	{
		grounded = false;

		CharacterController ctr;


		Vector3 oldPosition = movePoint.transform.position;

		float distFromOrigin = Vector3.Distance(transform.position, oldPosition); // get distance from centre point of the cube. old pos is set when space is pressed
																				  // so could be different from transform.pos

		Vector3 newPosition = movePoint.transform.position += turnPoint.transform.forward * (jumpDistance - distFromOrigin);


		Vector3 heightVector = movePoint.transform.up;

		Vector3 upVec;


		float t = 0f;


		while (t <= 1)
		{
			RollBall(1);
			if (ball.IsColliding)
			{
				//break;
			}

			float jHeight = jumpHeight * Mathf.Sin(Mathf.PI * t * moveSpeed);

			jHeight = Mathf.Clamp(jHeight, 0f, jumpHeight);
			upVec = new Vector3(heightVector.x, heightVector.y, heightVector.z) * jHeight;

			height = upVec;
			movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * moveSpeed);//move forwards 

			movePoint.transform.position += upVec;

			if (movePoint.transform.position == newPosition) { break; }

			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();

		}


		StartCoroutine(Fall());
		yield break;
	}

	IEnumerator Fall()
	{
		if (grounded) yield break;
		//tilt camera down, return to original pos when landing
		if (centreCollider.IsColliding)
		{
			Instantiate(landParticle, ball.transform.position, Quaternion.identity);
			transform.position = RoundVector(movePoint.transform.position);
			movePoint.transform.localPosition = ballpos;
			grounded = true;
			yield break;
		}


		Vector3 oldPosition = movePoint.transform.position;

		Vector3 newPosition = movePoint.transform.position -= movePoint.transform.up * jumpHeight;

		float t = 0f;

		while (t <= 1)
		{


			movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * jumpSpeed);

			if (movePoint.transform.position == newPosition) break;
			t += Time.deltaTime;

			if (centreCollider.IsColliding)
			{



				Instantiate(landParticle, ball.transform.position, Quaternion.identity);
				transform.position = RoundVector(movePoint.transform.position);
				movePoint.transform.localPosition = ballpos;
				grounded = true;
				transform.parent = centreCollider.currentCube.transform;
				if (centreCollider.currentCube && Mathf.Abs(Vector3.Distance(centreCollider.currentCube.transform.position, movePoint.transform.position)) > 0)
				{

					//yield return StartCoroutine(RecentreBall());
				}

				yield break;
			}

			yield return new WaitForEndOfFrame();
		}







		yield return StartCoroutine(Fall());

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


	void TiltCamera(float angle)
	{
		var newRotation = Quaternion.Euler(angle, 0, 0);
		cameraTurn.transform.rotation = newRotation;
	}

	Vector3 CalculateJumpArc(Vector3 forward, Vector3 up)
	{
		// pass in up pos

		// pass in fwd pos

		//create holding variable, up + fwd

		//multiply up by holding. up = 0,1,0 thus x*0 and z*0 = 0, only mults up/y


		Vector3 added = forward + up;

		added.y = Mathf.Sin(jumpSpeed * Time.time) * jumpDistance;

		var calculatedCurve = Vector3.zero;

		calculatedCurve = Vector3.Scale(up, added);
		return calculatedCurve;
	}



}
