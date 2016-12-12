using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
	public InventoryBox storage;
	public InventoryItem[] items;

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
		}
	}
}
