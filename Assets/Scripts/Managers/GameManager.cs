using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
	public Tram CurrentTram { get; set; }

	public IsometricCamera camera;
	public int chunkSize = 24;
	public int chunkDistance = 2;
	public Ground groundPrefab;
	public Player player;

	private float _nextUpdateGround;
	private List<Chunk> _chunks;

	public Chunk GetChunkAt(Vector3 position)
	{
		var chunkX = Mathf.FloorToInt(position.x / chunkSize);
		var chunkZ = Mathf.FloorToInt(position.z / chunkSize);

		return GetChunkAt(chunkX, chunkZ);
	}

	public Chunk GetChunkAt(int x, int z)
	{
		foreach (var chunk in _chunks)
		{
			if (x == chunk.x && z == chunk.z)
			{
				return chunk;
			}
		}

		return null;
	}

	public void AddChunk(int x, int z)
	{
		var chunk = new Chunk();

		chunk.x = x;
		chunk.z = z;

		chunk.AddGround();

		_chunks.Add(chunk);
	}

	public void AddChunk(Vector3 position)
	{
		var chunkX = Mathf.FloorToInt(position.x / chunkSize);
		var chunkZ = Mathf.FloorToInt(position.z / chunkSize);

		AddChunk(chunkX, chunkZ);
	}

	protected override void OnSetup()
	{
		_chunks = new List<Chunk>();

		for (int x = -chunkDistance; x <= chunkDistance; x++)
		{
			for (int z = -chunkDistance; z <= chunkDistance; z++)
			{
				var chunk = GetChunkAt(x, z);

				if (chunk == null)
				{
					AddChunk(x, z);
				}
			}
		}
	}

	protected virtual void Update()
	{
		if (Time.time >= _nextUpdateGround)
		{
			var playerPosition = player.transform.position;
			var chunkX = Mathf.FloorToInt(playerPosition.x / chunkSize);
			var chunkZ = Mathf.FloorToInt(playerPosition.z / chunkSize);

			for (int x = chunkX - chunkDistance; x <= chunkX + chunkDistance; x++)
			{
				for (int z = chunkZ - chunkDistance; z <= chunkZ + chunkDistance; z++)
				{
					var chunk = GetChunkAt(x, z);

					if (chunk == null)
					{
						AddChunk(x, z);
					}
				}
			}

			for (int i = _chunks.Count - 1; i >= 0; i--)
			{
				var chunk = _chunks[i];
				var clearTime = chunk.GetClearTime();

				if (clearTime == 0f)
				{
					if ((chunk.x < chunkX - chunkDistance || chunk.x > chunkX + chunkDistance)
						|| (chunk.z < chunkZ - chunkDistance || chunk.z > chunkZ + chunkDistance))
					{
						chunk.SetClearTime(30f);
					}
				}
				else if (Time.time >= clearTime)
				{
					chunk.RemoveChildren();

					_chunks.Remove(chunk);
				}
			}

			_nextUpdateGround = Time.time + 1f;
		}
	}
}
