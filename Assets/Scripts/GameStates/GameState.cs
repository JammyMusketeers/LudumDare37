using UnityEngine;
using System.Collections;

public class GameState : BaseState
{
	private float _nextHungerDecrease = 3f;

	public override void OnLoad(BaseState lastState)
	{
		GameManager.Instance.gameUI.SetActive(true);

		/*
		var chunkDistance = GameManager.Instance.chunkDistance;

		for (int x = -chunkDistance; x <= chunkDistance; x++)
		{
			for (int z = -chunkDistance; z <= chunkDistance; z++)
			{
				var chunk = GameManager.Instance.GetChunkAt(x, z);

				if (chunk == null)
				{
					GameManager.Instance.AddChunk(x, z, true);
				}
			}
		}
		*/
	}

	public override void OnUnload(BaseState nextState)
	{
		GameManager.Instance.gameUI.SetActive(false);
		GameManager.Instance.ClearChunks();
	}

	public override void Update()
	{
		var interfaceManager = GameManager.Instance.interfaceManager;
		var chunkDistance = GameManager.Instance.chunkDistance;
		var chunkSize = GameManager.Instance.chunkSize;
		var player = GameManager.Instance.CurrentPlayer;
		var tram = GameManager.Instance.CurrentTram;

		if (Time.time >= _nextHungerDecrease)
		{
			player.hunger -= 1;
			_nextHungerDecrease += 3f;
		}

		interfaceManager.SetSpeed(tram.GetSpeedPerSecond());
		interfaceManager.SetDistance(tram.transform.position.z);
		interfaceManager.SetHealth(player.health / 100f);
		interfaceManager.SetHunger(player.hunger /100f);
		interfaceManager.SetFuelConsumption(tram.GetFuelConsumption());
		interfaceManager.SetFuel(tram.fuel / 100f);

		base.Update();
	}
}