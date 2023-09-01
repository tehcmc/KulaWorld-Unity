using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveCollider : MonoBehaviour
{
	public bool IsColliding { get; private set; } = false;
	[SerializeField] bool showColourIndicator = true;
	[SerializeField] MeshRenderer debugIndicator;

	public Cube currentCube { get; private set; }
	// Start is called before the first frame update
	void Start()
	{

	}

	private void OnTriggerStay(Collider other)
	{
		if (other.tag != "Cube") return;
		if (other == currentCube) return;

		//	Debug.Log(name + " start colliding with " + other.gameObject.transform.parent.name);
		IsColliding = true;
		currentCube = other.GetComponentInParent<Cube>();
		if (currentCube) currentCube.Occupied = true;

	}
	private void OnTriggerExit(Collider other)
	{
		//	if (other != currentCube) return;
		//	Debug.Log(name + " stopped colliding with " + other.gameObject.transform.parent.name);
		IsColliding = false;

		if (currentCube) currentCube.Occupied = false;


		currentCube = null;

	}

	// Update is called once per frame
	void Update()
	{
		if (showColourIndicator)
		{
			if (IsColliding)
			{
				debugIndicator.material.color = Color.red;
			}
			else
			{
				debugIndicator.material.color = Color.green;
			}
		}

	}
}
