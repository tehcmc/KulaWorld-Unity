using System.Collections;
using System.Collections.Generic;
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
		}


	}


	public Material GetStageMaterial(string name)
	{
		if (!stageDictionary.ContainsKey(name)) return null;
		Debug.Log(stageDictionary[name].StageMaterial.name);
		return stageDictionary[name].StageMaterial;
	}
}
