using System;
using UnityEngine;



public class Key : Collectible
{

	// collect x amount of keys to unlock stage exit. use delegate in this class to invoke
	// a check function in the exit class. open exit when required num of keys are collected.
	Player player;
	private void Awake()
	{
		collectibleName = "key";
	}
	private void Start()
	{
		player = FindObjectOfType<Player>();


	}


	protected override void Collect(Collider other)
	{
		player.KeyCount += 1;
		Debug.Log(player.KeyCount);
		base.Collect(other);
	}

	void Update()
	{

	}
}
