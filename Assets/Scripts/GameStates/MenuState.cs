using UnityEngine;
using System.Collections;

public class MenuState : BaseState
{
	public override void OnLoad(BaseState lastState)
	{
		UnityEngine.Random.InitState(13373131);

		GameManager.Instance.ClearChunks();
		GameManager.Instance.camera.Reset();
		GameManager.Instance.menuUI.SetActive(true);

		StormManager.Instance.ResetStorm(new Vector3(0f, 0f, 0f));
		StormManager.Instance.SetStormActive(false);

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

		GameManager.Instance.CurrentTram.transform.position = new Vector3(0f, 0f, 0f);
		GameManager.Instance.CurrentTram.Reset();
		GameManager.Instance.CurrentTram.EngineOn(true);
		GameManager.Instance.CurrentTram.SetThrottle(1f);
		GameManager.Instance.CurrentTram.SetSpeed(GameManager.Instance.CurrentTram.maxMoveSpeed);

		GameManager.Instance.CurrentPlayer.Reset();
		GameManager.Instance.CurrentPlayer.SetHidden(true);
		GameManager.Instance.CurrentPlayer.PutInsideTram(GameManager.Instance.CurrentTram);
	}

	public override void OnUnload(BaseState nextState)
	{
		GameManager.Instance.menuUI.SetActive(false);
	}

	public override void Update()
	{
		GameManager.Instance.CurrentTram.fuel = 100f;

		if (Input.GetButtonDown("Use"))
		{
			StateManager.Instance.SetState(new GameState());
		}

		base.Update();
	}
}