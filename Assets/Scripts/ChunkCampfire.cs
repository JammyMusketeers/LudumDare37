using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkCampfire : ChunkObject
{
	public GameObject fireObject;
	public ResourceType resourceTypeRequired;

	public override void Create()
	{
		fireObject.SetActive(false);
	}
}
