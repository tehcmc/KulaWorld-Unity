using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	// Start is called before the first frame update	
	[SerializeField] Transform RotatePoint;
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			RotatePoint.rotation = new Quaternion(RotatePoint.rotation.x, RotatePoint.rotation.y + 90, RotatePoint.rotation.z, 0);
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			RotatePoint.rotation = new Quaternion(RotatePoint.rotation.x, RotatePoint.rotation.y - 90, RotatePoint.rotation.z, 0);
		}


	}
}
