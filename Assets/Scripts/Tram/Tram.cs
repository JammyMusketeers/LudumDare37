using UnityEngine;
using System.Collections;

public class Tram : MonoBehaviour
{
	public float minMoveSpeed = 0f;
	public float maxMoveSpeed = 5f;
	public float moveSpeed = 1f;
	public float engineSpoolRate = 1f;
	public float momentumDecay = 0.95f;
	public Vector3 moveVector;
	public GameObject exterior;
	public Collider entryTrigger;
	public Collider exitTrigger;
	public Collider exteriorCollider;
	public Collider[] interiorColliders;
	public Collider engineCollider;
	public Transform insideSpawn;
	public Transform outsideSpawn;
	public Transform[] wheels;
	public Transform leverObject;
	public GameObject leverInRangeObject;
	public float wheelSpeed = 1f;
	public Transform throttleLever;
	public float leverAngleRange = 90;
	public float leverAngleOffset = 45;
	public AudioSource engineSoundEmitter;
	public float pitchMinimum = 0.5f;
	public float pitchMultiplier = 1.5f;

	private bool _isBeingOperated;
	private float throttleLevel;
	private float engineSpool;
	private float currentSpeed;
	private bool engineOn = false;

	private Player _player;

	public float fuel = 100f;
	public float fuelMultiplier = 1f;

	public virtual void PlayerEnter(Player player)
	{
		exteriorCollider.enabled = false;
		exterior.GetComponent<Renderer>().enabled = false;

		_player = player;
	}

	public virtual void PlayerLeave(Player player)
	{
		exteriorCollider.enabled = true;
		exterior.GetComponent<Renderer>().enabled = true;

		_player = null;
	}

	public virtual void SetThrottle(float newThrottleLevel)
	{
		throttleLevel = newThrottleLevel;

		if (throttleLevel > 1)
			throttleLevel = 1;

		if (throttleLevel < 0)
			throttleLevel = 0;
		if (throttleLevel > 0 && !engineOn)
			EngineOn(true);

		if (throttleLever != null) 
		{
			float leverAngle = (throttleLevel * leverAngleRange) + leverAngleOffset;
			throttleLever.localEulerAngles = new Vector3 (leverAngle,0,0);
		}
	}

	public void EngineOn(bool On)
	{
		if (On)
		{
			engineOn = true;
			engineSoundEmitter.Play();
		} 
		else
		{
			engineOn = false;
			engineSpool = 0;
			engineSoundEmitter.Stop();
		}
	}

	public void Fill(float amount)
	{
		fuel += amount;
		fuel = Mathf.Clamp(fuel,0, 100f);
		if(fuel > 0)
		{
			EngineOn(true);
		}
	}

	public float GetSpeedPerSecond()
	{
		return currentSpeed;
	}

	public float GetFuelConsumption()
	{
		return engineSpool * fuelMultiplier;
	}

	public bool IsCloseToLever(Vector3 position)
	{
		return Vector3.Distance(leverObject.position, position) < 1.5f; 
	}

	public bool IsBeingOperated()
	{
		return _isBeingOperated;
	}

	public void SetIsBeingOperated(bool isBeingOperated)
	{
		_isBeingOperated = isBeingOperated;
	}

	protected virtual void Update()
	{
		if (!_isBeingOperated && _player != null && IsCloseToLever(_player.transform.position))
		{
			leverInRangeObject.SetActive(true);
		}
		else
		{
			leverInRangeObject.SetActive(false);
		}

		if (IsBeingOperated())
		{
			var v = Input.GetAxis("Vertical");

			if (v < 0f)
			{
				SetThrottle(throttleLevel - Time.deltaTime);
			}
			else if (v > 0f)
			{
				SetThrottle(throttleLevel + Time.deltaTime);
			}
		}

		// calculate engine:
		if(engineOn)
		{
			engineSpool = Mathf.Lerp(engineSpool, throttleLevel, engineSpoolRate * Time.deltaTime);
	
			if (engineSoundEmitter != null)
				engineSoundEmitter.pitch = pitchMinimum + (engineSpool * pitchMultiplier);
		} 

		if (throttleLevel > 0)
		{
			currentSpeed += engineSpool * Time.deltaTime;

			// cap max speed
			if (currentSpeed > maxMoveSpeed)
				currentSpeed = maxMoveSpeed;
		}
		else
		{
			// apply brakes here!
			currentSpeed *= 0.98f;
		}

		// constant bleed-off of speed
		currentSpeed *= momentumDecay;


		// apply motion:

		var position = transform.position;

		position += (moveVector * currentSpeed * Time.deltaTime);

		if (wheels.Length > 0)
		{
			foreach (Transform wheel in wheels)
			{
				wheel.Rotate(Vector3.forward * currentSpeed * wheelSpeed * Time.deltaTime);
			}
		}

		transform.position = position;

		fuel -= engineSpool * fuelMultiplier * Time.deltaTime;
		fuel = Mathf.Clamp(fuel,0f,100f);

		if(fuel <= 0 && engineOn)
		{
			EngineOn(false);
		}
	}

	public void Reset()
	{
		_isBeingOperated = false;
	}

	protected virtual void Start()
	{
		
	}
}