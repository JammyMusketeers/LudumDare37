using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkLootSpawner : ChunkObject
{
	[Serializable]
	public struct SpawnerItem
	{
		public RarityType rarityType;
		public ResourceType resourceType;
	}

	public Transform[] spawnPoints;
	public SpawnerItem[] spawnItems;

	public void AddLoot()
	{
		if (spawnItems.Length == 0)
		{
			return;
		}

		var chunkSize = GameManager.Instance.chunkSize;
		var lootItemPrefabs = GameManager.Instance.lootItemPrefabs;
		var spawnPointIndexes = new List<int>();

		for (int i = 0; i < spawnPoints.Length; i++)
		{
			spawnPointIndexes.Add(i);
		}

		for (int i = 0; i < spawnItems.Length; i++)
		{
			var spawnItem = spawnItems[i];

			if (spawnPointIndexes.Count > 0)
			{
				var randomIndex = UnityEngine.Random.Range(0, spawnPointIndexes.Count);
				var spawnPointIndex = spawnPointIndexes[randomIndex];
				var spawnPoint = spawnPoints[spawnPointIndex];
				var candidates = new List<LootItem>();

				foreach (var item in GameManager.Instance.lootItemPrefabs)
				{
					if ((spawnItem.rarityType == RarityType.ANY || item.rarityType == spawnItem.rarityType)
						&& (spawnItem.resourceType == ResourceType.ANY || item.resourceType == spawnItem.resourceType))
					{
						candidates.Add(item);
					}
				}

				if (candidates.Count > 0)
				{
					randomIndex = UnityEngine.Random.Range(0, candidates.Count);

					var randomLoot = candidates[randomIndex];
					var lootObject = GameObject.Instantiate(randomLoot);

					lootObject.transform.position = spawnPoint.transform.position;

					_chunk.AddChild(lootObject.gameObject);

					spawnPointIndexes.Remove(spawnPointIndex);
				}
			}
		}
	}

	public override void Create()
	{
		AddLoot();
	}
}
