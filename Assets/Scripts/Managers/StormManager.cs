using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StormManager : Singleton<StormManager>
{
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
	private Vector3 _startPosition;
	private bool _isStormActive;

	public void ResetStorm(Vector3 origin)
	{
		storm.transform.position = new Vector3(origin.x, origin.y, origin.z - 1000f);

		var s1 = stormEffect1.emission;
		var s2 = stormEffect2.emission;
		var s3 = stormEffect3.emission;

		s1.enabled = false;
		s2.enabled = false;
		s3.enabled = false;

		stormLevel0 = true;
		stormLevel1 = false;
		stormLevel2 = false;
		stormLevel3 = false;
	}
	
	public void SetStormActive(bool isActive)
	{
		_isStormActive = isActive;
	}

	protected virtual void Update()
	{
		if (!_isStormActive)
		{
			return;
		}

		if (Time.time >= _nextStormChecker)
		{
			var distance = Vector3.Distance(storm.transform.position, player.transform.position);
			var s1 = stormEffect1.emission;
			var s2 = stormEffect2.emission;
			var s3 = stormEffect3.emission;
			
			if (distance >= 900)
			{
				stormLevel0 = true;
				stormLevel1 = false;

			}

			if (distance <= 900f && distance >= 500f)
			{
				stormLevel0 = false;
				stormLevel1 = true;
				stormLevel2 = false;
			}

			if (distance <= 500f && distance >= 200f)
			{
				stormLevel1 = false;
				stormLevel2 = true;
				stormLevel3 = false;
			}

			if (distance <= 200f)
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

			_nextStormChecker = Time.time + 1f;
		}
	}
}