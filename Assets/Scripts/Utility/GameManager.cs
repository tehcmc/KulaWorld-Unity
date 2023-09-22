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

	AudioSource audio;

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

		audio = GetComponent<AudioSource>();
		Stages = GetComponent<StageList>();


	}
	private void OnEnable()
	{


	}
	private void Start()
	{

		var music = Stages.GetStage("Egypt").StageMusic;
		//if (music) audio.PlayOneShot(music);
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
