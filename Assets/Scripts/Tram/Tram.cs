using UnityEngine;
using System.Collections;

public class Tram : MonoBehaviour
{
	public float minMoveSpeed = 0f;
	public float maxMoveSpeed = 5f;
	public float moveSpeed = 1f;
	public float engineSpoolRate = 1f;
	public float momentumDecay = 1f;
	public Vector3 moveVector;
	public GameObject exterior;
	public GameObject exteriorSubstitute;
	public Collider entryTrigger;
	public Collider exitTrigger;
	public Collider exteriorCollider;
	public Collider[] interiorColliders;
	public Transform startSpawn;
	public Transform insideSpawn;
	public Transform[] wheels;
	public Transform leverObject;
	public Transform storageObject;
	public Transform engineObject;
	public InventoryBox storageInventory;
	public GameObject leverInRangeObject;
	public GameObject engineInRangeObject;
	public GameObject storageInRangeObject;
	public float wheelSpeed = 1f;
	public Transform throttleLever;
	public float leverAngleRange = 90;
	public float leverAngleOffset = 45;
	public AudioSource engineSoundEmitter;
	public AudioClip soundStartEngine;
	public AudioClip soundStopEngine;
	public float pitchMinimum = 0.5f;
	public float pitchMultiplier = 1.5f;

	private bool _isBeingOperated;
	private float throttleLevel;
	public float engineSpool;
	private float currentSpeed;
	private bool engineOn = false;

	private Player _player;
	private AudioSource thisAudio;

	public float fuel = 100f;
	public float fuelMultiplier = 1f;

	public CanvasGroup engineCanvas;
	public CanvasGroup driveCanvas;
	public CanvasGroup storageCanvas;

	private bool _initialized;

	public virtual void PlayerEnter(Player player)
	{
		exteriorCollider.enabled = false;
		exteriorSubstitute.SetActive(true);
		exterior.SetActive(false);

		_player = player;
	}

	public virtual void PlayerLeave(Player player)
	{
		exteriorCollider.enabled = true;
		exteriorSubstitute.SetActive(false);
		exterior.SetActive(true);

		_player = null;
	}

	public virtual void SetThrottle(float newThrottleLevel)
	{
		throttleLevel = newThrottleLevel;

		if (throttleLevel > 1)
			throttleLevel = 1;

		if (throttleLevel < 0)
			throttleLevel = 0;

		if (throttleLevel > 0 && !engineOn && fuel > 0)
			EngineOn(true);

		if (throttleLever != null) 
		{
			float leverAngle = (throttleLevel * leverAngleRange) + leverAngleOffset;
			throttleLever.localEulerAngles = new Vector3 (leverAngle,0,0);
		}
	}

	private void SetSpeed(float speed)
	{
		currentSpeed = speed;
	}

	public void ShowCanvasTips(bool isShown)
	{
		storageCanvas.gameObject.SetActive(isShown);
		driveCanvas.gameObject.SetActive(isShown);
		engineCanvas.gameObject.SetActive(isShown);
	}

	public void EngineOn(bool On)
	{
		if (On)
		{
			engineOn = true;
			engineSoundEmitter.Play();

			if (soundStartEngine != null)
				thisAudio.PlayOneShot(soundStartEngine);
		} 
		else
		{
			engineOn = false;
			engineSpool = 0;
			engineSoundEmitter.Stop();

			if (soundStopEngine != null)
				thisAudio.PlayOneShot(soundStopEngine);
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

	public bool IsCloseToStorage(Vector3 position)
	{
		return Vector3.Distance(storageObject.position, position) < 3f; 
	}

	public bool IsCloseToEngine(Vector3 position)
	{
		return Vector3.Distance(engineObject.position, position) < 1.5f; 
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

	public void UpdateCanvasTip(CanvasGroup group, Vector3 position)
	{
		var distance = Vector3.Distance(group.transform.position, position) - 2f;
		var clamped = Mathf.Clamp(distance, 0f, 4f);

		group.alpha = 1f - ((1f / 4f) * clamped);
	}

	protected virtual void Update()
	{
		var player = GameManager.Instance.CurrentPlayer;

		if (!_isBeingOperated && player != null && IsCloseToLever(player.transform.position))
		{
			leverInRangeObject.SetActive(true);
		}
		else
		{
			leverInRangeObject.SetActive(false);
		}

		if (player != null && !player.IsInsideTram() && IsCloseToStorage(player.transform.position))
		{
			storageInRangeObject.SetActive(true);
		}
		else
		{
			if (storageInventory.IsOpen())
			{
				storageInventory.Close();
			}

			storageInRangeObject.SetActive(false);
		}

		if (player != null && player.IsInsideTram() && IsCloseToEngine(player.transform.position))
		{
			engineInRangeObject.SetActive(true);
		}
		else
		{
			engineInRangeObject.SetActive(false);
		}

		if (player != null)
		{
			if (player.IsInsideTram())
			{
				UpdateCanvasTip(driveCanvas, player.transform.position);
				UpdateCanvasTip(engineCanvas, player.transform.position);

				storageCanvas.alpha = 0f;
			}
			else
			{
				UpdateCanvasTip(storageCanvas, player.transform.position);

				engineCanvas.alpha = 0f;
				driveCanvas.alpha = 0f;
			}
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
			currentSpeed *= 1 - (momentumDecay * Time.deltaTime);
		}

		// constant bleed-off of speed
		currentSpeed *= 1 - (momentumDecay * Time.deltaTime);


		// apply motion:

		var position = transform.position;

		position += (moveVector * currentSpeed * Time.deltaTime);
		Debug.Log("CurrentSpeed: "+currentSpeed+" engineSpool: "+engineSpool+" throt: "+throttleLevel);

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
		Awake();

		storageInventory.Reset();

		_isBeingOperated = false;

		EngineOn(false);

		fuel = 100f;
	}

	protected virtual void Start()
	{
		
	}

	protected virtual void Awake()
	{
		if (!_initialized)
		{
			thisAudio = GetComponent<AudioSource>() as AudioSource;
			
			_initialized = true;
		}
	}
}