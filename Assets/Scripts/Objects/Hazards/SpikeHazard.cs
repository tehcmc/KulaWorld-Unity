using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHazard : MonoBehaviour
{

	void Start()
	{

	}
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Ball")) return;

		other.GetComponentInParent<AudioComponent>().PlaySound("burst");
		Debug.Log("pop");

	}
}
