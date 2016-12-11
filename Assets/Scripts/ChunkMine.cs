using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkMine : ChunkObject
{
	public GameObject explosionFX;
	public float damage = 40f;
	

	public void HitPlayer(Player player)
	{
		Instantiate(explosionFX, transform.position, Quaternion.identity);
		player.Hit(damage);
		Destroy(gameObject);
	}
}

