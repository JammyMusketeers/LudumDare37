using UnityEngine;
using System.Collections;

public class BaseState
{
	public virtual void OnLoad(BaseState lastState)
	{

	}

	public virtual void OnUnload(BaseState nextState)
	{

	}

	public virtual void Update() {}
}