using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StageDetails
{
	[SerializeField] Material stageMaterial;
	[SerializeField] Material stageSkybox;

	[SerializeField] AudioClip stageMusic;
	[SerializeField] string stageName;

	public Material StageMaterial { get => stageMaterial; protected set => stageMaterial = value; }
	public Material StageSkybox { get => stageSkybox; protected set => stageSkybox = value; }
	public AudioClip StageMusic { get => stageMusic; protected set => stageMusic = value; }

	public string StageName { get => stageName; protected set => stageName = value; }
}
