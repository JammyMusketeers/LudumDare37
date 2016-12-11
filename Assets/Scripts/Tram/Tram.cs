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
	public Collider enterExitTrigger;
	public Collider exteriorCollider;
	public Collider[] interiorColliders;
	public Collider engineCollider;
	public Transform insideSpawn;
	public Transform outsideSpawn;
	public Transform[] wheels;
	public float wheelSpeed = 1f;
	public Transform throttleLever;
	public float leverAngleRange = 90;
	public float leverAngleOffset = 45;
	public AudioSource engineSoundEmitter;
	public float pitchMinimum = 0.5f;
	public float pitchMultiplier = 1.5f;

	private float throttleLevel;
	private float engineSpool;
	private float currentSpeed;

	public float fuel = 100f;
	public float fuelMultiplier = 1f;

	public virtual void PlayerEnter(Player player)
	{
		exteriorCollider.enabled = false;
		exterior.GetComponent<Renderer>().enabled = false;
	}

	public virtual void PlayerLeave(Player player)
	{
		exteriorCollider.enabled = true;
		exterior.GetComponent<Renderer>().enabled = true;
	}

	public virtual void SetThrottle(float newThrottleLevel)
	{
		throttleLevel = newThrottleLevel;
		if (throttleLevel > 1)
			throttleLevel = 1;

		if (throttleLevel < 0)
			throttleLevel = 0;

		if (throttleLever != null) 
		{
			float leverAngle = (throttleLevel * leverAngleRange) + leverAngleOffset;
			throttleLever.localEulerAngles = new Vector3 (leverAngle,0,0);
		}
	}

	public void Fill(float fuel)
	{
		fuel += fuel;
		fuel = Mathf.Clamp(fuel,0, 100f);
	}

	public float GetSpeedPerSecond()
	{
		return currentSpeed;
	}

	public float GetFuelConsumption()
	{
		return engineSpool * fuelMultiplier;
	}

	protected virtual void Update()
	{
		// DEBUG MOVE CODE (CONNA PLEASE REPLACE WITH PLAYER INPUTS!)
		if (Input.GetKey(KeyCode.K))
		{
			SetThrottle(throttleLevel - Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.T) && fuel > 0)
		{
			SetThrottle(throttleLevel + Time.deltaTime);
		}

		// calculate engine:

		engineSpool = Mathf.Lerp(engineSpool, throttleLevel, engineSpoolRate * Time.deltaTime);

		if (engineSoundEmitter != null)
			engineSoundEmitter.pitch = pitchMinimum + (engineSpool * pitchMultiplier);

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

		if(fuel <= 0)
		{
			throttleLevel = 0;
		}
	}

	protected virtual void Start()
	{
		GameManager.Instance.CurrentTram = this;
		SetThrottle(0);
	}
}