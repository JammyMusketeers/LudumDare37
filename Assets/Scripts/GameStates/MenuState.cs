﻿using UnityEngine;
using System.Collections;

public class MenuState : BaseState
{
	public override void OnLoad(BaseState lastState)
	{
		GameManager.Instance.camera.Reset();
		GameManager.Instance.menuUI.SetActive(true);

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

		GameManager.Instance.CurrentPlayer.Reset();
		GameManager.Instance.CurrentPlayer.SetHidden(true);
		GameManager.Instance.CurrentPlayer.PutInsideTram(GameManager.Instance.CurrentTram);
	}

	public override void OnUnload(BaseState nextState)
	{
		GameManager.Instance.menuUI.SetActive(false);
		GameManager.Instance.ClearChunks();
	}
}