using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public PlayerControl controller;

	private bool _isInsideTram;
	private bool _canEnterExitTram;
	public bool _hasItem;
	public GameObject itemSlot;
	private  LootItem currentLootItem;
	private  GameObject lastTouchedLootItem;
	private  float pickupDistance = 1f;
	public float Hunger = 100f;
	public float Health = 100f;

	public void CollectLoot(LootItem lootItem)
	{
		lootItem.transform.parent = itemSlot.transform;
		lootItem.transform.localPosition = new Vector3 (0,0,0);
		currentLootItem = lootItem;
		_hasItem = true;
	}

	public void Feed(float food)
	{
		Hunger += food;
		Hunger = Mathf.Clamp(Hunger,0, 100f);
	}

	protected virtual void Update()
	{
		if (Input.GetButtonDown("Use"))
		{
			if(_hasItem)
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
		
 	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		var tram = GameManager.Instance.CurrentTram;

		if (_isInsideTram && collider == tram.enterExitTrigger)
		{
			_canEnterExitTram = true;
		}
 	}

}