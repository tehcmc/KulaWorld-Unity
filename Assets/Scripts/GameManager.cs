using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StageList))]
public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public float gameTime;

	public float timeRate = 1;

	[SerializeField] bool changeFPS = false;
	[SerializeField][Range(1, 145)] int targetFrames = 60;

	StageList stages;

	public StageList Stages { get => stages; protected set => stages = value; }

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(this);
			return;
		}
		else
		{
			Instance = this;
		}

		Application.targetFrameRate = targetFrames;

		Stages = GetComponent<StageList>();


	}


	void Start()
	{

	}


	void Update()
	{
		gameTime = Time.deltaTime / timeRate;

		if (changeFPS)
		{
			Application.targetFrameRate = targetFrames;
		}
	}
}
