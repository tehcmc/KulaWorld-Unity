using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class StageList : MonoBehaviour
{
	IDictionary<string, StageDetails> stageDictionary = new Dictionary<string, StageDetails>();

	[SerializeField] List<StageDetails> stages = new List<StageDetails>();



	private void Awake()
	{

		foreach (var stage in stages)
		{
			stageDictionary.Add(stage.StageName, stage);
			Debug.Log(stage.StageName);

		}
		foreach (var stage in stageDictionary)
		{

		}

	}


	public StageDetails GetStage(string stageName)
	{
		if (!stageDictionary.ContainsKey(stageName)) { Debug.Log("invalid"); return null; }
		Debug.Log("valid");
		return stageDictionary[stageName];
	}
	public Material GetStageMaterial(string name)
	{
		if (!stageDictionary.ContainsKey(name)) return null;
		return stageDictionary[name].StageMaterial;
	}
}
