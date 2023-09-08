using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
	[SerializeField] int scoreValue = 100;
	[SerializeField] ParticleSystem pickupParticle;
	void Start()
	{

	}

	void Update()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Ball") return;

		Destroy(gameObject);
	}
}
