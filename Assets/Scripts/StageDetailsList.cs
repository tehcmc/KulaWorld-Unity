using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class StageDetailsList : MonoBehaviour
{
	IDictionary<string, StageDetails> detailsDictionary = new Dictionary<string, StageDetails>();

	[SerializeField] List<StageDetails> detailsList = new List<StageDetails>();



	private void Awake()
	{

		foreach (var details in detailsList)
		{
			detailsDictionary.Add(details.StageName, details);
			Debug.Log(details.StageName);

		}
		
	}


	public StageDetails GetStageDetails(string name)
	{
		if (!detailsDictionary.ContainsKey(name)) { Debug.Log("invalid"); return null; }
		Debug.Log("valid");
		return detailsDictionary[name];
	}
	public Material GetStageMaterial(string name)
	{
		if (!detailsDictionary.ContainsKey(name)) return null;
		return detailsDictionary[name].StageMaterial;
	}
}
