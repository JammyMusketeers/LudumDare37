using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StormManager : Singleton<GameManager> {

	public PlayerControl playerControl;
	public Player player;
	public GameObject storm;

	private bool stormLevel0 = true;
	private bool stormLevel1 = false;
	private bool stormLevel2 = false;
	private bool stormLevel3 = false;
	private float _nextStormChecker;

	
	// Update is called once per frame
	void Update () {

		//Check how far Storm is
		if(Time.time >= _nextStormChecker)
		{

			var distance = Vector3.Distance(storm.transform.position, player.transform.position);
			
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
				stormLevel3 = false;
			}

			if(distance < 200f)
			{
				stormLevel2 = false;
				stormLevel3 = true;
			}
			
			if (stormLevel0)
			{
				playerControl.stormSpeedDecay = 1f;
			}

			if (stormLevel1)
			{
				playerControl.stormSpeedDecay = 0.9f;
			}

			if (stormLevel2)
			{
				playerControl.stormSpeedDecay = 0.7f;
			}

			if (stormLevel3)
			{
				playerControl.stormSpeedDecay = 0.5f;
			}

			Debug.Log("Storm Distance = " + distance);
			_nextStormChecker = Time.time + 1f;

		}
	
	}
}
