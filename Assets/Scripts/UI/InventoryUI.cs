using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
	public InventoryBox storage;
	public InventoryItem[] items;

	private float _nextMoveItem;

	private int _selectedIndex;

	public void SetItem(int slotId, LootItem lootItem)
	{
		if (lootItem != null)
		{
			items[slotId].icon.sprite = lootItem.icon;
		}
		else
		{
			items[slotId].icon = null;
		}
	}

	public void SelectFirst()
	{
		var item = items[_selectedIndex];

		if (item.highlight)
		{
			item.highlight.SetActive(false);
		}

		_selectedIndex = 0;

		item = items[_selectedIndex];

		if (item.highlight)
		{
			item.highlight.SetActive(true);
		}
	}

	public void SelectNext()
	{
		var item = items[_selectedIndex];

		if (item.highlight)
		{
			item.highlight.SetActive(false);
		}

		_selectedIndex++;

		if (_selectedIndex >= items.Length)
		{
			_selectedIndex = 0;
		}

		item = items[_selectedIndex];

		if (item.highlight)
		{
			item.highlight.SetActive(true);
		}
	}

	public void SelectPrev()
	{
		var item = items[_selectedIndex];

		if (item.highlight)
		{
			item.highlight.SetActive(false);
		}

		_selectedIndex--;

		if (_selectedIndex < 0)
		{
			_selectedIndex = items.Length - 1;
		}

		item = items[_selectedIndex];

		if (item.highlight)
		{
			item.highlight.SetActive(true);
		}
	}

	protected void UseSlot(int slotId)
	{
		var player = GameManager.Instance.CurrentPlayer;
		var item = storage.RetrieveItem(slotId);

		if (item != null)
		{
			player.CollectLoot(item);

			storage.Close();
		}
	}

	protected virtual void Awake()
	{
		for (int i = 0; i < items.Length; i++)
		{
			var index = i;
			var item = items[i];

			item.button.onClick.AddListener(() =>
			{
				UseSlot(index);
			});

			item.highlight.SetActive(false);
		}
	}

	protected virtual void Update()
	{
		if (gameObject.activeSelf && Time.time >= _nextMoveItem)
		{
			var horizontal = Input.GetAxis("Horizontal");

			if (horizontal > 0.1f)
			{
				_nextMoveItem = Time.time + 0.2f;

				SelectNext();
			}
			else if (horizontal < -0.1f)
			{
				_nextMoveItem = Time.time + 0.2f;

				SelectPrev();
			}
		}

		if (Input.GetButtonDown("Pickup"))
		{
			UseSlot(_selectedIndex);
		}
	}
}
