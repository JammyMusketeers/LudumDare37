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

		while (randomAmount > 0)
		{
			var randomIndex = UnityEngine.Random.Range(0, chunkObjectPrefabs.Length);
			var randomObject = chunkObjectPrefabs[randomIndex];
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

		while (randomAmount > 0)
		{
			var randomIndex = UnityEngine.Random.Range(0, lootItemPrefabs.Length);
			var randomLoot = lootItemPrefabs[randomIndex];
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