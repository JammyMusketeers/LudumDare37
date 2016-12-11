using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public PlayerControl controller;

	private bool _isInsideTram;
	private bool _canEnterExitTram;
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

	public void CollectLoot(LootItem lootItem)
	{
		lootItem.transform.parent = itemSlot.transform;
		lootItem.transform.localPosition = new Vector3 (0,0,0);
		currentLootItem = lootItem;
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

	protected virtual void Update()
	{
		if (Input.GetButtonDown("Use"))
		{
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


			if (_canEnterExitTram)
			{
				var tram = GameManager.Instance.CurrentTram;

				if (_isInsideTram)
				{
					GameManager.Instance.FadeToBlack(() =>
					{
						_isInsideTram = false;
						transform.parent = null;

						transform.position = tram.outsideSpawn.position;
						transform.rotation = tram.outsideSpawn.rotation;

						controller.SetPosition(transform.position);
						controller.SetRotation(transform.rotation);

						tram.PlayerLeave(this);

						GameManager.Instance.FadeFromBlack();
					});
				}
				else
				{
					GameManager.Instance.FadeToBlack(() =>
					{
						_isInsideTram = true;

						transform.parent = tram.transform;
						transform.position = tram.insideSpawn.position;
						transform.rotation = tram.insideSpawn.rotation;

						controller.SetPosition(transform.position);
						controller.SetRotation(transform.rotation);

						tram.PlayerEnter(this);

						GameManager.Instance.FadeFromBlack();
					});
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

		if (!_isInsideTram && collider == tram.enterExitTrigger)
		{
			_canEnterExitTram = true;
		}

		if(collider.gameObject.tag == "Loot")
		{
			lastTouchedLootItem = collider.gameObject;
		}

		if(collider.gameObject.tag == "Hazard")
		{
			_takeDamage = true;
		}

		if(collider == tram.engineCollider)
		{
			_canFillEngine = true;
			_canEnterExitTram = false;
		}
		
 	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		var tram = GameManager.Instance.CurrentTram;

		if (_isInsideTram && collider == tram.enterExitTrigger)
		{
			_canEnterExitTram = true;
		}

		if(collider == tram.engineCollider)
		{
			_canFillEngine = false;
			_canEnterExitTram = true;
		}

		if(collider.gameObject.tag == "Hazard")
		{
			_takeDamage = false;
		}
 	}

}