using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public PlayerControl controller;

	private bool _isInsideTram;
	private bool _canEnterTram;
	public bool _hasItem;
	public bool _canFillEngine;
	private bool _takeDamage;
	private float _nextTakeDamage;
	public GameObject itemSlot;
	private  LootItem currentLootItem;
	private  GameObject lastTouchedLootItem;
	private  float pickupDistance = 1f;
	public float hunger = 100f;
	public float health = 100f;
	public AudioSource playerSound;

	private bool _isHidden;

	public void SetHidden(bool isHidden)
	{
		_isHidden = isHidden;
		gameObject.SetActive(!isHidden);
	}

	public void CollectLoot(LootItem lootItem)
	{
		lootItem.transform.parent = itemSlot.transform;
		lootItem.transform.localPosition = new Vector3 (0,0,0);
		currentLootItem = lootItem;
		playerSound.PlayOneShot(lootItem.collectSound);
		_hasItem = true;
	}

	public void Feed(float food)
	{
		hunger += food;
		hunger = Mathf.Clamp(hunger,0, 100f);
	}

	public void Heal(float amount)
	{
		health += amount;
		health = Mathf.Clamp(health,0, 100f);
	}

	public void Hit(float damage)
	{
		health -= damage;
		health = Mathf.Clamp(health, 0, 100f);

		if (health == 0f)
		{
			GameManager.Instance.QueueDeath();
		}
	}

	public bool IsInsideTram()
	{
		return _isInsideTram;
	}

	public bool CanEnterTram()
	{
		return _canEnterTram;
	}

	public void Reset()
	{
		controller.Reset();

		currentLootItem = null;

		_canFillEngine = false;
		_canEnterTram = false;
		_takeDamage = false;
		_hasItem = false;

		hunger = 100f;
		health = 100f;
	}

	public void LookAt(Vector3 target)
	{
		var currentRotation = transform.eulerAngles;

		transform.LookAt(target);

		var newRotation = transform.eulerAngles;

		newRotation.x = 0f;
		newRotation.z = 0f;
			
		transform.eulerAngles = newRotation;

		controller.SetRotation(transform.rotation);
	}

	public void PutInsideTram(Tram tram)
	{
		transform.parent = tram.transform;

		if (tram.startSpawn)
		{
			transform.position = tram.startSpawn.position;
		}
		else
		{
			transform.position = tram.insideSpawn.position;
		}

		controller.SetPosition(transform.position);
		controller.SetRotation(transform.rotation);
		
		_isInsideTram = true;
	}

	protected virtual void Update()
	{
		if (_isHidden || !StateManager.Instance.Is<GameState>())
		{
			return;
		}

		var tram = GameManager.Instance.CurrentTram;

		if (Input.GetButtonDown("Use"))
		{
			if (_isInsideTram)
			{
				if (tram.IsCloseToLever(transform.position))
				{
					if (!tram.IsBeingOperated())
					{
						tram.SetIsBeingOperated(true);

						LookAt(tram.leverInRangeObject.transform.position);
					}
					else
					{
						tram.SetIsBeingOperated(false);
					}
				}
			}

			if(_hasItem && _canFillEngine)
			{
				currentLootItem.SendMessage("UseEngine", GameManager.Instance.CurrentTram);
				currentLootItem = null;
				_hasItem = false;
				playerSound.PlayOneShot(currentLootItem.useSounds);
			}
			
			if(_hasItem && currentLootItem.canUseAnywhere)
			{
				currentLootItem.SendMessage("Use", this);
				currentLootItem = null;
				_hasItem = false;
				if (currentLootItem.useSounds != null)
					playerSound.PlayOneShot(currentLootItem.useSounds);
			}
		}

		if (_canEnterTram)
		{
			if (!_isInsideTram)
			{
				if (Input.GetButtonDown("Jump"))
				{
					_isInsideTram = true;

					controller.JumpOnToTram(tram, () =>
					{
						tram.PlayerEnter(this);
					});

					_canEnterTram = false;
				}
			}
		}

		if (Input.GetButtonDown("Pickup"))
		{

			if(!_hasItem)
			{
				if(lastTouchedLootItem != null)
				{
					if(Vector3.Distance(lastTouchedLootItem.transform.position, transform.position) < pickupDistance)
					{
						lastTouchedLootItem.SendMessage("Collect", this);
					}
				}
			}
			else
			{
				currentLootItem.transform.SetParent(null);
				currentLootItem.transform.position = new Vector3
				(currentLootItem.transform.position.x,
				0f,
				currentLootItem.transform.position.z);
				_hasItem = false;
				if(lastTouchedLootItem != currentLootItem.gameObject && Vector3.Distance(lastTouchedLootItem.transform.position, transform.position) < pickupDistance)
				{
					lastTouchedLootItem.SendMessage("Collect", this);
				}
		
			}
			
		}

		if (_takeDamage)
		{
			Hit(5f * Time.deltaTime);
		}
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
		if (_isHidden)
		{
			return;
		}

		var tram = GameManager.Instance.CurrentTram;

		if (collider == tram.entryTrigger)
		{
			_canEnterTram = true;
		}

		if (collider == tram.exitTrigger)
		{
			if (_isInsideTram)
			{
				_isInsideTram = false;
				
				transform.parent = null;

				tram.PlayerLeave(this);
			}
		}

		if (collider.gameObject.tag == "Loot")
		{
			lastTouchedLootItem = collider.gameObject;
		}

		if (collider.gameObject.tag == "Hazard")
		{
			_takeDamage = true;
		}

		if (collider.gameObject.tag == "OneHit")
		{
			collider.gameObject.SendMessage("HitPlayer", this);
		}

		if (collider == tram.engineCollider)
		{
			_canFillEngine = true;
		}
 	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		if (_isHidden)
		{
			return;
		}

		var tram = GameManager.Instance.CurrentTram;

		if (collider == tram.entryTrigger)
		{
			_canEnterTram = false;
		}

		if (collider == tram.engineCollider)
		{
			_canFillEngine = false;
		}

		if(collider.gameObject.tag == "Hazard")
		{
			_takeDamage = false;
		}
 	}

}