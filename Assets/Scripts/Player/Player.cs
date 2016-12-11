﻿using UnityEngine;
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
		health = Mathf.Clamp(health,0, 100f);
	}

	public bool IsInsideTram()
	{
		return _isInsideTram;
	}

	public bool CanEnterTram()
	{
		return _canEnterTram;
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

	protected virtual void Update()
	{
		var tram = GameManager.Instance.CurrentTram;

		if (tram.IsBeingOperated())
		{

		}
		
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
			}
			
			if(_hasItem && currentLootItem.canUseAnywhere)
			{
				currentLootItem.SendMessage("Use", this);
				currentLootItem = null;
				_hasItem = false;
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

		if(_takeDamage)
		{
			health -= 3f * Time.deltaTime;
		}
		
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
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