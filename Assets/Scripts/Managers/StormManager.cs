using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class StormManager : Singleton<StormManager>
{
	public PlayerControl playerControl;
	public Player player;
	public GameObject storm;
	public float stormStartDistance = 800;
	public ParticleSystem stormEffect1;
	public ParticleSystem stormEffect2;
	public ParticleSystem stormEffect3;
	public AudioMixer audioMixer;
	public float audioChannel1MaxRange = 1000;
	public float audioChannel1MinRange = 100;
	public float audioChannel2MaxRange = 600;
	public float audioChannel2MinRange = 160;
	public float audioChannel3MaxRange = 350;
	public float audioChannel3MinRange = 20;
	public float offVolume = -20;

	private bool stormLevel0 = true;
	private bool stormLevel1 = false;
	private bool stormLevel2 = false;
	private bool stormLevel3 = false;
	private float _nextStormChecker;
	private Vector3 _startPosition;
	private bool _isStormActive;

	public void ResetStorm(Vector3 origin)
	{
		storm.transform.position = new Vector3(origin.x, origin.y, origin.z - stormStartDistance);

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
			var distance = player.gameObject.transform.position.z - storm.transform.position.z;
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

			// sound updates

			if (distance < audioChannel1MaxRange)
			{
				if (distance > audioChannel1MinRange) 
				{
					float rangeArea = audioChannel1MaxRange - audioChannel1MinRange;
					float pointInRange = distance - audioChannel1MinRange;
					float proportion = pointInRange / rangeArea;
					float volumeModifier = offVolume * proportion;
					audioMixer.SetFloat("level1Vol", volumeModifier);
				}
				else
				{
					audioMixer.SetFloat("level1Vol", 0);
				}
			}
			else
			{
				audioMixer.SetFloat("level1Vol", offVolume);
			}

			if (distance < audioChannel2MaxRange)
			{
				if (distance > audioChannel2MinRange) 
				{
					float rangeArea = audioChannel2MaxRange - audioChannel2MinRange;
					float pointInRange = distance - audioChannel2MinRange;
					float proportion = pointInRange / rangeArea;
					float volumeModifier = offVolume * proportion;
					audioMixer.SetFloat("level2Vol", volumeModifier);
				}
				else
				{
					audioMixer.SetFloat("level2Vol", 0);
				}
			}
			else
			{
				audioMixer.SetFloat("level2Vol", offVolume);
			}

			if (distance < audioChannel3MaxRange)
			{
				if (distance > audioChannel3MinRange) 
				{
					float rangeArea = audioChannel3MaxRange - audioChannel3MinRange;
					float pointInRange = distance - audioChannel3MinRange;
					float proportion = pointInRange / rangeArea;
					float volumeModifier = offVolume * proportion;
					audioMixer.SetFloat("level3Vol", volumeModifier);
				}
				else
				{
					audioMixer.SetFloat("level3Vol", 0);
				}
			}
			else
			{
				audioMixer.SetFloat("level3Vol", offVolume);
			}

			_nextStormChecker = Time.time + 1f;
		}
	}
}