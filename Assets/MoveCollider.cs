using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveCollider : MonoBehaviour
{
	public bool IsColliding { get; private set; } = false;
	[SerializeField] MeshRenderer debugIndicator;

	public Transform currentCube { get; private set; }
	// Start is called before the first frame update
	void Start()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		IsColliding = true;
		currentCube = other.transform;

	}
	private void OnTriggerExit(Collider other)
	{
		IsColliding = false;
		currentCube = null;

	}

	// Update is called once per frame
	void Update()
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
