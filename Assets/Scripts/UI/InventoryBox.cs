using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryBox : MonoBehaviour
{
	public int numberOfSlots;
	public LootItem[] contents;
	public Image[] contentIcons;

	public UIManager ui;

	private bool _isOpen;

	public bool IsOpen()
	{
		return _isOpen;
	}

	public void Reset()
	{
		for (int i = 0; i < contents.Length; i++)
		{
			if (contents[i])
			{
				Destroy(contents[i]);
			}

			contents[i] = null;
		}

		foreach (var item in ui.inventory.items)
		{
			item.icon.sprite = null;
		}

		ui.ShowInventoryPanel(false);

		_isOpen = false;
	}

	public void Open()
	{
		if (!_isOpen)
		{
			_isOpen = true;

			ui.ShowInventoryPanel(true);
			ui.LockInput(true);
		}
		else
		{
			Debug.LogWarning("Tried to open inventory but it's already open.");
		}
	}

	public void Close()
	{
		if (_isOpen)
		{
			_isOpen = false;

			ui.ShowInventoryPanel(false);
			ui.LockInput(false);
		}
		else
		{
			Debug.LogWarning("Tried to close inventory but it's already closed.");
		}
	}

	public LootItem InsertItem(int slotId, LootItem newItem)
	{
		var existingItem = (LootItem)null;

		if (contents[slotId] != null)
		{
			existingItem = contents[slotId];
			existingItem.gameObject.transform.SetParent(null);
			existingItem.gameObject.SetActive(true); 
		}

		contents[slotId] = newItem;

		newItem.gameObject.transform.SetParent(transform);
		newItem.gameObject.SetActive(false);

		ui.inventory.SetItem(slotId, newItem);

		return existingItem;
	}

	public LootItem RetrieveItem(int slotId)
	{
		var returnItem = (LootItem)null;

		if (contents[slotId] == null)
		{
			return null;
		}
		else
		{
			returnItem = contents[slotId];
			returnItem.gameObject.transform.SetParent(null);
			returnItem.gameObject.SetActive(true); 

			ui.inventory.SetItem(slotId, null);

			contents[slotId] = null;

			return returnItem;
		}
	}

	public int GetFreeSlot()
	{
		for (int i = 0; i < contents.Length; i++)
		{
			if (contents[i] == null)
			{
				return i;
			}
		}

		return -1;
	}

	public int CountUsedSpace()
	{
		var contentsCount = 0;

		for (int i = 0; i < contents.Length; i++)
		{
			if (contents[i] != null)
			{
				contentsCount++;
			}
		}

		return contentsCount;
	}

	public int CountFreeSpace()
	{
		var contentsCount = contents.Length;

		for (int i = 0; i < contents.Length; i++)
		{
			if (contents[i] != null)
			{
				contentsCount--;
			}
		}

		return contentsCount;
	}

	private void Awake() 
	{
		contents = new LootItem[numberOfSlots];
	}
}
