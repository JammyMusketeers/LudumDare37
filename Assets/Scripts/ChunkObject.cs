using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkObject : MonoBehaviour
{
	public string objectName;
	public int spawnChance = 50;

	protected Chunk _chunk;

	public void SetChunk(Chunk chunk)
	{
		_chunk = chunk;
	}

	public virtual void Create()
	{
		
	}
}
