using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : Collectible
{
	// collect x amount of fruit over multiple stages, once required number has been reached
	// next stage should be a "special" stage. if player loses this stage, move on to the original next stage


	[SerializeField] List<Mesh> fruitMesh;
	[SerializeField] MeshFilter currentMesh;
	Player player;
	void Start()
	{
		player = FindObjectOfType<Player>();
		if (player && fruitMesh[player.CurrentFruit])
		{
			currentMesh.mesh = fruitMesh[player.CurrentFruit];
			Debug.Log(fruitMesh[player.CurrentFruit]);
		}
	}


		void Update()
	{

	}
}
