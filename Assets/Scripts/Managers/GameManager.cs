using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
	[Serializable]
	public class GroundType
	{
		public Ground prefab;
		public int spawnChance;
	}

	public Tram CurrentTram { get; set; }
	public Player CurrentPlayer { get; set; }

	public IsometricCamera camera;
	public AudioMixer audioMixer;
	public AudioSource musicSource;
	public Player player;
	public Tram tram;
	public Image fadeToBlack;
	public int chunkSize = 24;
	public int chunkDistance = 2;
	public Ground groundPrefab;
	public GameObject railPrefab;

	public GroundType[] groundTypes;

	public UIManager interfaceManager;

	public GameObject menuUI;
	public GameObject gameUI;
	public GameObject loseUI;

	public Button startGameButton;
	public Button retryButton;

	public LootItem[] lootItemPrefabs;
	public int minLootSpawn = 0;
	public int maxLootSpawn = 4;
	public int lootSpawnDistanceAdd = 1;

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
	private List<Chunk> _chunks;

	private Action _fadeToBlackCallback;
	private float _targetFadeAlpha;
	private bool _isDeathQueued;
	private bool _isFading;

	public void MuteMusic()
	{
		musicSource.Stop();
	}

	public void PlayMusic()
	{
		musicSource.Play();
	}

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

	public void QueueDeath()
	{
		_isDeathQueued = true;
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

		var groundCandidates = new List<Ground>();

		foreach (var groundType in groundTypes)
		{
			var spawnChance = UnityEngine.Random.Range(0, 101);

			if (spawnChance < groundType.spawnChance)
			{
				groundCandidates.Add(groundType.prefab);
			}
		}

		var ground = groundPrefab;

		if (groundCandidates.Count > 0)
		{
			ground = groundCandidates[UnityEngine.Random.Range(0, groundCandidates.Count)];
		}

		chunk.AddGround(ground);
		chunk.AddGrass();

		if (addLoot)
		{
			chunk.AddLoot();
			if(x != 0)
			{
				chunk.AddObjects();
			}
			
		}

		if (x != 0)
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

	public void ClearChunks()
	{
		if (_chunks == null)
		{
			return;
		}

		foreach (var chunk in _chunks)
		{
			chunk.RemoveChildren();
		}

		_chunks.Clear();
	}

	protected override void OnSetup()
	{
		_chunks = new List<Chunk>();
		
		CurrentPlayer = player;
		CurrentTram = tram;

		startGameButton.onClick.AddListener(() =>
		{
			StateManager.Instance.SetState(new GameState());
		});

		retryButton.onClick.AddListener(() =>
		{
			StateManager.Instance.SetState(new MenuState());
		});
	}

	protected virtual void Update()
	{
		if (Input.GetButtonDown("Back"))
		{
			Application.Quit();
		}

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

		if (CurrentPlayer != null && CurrentTram != null && Time.time >= _nextUpdateGround)
		{
			var playerPosition = CurrentPlayer.transform.position;
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
				var isPlayerInRange = chunk.IsInRange(playerChunkX, playerChunkZ, chunkDistance);
				var isTramInRange = chunk.IsInRange(tramChunkX, tramChunkZ, chunkDistance);

				if (clearTime == 0f)
				{
					if (!isPlayerInRange && !isTramInRange)
					{
						chunk.SetClearTime(30f);
					}
				}
				else if (!isPlayerInRange && !isTramInRange)
				{
					if (Time.time >= clearTime)
					{
						chunk.RemoveChildren();

						_chunks.Remove(chunk);
					}
				}
				else
				{
					chunk.CancelClear();
				}
			}

			_nextUpdateGround = Time.time + 1f;
		}

		if (_isDeathQueued)
		{
			_isDeathQueued = false;

			StateManager.Instance.SetState(new LoseState());
		}
	}
}