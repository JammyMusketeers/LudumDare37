using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ground : MonoBehaviour
{
	public Transform[] spawnPoints;

	private List<Transform> _spawnPoints;

	protected virtual void Awake()
	{
		_spawnPoints = new List<Transform>();

		foreach (var spawnPoint in spawnPoints)
		{
			_spawnPoints.Add(spawnPoint);
		}
	}

	public List<Transform> GetSpawnPoints()
	{
		return _spawnPoints;
	}

	public void SetSize(int size)
	{
		transform.localScale = new Vector3(size, size, 1f);
	}

	public void SetPosition(int x, int z)
	{
		var size = transform.localScale.x;

		transform.position = new Vector3(x * size, 0f, z * size);
	}
}