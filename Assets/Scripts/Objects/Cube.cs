using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]

public class ObjectSlot
{
	[SerializeField] BaseObject storedObject;
	[SerializeField] Transform objectPosition;

	public BaseObject StoredObject { get => storedObject; set => storedObject = value; }
	public Transform ObjectPosition { get => objectPosition; set => objectPosition = value; }
}


public class Cube : MonoBehaviour
{
	// cube - contain list(dictionary?) of face nodes. face objects can be attached to them. face objects can be items or obstacles
	// material changes based on stage. Have stage class in game manager? should contain stage music as well as cube textures for each stage.



	[SerializeField] bool occupied;

	public bool Occupied { get => occupied; set => occupied = value; }
	MeshRenderer mr;
	Color defaultColor;
	Material mat;
	[SerializeField] GameObject cubeMesh;
	[SerializeField] List<ObjectSlot> slots;



	Stage stageType;


	private void Awake()
	{
		MeshRenderer mr = cubeMesh.GetComponent<MeshRenderer>();
		defaultColor = mr.material.color;
		
	}


	private void OnEnable()
	{

		stageType = GetComponentInParent<Stage>();
		Material stageMaterial = stageType.Details.StageMaterial;
		mat = stageMaterial;

		if (mat != null) cubeMesh.GetComponent<MeshRenderer>().material = mat;


		foreach (var slot in slots)
		{
			if (slot.StoredObject != null)
			{
				Instantiate(slot.StoredObject, slot.ObjectPosition);
			}
		}
	}
	void Start()
	{

	}


	void Update()
	{
		if (mr)
		{
			if (occupied) { mr.material.color = Color.green; }
			else { mr.material.color = defaultColor; }


		}

	}

}

