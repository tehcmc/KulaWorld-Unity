using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	// to change - move BALL(fake movement) once ball reaches next cube centre, move main object


	// bounce off wall - when collider hits wall, move back by small amount, start fall coroutine, if it lands recentre ball 

	public static event EventHandler<string> collectObject;
	private CustomInput input;

	GameManager gameManager;


	[SerializeField] float rotateSpeed = 1.7f;
	[SerializeField] float moveSpeed = 3f;
	[SerializeField] float turnSpeed = 10f;

	[SerializeField] float jumpHeight = 1f;
	[SerializeField] float jumpSpeed = 3f;
	[SerializeField] int jumpDistance = 2;

	[SerializeField] ParticleSystem landParticle;



	[SerializeField] GameObject turnPoint;
	[SerializeField] GameObject cameraTurn;
	float originalAngle = 0f;
	[SerializeField] GameObject movePoint;

	[SerializeField] Ball ball;


	[SerializeField] GameObject parent;



	// could be made into a list + dictionary of colliders
	[SerializeField] MoveCollider frontCollider;
	[SerializeField] MoveCollider leftCollider;
	[SerializeField] MoveCollider rightCollider;
	[SerializeField] MoveCollider bottomCollider;
	[SerializeField] MoveCollider centreCollider;


	//do not set in editor, used only to see whether values are being set or not
	[SerializeField] bool turning = false;
	[SerializeField] bool moving = false;
	[SerializeField] bool grounded = true;

	Vector3 ballpos;

	AudioComponent audioComp;

	RaycastHit hit;

	bool rayColliding = false;

	public GameObject TurnPoint { get => turnPoint; set => turnPoint = value; }
	public AudioComponent AudioComp { get => audioComp; set => audioComp = value; }

	private void Awake()
	{
		audioComp = GetComponent<AudioComponent>();
		originalAngle = cameraTurn.transform.rotation.eulerAngles.x;
		input = new CustomInput();
		gameManager = GameManager.Instance;
	}
	private void OnEnable()
	{
		input.Enable();
		StartCoroutine(Fall());
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

		//CheckGround();
	}

	private void CheckGround()
	{
		Vector3 rayPos = ball.transform.position;

		int layerMask = 1 << 3;

		if (Physics.Raycast(rayPos, rayPos + TurnPoint.transform.forward, out hit, .2f, layerMask))
		{
			Debug.Log("coll " + hit.collider.gameObject.name);
			rayColliding = true;
			Debug.DrawLine(rayPos, hit.point, Color.green);
		}
		else
		{
			rayColliding = false;
			Debug.DrawLine(rayPos, rayPos + TurnPoint.transform.forward * 0.2f, Color.red);
		}

	}


	private void HandleInput()
	{
		bool turnRightHeld = input.Movement.TurnRight.ReadValue<float>() > 0.1f;
		bool turnLeftHeld = input.Movement.TurnLeft.ReadValue<float>() > 0.1f;


		if (input.Movement.Jump.IsPressed())
		{
			TryJump();
		}
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




		if (Input.GetKeyDown(KeyCode.Q))
		{

			TiltCamera(-30);

		}

	}
	// -- Turning

	bool CanTurn()
	{

		if (!grounded) return false;
		if (moving) return false;
		if (turning) return false;
		return true;
	}

	void Turn(float val)
	{
		if (turning) return;

		StartCoroutine(TurnCamera(0, 0, val));

	}



	IEnumerator TurnCamera(float x, float y, float z)
	{
		turning = true;
		Quaternion oldRotation = TurnPoint.transform.rotation;

		TurnPoint.transform.Rotate(x, z, y);

		var newRotation = TurnPoint.transform.rotation;

		float t = 0f;

		while (t <= 1)
		{
			TurnPoint.transform.rotation = Quaternion.Lerp(oldRotation, newRotation, t * rotateSpeed);
			t += Time.deltaTime * rotateSpeed;
			if (TurnPoint.transform.rotation == newRotation) break;
			yield return new WaitForEndOfFrame();
		}
		turning = false;
		TurnPoint.transform.rotation = newRotation;

		yield return null;
	}

	// -- Moving
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

				StartCoroutine(RotateUp(TurnPoint.transform.right, -90));


				return;
			}

			if (!bottomCollider.IsColliding)
			{
				if (leftCollider.IsColliding || rightCollider.IsColliding)
				{

					return;
				}

				StartCoroutine(RotateAround(TurnPoint.transform.right, 90));
				return;

			}

			StartCoroutine(Move(TurnPoint.transform.forward, 1f));
		}
		else
		{
			ball.RollBall(TurnPoint.transform.right);
		}

	}

	public void MoveInDirection(Vector3 dir, int distance)
	{
		moving = false;
		StartCoroutine(Move(dir, distance));
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




		while (t < 1)
		{
			ball.RollBall(TurnPoint.transform.right);

			movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * moveSpeed);
			t += gameManager.gameTime;

			if (movePoint.transform.position == newPosition) { break; }
			yield return new WaitForEndOfFrame();
		}

		moving = false;

		transform.position = RoundVector(movePoint.transform.position);

		movePoint.transform.localPosition = ballpos;

		if (centreCollider.IsColliding) { transform.parent = centreCollider.currentCube.transform; }
		else { Debug.Log("fall"); grounded = false; StartCoroutine(Fall()); }

		yield break;

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
			ball.RollBall(TurnPoint.transform.right);
			movePoint.transform.rotation = Quaternion.Lerp(oldRotation, newRotation, t * moveSpeed);
			t += gameManager.gameTime;

			if (movePoint.transform.rotation == newRotation) break;
			yield return new WaitForEndOfFrame();
		}
		turning = false;

		movePoint.transform.rotation = newRotation;
		movePoint.transform.localPosition = ballpos;
		yield break;
	}

	IEnumerator RotateUp(Vector3 axis, float angle)
	{
		//transform up + 1 transform forward + 1 rotate -90	


		turning = true;
		Quaternion oldRotation = movePoint.transform.rotation;

		Vector3 oldPosition = movePoint.transform.position;

		Vector3 addPos = TurnPoint.transform.forward + TurnPoint.transform.up;
		Vector3 newPosition = movePoint.transform.position + addPos;

		movePoint.transform.RotateAround(oldPosition, axis, angle);
		var newRotation = movePoint.transform.rotation;

		float t = 0f;
		while (t <= 1)
		{
			ball.RollBall(TurnPoint.transform.right);
			movePoint.transform.SetPositionAndRotation(Vector3.Lerp(oldPosition, newPosition, t * moveSpeed), Quaternion.Lerp(oldRotation, newRotation, t * moveSpeed));
			t += gameManager.gameTime;
			if (movePoint.transform.rotation == newRotation && movePoint.transform.position == newPosition) break;
			yield return new WaitForEndOfFrame();
		}

		turning = false;
		transform.position = RoundVector(movePoint.transform.position);
		movePoint.transform.localPosition = ballpos;
		movePoint.transform.rotation = newRotation;

		yield break;

	}


	// -- Jumping

	void TryJump()
	{
		if (!grounded) return;
		if (turning) return;
		if (ball.IsColliding) return;
		if (!centreCollider.IsColliding) return;

		//	transform.parent = null;
		if (moving || Input.GetKey(KeyCode.W))
		{
			if (frontCollider.IsColliding)
			{
				StartCoroutine(JumpForwardsTwo());
			}
			else
			{
				StartCoroutine(JumpForwards());
			}

		}
		else
		{

			//StartCoroutine(Jump());
		}


	}

	IEnumerator Jump()
	{
		if (!grounded) yield break;
		grounded = false;
		Vector3 oldPosition = movePoint.transform.position;

		Vector3 newPosition = movePoint.transform.up * jumpHeight;


		Vector3 heightVector = movePoint.transform.up;
		Vector3 jumpVec;

		float t = 0f;

		while (t <= 1)
		{

			float jHeight = jumpHeight * Mathf.Sin(Mathf.PI * t * moveSpeed);

			jHeight = Mathf.Clamp(jHeight, 0f, jumpHeight);
			jumpVec = new Vector3(heightVector.x, heightVector.y, heightVector.z) * jHeight;
			//	movePoint.transform.position = jumpVec;
			t += gameManager.gameTime;

			if (movePoint.transform.position == newPosition) { break; }
			yield return new WaitForEndOfFrame();
		}
		StartCoroutine(Fall());



		yield break;

	}

	IEnumerator JumpForwardsTwo()
	{
		grounded = false;

		CharacterController ctr;


		Vector3 oldPosition = movePoint.transform.position;

		float distFromOrigin = Vector3.Distance(transform.position, oldPosition); // get distance from centre point of the cube. old pos is set when space is pressed
																				  // so could be different from transform.pos

		Vector3 newPosition = movePoint.transform.position += TurnPoint.transform.forward * (jumpDistance / 2.5f - distFromOrigin);


		Vector3 heightVector = movePoint.transform.up;

		Vector3 upVec;


		float t = 0f;






		while (t <= 1)
		{
			ball.RollBall(TurnPoint.transform.right);



			if (ball.IsColliding)
			{
				audioComp.PlaySound("bounce");
				break;
			}




			float jHeight = jumpHeight * Mathf.Sin(Mathf.PI * t * moveSpeed);

			jHeight = Mathf.Clamp(jHeight, 0f, jumpHeight / 2.5f);
			upVec = new Vector3(heightVector.x, heightVector.y, heightVector.z) * jHeight;

			movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * moveSpeed);//move forwards 

			movePoint.transform.position += upVec;

			if (movePoint.transform.position == newPosition) { break; }

			t += gameManager.gameTime;
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

		Vector3 newPosition = movePoint.transform.position += TurnPoint.transform.forward * (jumpDistance - distFromOrigin);


		Vector3 heightVector = movePoint.transform.up;

		Vector3 upVec;


		float t = 0f;






		while (t <= 1)
		{
			ball.RollBall(TurnPoint.transform.right);



			if (ball.IsColliding)
			{
				audioComp.PlaySound("bounce");
				break;
			}




			float jHeight = jumpHeight * Mathf.Sin(Mathf.PI * t * moveSpeed);

			jHeight = Mathf.Clamp(jHeight, 0f, jumpHeight);
			upVec = new Vector3(heightVector.x, heightVector.y, heightVector.z) * jHeight;

			movePoint.transform.position = Vector3.Lerp(oldPosition, newPosition, t * moveSpeed);//move forwards 

			movePoint.transform.position += upVec;

			if (movePoint.transform.position == newPosition) { break; }

			t += gameManager.gameTime;
			yield return new WaitForEndOfFrame();

		}


		StartCoroutine(Fall());
		yield break;
	}


	IEnumerator LerpToPos(Vector3 oldPos, Vector3 newPos)
	{
		float t = 0;
		while (t <= 1)
		{

			movePoint.transform.position = Vector3.Lerp(oldPos, newPos, t * moveSpeed);
			t += gameManager.gameTime;

			if (movePoint.transform.position == newPos) { break; }
			yield return new WaitForEndOfFrame();
		}

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
			t += gameManager.gameTime;

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
				audioComp.PlaySound("bounce");
				yield break;
			}

			yield return new WaitForEndOfFrame();
		}







		yield return StartCoroutine(Fall());

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


}
