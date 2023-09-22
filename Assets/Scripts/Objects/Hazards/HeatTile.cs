using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatTile : MonoBehaviour
{
	private void Awake()
	{

	}
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Ball")) return;
		other.GetComponentInParent<Player>().IsHeating = true;

	}
	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("Ball")) return;

		other.GetComponentInParent<Player>().IsHeating = false;

	}
}
