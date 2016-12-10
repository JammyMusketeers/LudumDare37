using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk
{
	public int x;
	public int z;

	private Ground _ground;

	private List<GameObject> _children;

	private float _clearTime;

	public float GetClearTime()
	{
		return _clearTime;
	}

	public void SetClearTime(float clearTime)
	{
		_clearTime = Time.time + clearTime;
	}

	public void AddGround()
	{
		_ground = GameObject.Instantiate(GameManager.Instance.groundPrefab);
		_ground.SetSize(GameManager.Instance.chunkSize);
		_ground.SetPosition(x, z);
	}

	public void AddRail()
	{
		var chunkSize = GameManager.Instance.chunkSize;
		var railSection = GameObject.Instantiate(GameManager.Instance.railPrefab);

		railSection.transform.position = new Vector3(x * chunkSize, 0f, z * chunkSize);

		AddChild(railSection);
	}

	public Ground GetGround()
	{
		return _ground;
	}

	public void AddChild(GameObject child)
	{
		_children.Add(child);
	}

	public void AddObjects()
	{
		var chunkSize = GameManager.Instance.chunkSize;
		var minObjectSpawn = GameManager.Instance.minObjectSpawn;
		var maxObjectSpawn = GameManager.Instance.maxObjectSpawn;
		var chunkObjectPrefabs = GameManager.Instance.chunkObjectPrefabs;
		var randomAmount = UnityEngine.Random.Range(minObjectSpawn, maxObjectSpawn + 1);
		var candidates = new List<ChunkObject>();

		foreach (var prefab in chunkObjectPrefabs)
		{
			var randomChance = UnityEngine.Random.Range(0, 101);

			if (randomChance < prefab.spawnChance)
			{
				candidates.Add(prefab);
			}
		}

		if (candidates.Count == 0)
		{
			return;
		}

		while (randomAmount > 0)
		{
			var randomIndex = UnityEngine.Random.Range(0, candidates.Count);
			var randomObject = candidates[randomIndex];
			var chunkObject = GameObject.Instantiate(randomObject);

			chunkObject.transform.position = new Vector3(
				(x * chunkSize) + UnityEngine.Random.Range(0, chunkSize),
				0f,
				(z * chunkSize) + UnityEngine.Random.Range(0, chunkSize)
			);

			AddChild(chunkObject.gameObject);

			chunkObject.SetChunk(this);
			chunkObject.Create();

			randomAmount--;
		}
	}

	public void AddLoot()
	{
		var chunkSize = GameManager.Instance.chunkSize;
		var minLootSpawn = GameManager.Instance.minLootSpawn;
		var maxLootSpawn = GameManager.Instance.maxLootSpawn;
		var lootItemPrefabs = GameManager.Instance.lootItemPrefabs;
		var randomAmount = UnityEngine.Random.Range(minLootSpawn, maxLootSpawn + 1);
		var candidates = new List<LootItem>();

		foreach (var prefab in lootItemPrefabs)
		{
			var randomChance = UnityEngine.Random.Range(0, 101);

			if (randomChance < prefab.spawnChance)
			{
				candidates.Add(prefab);
			}
		}

		if (candidates.Count == 0)
		{
			return;
		}

		while (randomAmount > 0)
		{
			var randomIndex = UnityEngine.Random.Range(0, candidates.Count);
			var randomLoot = candidates[randomIndex];
			var lootObject = GameObject.Instantiate(randomLoot);

			lootObject.transform.position = new Vector3(
				(x * chunkSize) + UnityEngine.Random.Range(0, chunkSize),
				0f,
				(z * chunkSize) + UnityEngine.Random.Range(0, chunkSize)
			);

			AddChild(lootObject.gameObject);

			randomAmount--;
		}
	}

	public void AddEnvironment()
	{
		var chunkSize = GameManager.Instance.chunkSize;
		var minEnvironmentSpawn = GameManager.Instance.minEnvironmentSpawn;
		var maxEnvironmentSpawn = GameManager.Instance.minEnvironmentSpawn;
		var environmentObjectPrefabs = GameManager.Instance.environmentObjectPrefabs;
		var randomAmount = UnityEngine.Random.Range(minEnvironmentSpawn, maxEnvironmentSpawn + 1);
		var candidates = new List<EnvironmentObject>();

		foreach (var prefab in environmentObjectPrefabs)
		{
			var randomChance = UnityEngine.Random.Range(0, 101);

			if (randomChance < prefab.spawnChance)
			{
				candidates.Add(prefab);
			}
		}

		if (candidates.Count == 0)
		{
			return;
		}

		while (randomAmount > 0)
		{
			var randomIndex = UnityEngine.Random.Range(0, candidates.Count);
			var randomLoot = candidates[randomIndex];
			var environmentObject = GameObject.Instantiate(randomLoot);

			environmentObject.transform.position = new Vector3(
				(x * chunkSize) + UnityEngine.Random.Range(-12, 12),
				0f,
				(z * chunkSize) + UnityEngine.Random.Range(-12, 12)
			);

			environmentObject.transform.eulerAngles = new Vector3(0f, UnityEngine.Random.Range(0,360), 0f);

			AddChild(environmentObject.gameObject);

			randomAmount--;
		}
	}

	public void AddGrass()
	{
		var chunkSize = GameManager.Instance.chunkSize;
		var minGrassSpawn = GameManager.Instance.minGrassSpawn;
		var maxGrassSpawn = GameManager.Instance.maxGrassSpawn;
		var grassObjectPrefabs = GameManager.Instance.grassObjectPrefabs;
		var randomAmount = UnityEngine.Random.Range(minGrassSpawn, maxGrassSpawn + 1);
		var candidates = new List<GrassObject>();

		foreach (var prefab in grassObjectPrefabs)
		{
			var randomChance = UnityEngine.Random.Range(0, 101);

			if (randomChance < prefab.spawnChance)
			{
				candidates.Add(prefab);
			}
		}

		if (candidates.Count == 0)
		{
			return;
		}

		while (randomAmount > 0)
		{
			var randomIndex = UnityEngine.Random.Range(0, candidates.Count);
			var randomLoot = candidates[randomIndex];
			var environmentObject = GameObject.Instantiate(randomLoot);

			environmentObject.transform.position = new Vector3(
				(x * chunkSize) + UnityEngine.Random.Range(-12f, 12f),
				0f,
				(z * chunkSize) + UnityEngine.Random.Range(-12f, 12f)
			);

			environmentObject.transform.eulerAngles = new Vector3(0f, UnityEngine.Random.Range(0,360), 0f);

			AddChild(environmentObject.gameObject);

			randomAmount--;
		}
	}

	public bool IsInRange(int otherX, int otherZ, int distance)
	{
		if ((x < otherX - distance || x > otherX + distance)
			|| (z < otherZ - distance || z > otherZ + distance))
		{
			return false;
		}

		return true;
	}

	public void RemoveChild(GameObject child)
	{
		_children.Remove(child);
	}

	public void RemoveChildren()
	{
		GameObject.Destroy(_ground.gameObject);

		foreach (var child in _children)
		{
			GameObject.Destroy(child);
		}
	}

	public Chunk()
	{
		_children = new List<GameObject>();
	}
}