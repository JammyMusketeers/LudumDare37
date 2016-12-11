using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
	public Tram CurrentTram { get; set; }

	public IsometricCamera camera;
	public Image fadeToBlack;
	public int chunkSize = 24;
	public int chunkDistance = 2;
	public Ground groundPrefab;
	public GameObject railPrefab;
	public Player player;
	public UIManager interfaceManager;
	public GameObject storm;

	public LootItem[] lootItemPrefabs;
	public int minLootSpawn = 0;
	public int maxLootSpawn = 4;

	public ChunkObject[] chunkObjectPrefabs;
	public int minObjectSpawn = 0;
	public int maxObjectSpawn = 4;

	public EnvironmentObject[] environmentObjectPrefabs;
	public int minEnvironmentSpawn = 0;
	public int maxEnvironmentSpawn = 4;

	public GrassObject[] grassObjectPrefabs;
	public int minGrassSpawn = 40;
	public int maxGrassSpawn = 100;

	private float _nextUpdateGround;
	private float _nextHungerDecrease = 3f;
	private float _nextStormChecker;
	private List<Chunk> _chunks;

	private Action _fadeToBlackCallback;
	private float _targetFadeAlpha;
	private bool _isFading;

	private bool stormLevel0 = true;
	private bool stormLevel1 = false;
	private bool stormLevel2 = false;
	private bool stormlevel3 = false;

	public void FadeToBlack(Action callback = null)
	{
		if (!_isFading && _targetFadeAlpha == 0f)
		{
			_fadeToBlackCallback = callback;
			_targetFadeAlpha = 1f;
			_isFading = true;
		}
	}

	public void FadeFromBlack(Action callback = null)
	{
		if (!_isFading && _targetFadeAlpha == 1f)
		{
			_fadeToBlackCallback = callback;
			_targetFadeAlpha = 0f;
			_isFading = true;
		}
	}

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

	public void AddChunk(int x, int z, bool addLoot = false)
	{
		var chunk = new Chunk();

		chunk.x = x;
		chunk.z = z;

		chunk.AddGround();
		chunk.AddGrass();
		

		if (addLoot && x !=0)
		{
			chunk.AddLoot();
			chunk.AddObjects();
		}

		if(x !=0)
		{
			chunk.AddEnvironment();
		}

		if (x == 0)
		{
			chunk.AddRail();
		}

		_chunks.Add(chunk);
	}

	public void AddChunk(Vector3 position, bool addLoot = false)
	{
		var chunkX = Mathf.FloorToInt(position.x / chunkSize);
		var chunkZ = Mathf.FloorToInt(position.z / chunkSize);

		AddChunk(chunkX, chunkZ, addLoot);
	}

	public void ForEachChunkInRange(int chunkX, int chunkZ, int distance, Action<int, int> callback)
	{
		for (int x = chunkX - distance; x <= chunkX + distance; x++)
		{
			for (int z = chunkZ - distance; z <= chunkZ + distance; z++)
			{
				callback(x, z);
			}
		}
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
					AddChunk(x, z, true);
				}
			}
		}
	}

	protected virtual void Update()
	{
		if (_isFading)
		{
			var color = fadeToBlack.color;

			color.a = Mathf.Lerp(color.a, _targetFadeAlpha, Time.deltaTime * 8f);

			fadeToBlack.color = color;

			if (Mathf.Approximately(color.a, _targetFadeAlpha))
			{
				var callback = _fadeToBlackCallback;

				_fadeToBlackCallback = null;
				_isFading = false;

				if (callback != null)
				{
					callback();
				}
			}
		}

		if(Time.time >= _nextHungerDecrease)
		{
			player.hunger -= 1;
			_nextHungerDecrease += 3f;
		}

		if (Time.time >= _nextUpdateGround)
		{
			var playerPosition = player.transform.position;
			var tramPosition = CurrentTram.transform.position;
			var playerChunkX = Mathf.FloorToInt(playerPosition.x / chunkSize);
			var playerChunkZ = Mathf.FloorToInt(playerPosition.z / chunkSize);
			var tramChunkX = Mathf.FloorToInt(tramPosition.x / chunkSize);
			var tramChunkZ = Mathf.FloorToInt(tramPosition.z / chunkSize);

			ForEachChunkInRange(tramChunkX, tramChunkZ, chunkDistance, (x, z) =>
			{
				var chunk = GetChunkAt(x, z);

				if (chunk == null)
				{
					AddChunk(x, z, true);
				}
			});

			ForEachChunkInRange(playerChunkX, playerChunkZ, chunkDistance, (x, z) =>
			{
				var chunk = GetChunkAt(x, z);

				if (chunk == null)
				{
					AddChunk(x, z);
				}
			});

			for (int i = _chunks.Count - 1; i >= 0; i--)
			{
				var chunk = _chunks[i];
				var clearTime = chunk.GetClearTime();

				if (clearTime == 0f)
				{
					if (!chunk.IsInRange(playerChunkX, playerChunkZ, chunkDistance)
						&& !chunk.IsInRange(tramChunkX, tramChunkZ, chunkDistance))
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

		//Check how far Storm is
		if(Time.time >= _nextStormChecker)
		{

			var distance = Vector3.Distance(storm.transform.position, CurrentTram.transform.position);
			
			if(distance >= 900)
			{
				stormLevel0 = true;
				stormLevel1 = false;

			}

			if(distance < 900f && distance >= 500f)
			{
				stormLevel1 = true;
				stormLevel2 = false;
			}

			if(distance < 500f && distance >= 200f)
			{
				stormLevel1 = false;
				stormLevel2 = true;
				stormlevel3 = false;
			}

			if(distance < 200f)
			{
				stormLevel2 = false;
				stormlevel3 = true;
			}

			_nextStormChecker = Time.time + 1f;

		}
		
		interfaceManager.SetSpeed(CurrentTram.GetSpeedPerSecond());
		interfaceManager.SetDistance(CurrentTram.transform.position.z);
		interfaceManager.SetHealth(player.health / 100f);
		interfaceManager.SetHunger(player.hunger /100f);
		interfaceManager.SetFuelConsumption(CurrentTram.GetFuelConsumption());
		interfaceManager.SetFuel(CurrentTram.fuel / 100f);

		Debug.Log("Storm Distance = " + Vector3.Distance(storm.transform.position, CurrentTram.transform.position));
		
	}
}