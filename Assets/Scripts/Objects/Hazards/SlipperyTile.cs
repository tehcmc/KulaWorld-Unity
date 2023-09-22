using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyTile : BaseObject
{
	// when player reaches centre of this tile, move forward in player's facing direction by x amount.

	[SerializeField] int moveDistaance = 1;



	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Ball")) return;

		Debug.Log("slip!");
		var playerMovement = other.GetComponentInParent<PlayerMovement>();

		if (!playerMovement) return;
		Debug.Log("slip CALL");

		playerMovement.MoveInDirection(playerMovement.TurnPoint.transform.forward, moveDistaance);
	}

}
