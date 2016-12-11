using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StateManager : Singleton<StateManager>
{
	private BaseState _currentState;

	protected override void OnSetup()
	{
		SetState(new MenuState());
	}

	protected virtual void Update()
	{
		if (_currentState != null)
		{
			_currentState.Update();
		}
	}

	public bool Is<T>() where T : BaseState
	{
		if (_currentState != null && _currentState is T)
		{
			return true;
		}

		return false;
	}

	public void SetState(BaseState state)
	{
		var lastState = _currentState;

		if (lastState != null)
		{
			lastState.OnUnload(state);
		}

		state.OnLoad(lastState);

		_currentState = state;
	}
}