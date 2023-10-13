using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
	[SerializeField] string stageType = "Egypt";
	[SerializeField] StageDetails details = new StageDetails();
	public string StageType { get => stageType; set => stageType = value; }
	public StageDetails Details { get => details; set => details = value; }

	private void Awake()
	{
		Details = GameManager.Instance.Stages.GetStageDetails(stageType);	
	}

}
