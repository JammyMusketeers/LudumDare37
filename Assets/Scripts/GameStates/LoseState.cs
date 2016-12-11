﻿using UnityEngine;
using System.Collections;

public class LoseState : BaseState
{
	public override void OnLoad(BaseState lastState)
	{
		GameManager.Instance.loseUI.SetActive(true);
	}

	public override void OnUnload(BaseState nextState)
	{
		GameManager.Instance.loseUI.SetActive(false);
	}
}