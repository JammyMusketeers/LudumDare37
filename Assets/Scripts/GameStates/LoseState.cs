using UnityEngine;
using System.Collections;

public class LoseState : BaseState
{
	public override void OnLoad(BaseState lastState)
	{
		GameManager.Instance.loseUI.SetActive(true);
		GameManager.Instance.MuteMusic();

		StormManager.Instance.SetStormActive(false);

		UIManager.Instance.loseDistanceTravelled.text = UIManager.Instance.distanceAmount.text;
	}

	public override void OnUnload(BaseState nextState)
	{
		GameManager.Instance.loseUI.SetActive(false);
	}

	public override void Update()
	{
		if (Input.GetButtonDown("Use"))
		{
			StateManager.Instance.SetState(new MenuState());
		}

		base.Update();
	}
}