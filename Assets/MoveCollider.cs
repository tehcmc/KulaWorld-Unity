using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCollider : MonoBehaviour
{
	public bool IsColliding { get; private set; } = false;
	// Start is called before the first frame update
	void Start()
	{

	}


	private void OnCollisionEnter(Collision collision)
	{
		IsColliding = true;
	}
	private void OnCollisionExit(Collision collision)
	{
		IsColliding = false;
	}
	// Update is called once per frame
	void Update()
	{

	}
}
