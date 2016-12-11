using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StormManager : Singleton<GameManager> {

	public PlayerControl playerControl;
	public Player player;
	public GameObject storm;
	public ParticleSystem stormEffect1;
	public ParticleSystem stormEffect2;
	public ParticleSystem stormEffect3;

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
			var s1 = stormEffect1.emission;
			var s2 = stormEffect2.emission;
			var s3 = stormEffect3.emission;
			
			if(distance >= 900)
			{
				stormLevel0 = true;
				stormLevel1 = false;

			}

			if(distance <= 900f && distance >= 500f)
			{
				stormLevel0 = false;
				stormLevel1 = true;
				stormLevel2 = false;
			}

			if(distance <= 500f && distance >= 200f)
			{
				stormLevel1 = false;
				stormLevel2 = true;
				stormLevel3 = false;
			}

			if(distance <= 200f)
			{
				stormLevel2 = false;
				stormLevel3 = true;
			}
			
			if (stormLevel0)
			{
				playerControl.stormSpeedDecay = 1f;
				s1.enabled = false;
			}

			if (stormLevel1)
			{
				playerControl.stormSpeedDecay = 0.9f;
				s1.enabled = true;
				s2.enabled = false;
			}
			if (stormLevel2)
			{
				playerControl.stormSpeedDecay = 0.7f;
				s2.enabled = true;
				s3.enabled = false;
			}

			if (stormLevel3)
			{
				playerControl.stormSpeedDecay = 0.5f;
				s3.enabled = true;
			}

			Debug.Log("Storm Distance = " + distance);
			_nextStormChecker = Time.time + 1f;

		}
	
	}

}
