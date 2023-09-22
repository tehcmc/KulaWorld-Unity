using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolourMaterial : MonoBehaviour
{
	[SerializeField] Color colour = Color.white;
	Material material;

	public Color Colour { get => colour; set => colour = value; }

	void Start()
	{
		Recolour(colour);
	}

	public void Recolour(Color color)
	{
		material = GetComponent<MeshRenderer>().material;
		material.color = color;
	}

	void Update()
	{

	}
}
