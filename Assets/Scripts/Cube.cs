using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
	// Start is called before the first frame update
	[SerializeField] bool occupied;

	public bool Occupied { get => occupied; set => occupied = value; }
	MeshRenderer mr;
	Color defaultColor;
	private void Awake()
	{
		MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
		defaultColor = mr.material.color;
	}
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (mr)
		{
			if (occupied) { mr.material.color = Color.green; }
			else { mr.material.color = defaultColor; }


		}

	}

}

