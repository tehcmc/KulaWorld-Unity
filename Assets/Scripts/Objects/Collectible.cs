using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CollectibleArgs
{
	public CollectibleArgs(string name) { Name = name; }
	public string Name { get; }
}

public class Collectible : MonoBehaviour
{
	public delegate void CollectEventHandler(object sender, CollectibleArgs args);
	public static event CollectEventHandler OnCollectEvent;



	[SerializeField] protected string collectibleName = "coin";
	[SerializeField] protected int scoreValue = 100;
	[SerializeField] protected ParticleSystem pickupParticle;
	[SerializeField] protected string sound;


	bool collected = false;
	private void Awake()
	{
		collectibleName = collectibleName.ToLower();
	}



	void Start()
	{

	}

	void Update()
	{

	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (collected) return;
		if (other.tag != "Ball") return;
		Collect(other);
	}

	protected virtual void Collect(Collider other)
	{
		if (!other) return;
		collected = true;
		OnCollectEvent?.Invoke(this, new CollectibleArgs(collectibleName));
		other.GetComponentInParent<AudioComponent>().PlaySound(sound);
		Destroy(gameObject);
	}
}
