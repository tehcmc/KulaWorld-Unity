using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MoveCollider
{

	GameManager gameManager;

	[SerializeField] float turnSpeed = 20f;



	[SerializeField] GameObject ballModel;

	[SerializeField] Color defaultColor;

	[SerializeField] Color heatColor; 



	[SerializeField] public MoveCollider bottomCollider { get; protected set; }
	public Color HeatColor { get => heatColor; set => heatColor = value; }
	public Color DefaultColor { get => defaultColor; set => defaultColor = value; }

	public GameObject BallModel { get => ballModel; set => ballModel = value; }

	private void Awake()
	{
		gameManager = GameManager.Instance;
		if (!BallModel) Debug.LogError("No model");
	}
	void Start()
	{

	}


	void Update()
	{

	}

	public void RollBall(Vector3 dir)
	{

		BallModel.gameObject.transform.RotateAround(dir, gameManager.gameTime * turnSpeed);
	}

}
