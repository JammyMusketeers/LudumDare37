using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryBox : MonoBehaviour {

	public int numberOfSlots;
	public LootItem[] contents;
	public Image[] contentIcons;

	private bool open;

	public void Open() {
		if (!open)
		{
			open = true;
		}
		else
		{
			Debug.LogWarning("Tried to open inventory but it's already open.");
		}
	}

	public void Close() {
		if (open)
		{
			open = false;
		}
		else
		{
			Debug.LogWarning("Tried to close inventory but it's already closed.");
		}
	}

	public void InsertItem(int slotid, LootItem newItem)
	{
		if (contents[slotid] != null) {
			// OCCUPIED! swap items eventually

		}
		else
		{
			contents[slotid] = newItem;
			newItem.gameObject.transform.SetParent(transform);
			newItem.gameObject.SetActive(false); 
		}
	}

	public LootItem RetrieveItem(int slotid)
	{
		LootItem returnItem;
		if (contents[slotid] == null) {
			// Empty!
			return null;
		}
		else
		{
			returnItem = contents[slotid];
			returnItem.gameObject.transform.SetParent(null);
			returnItem.gameObject.SetActive(true); 

			contents[slotid] = null;	// clear slot

			return returnItem;
		}
	}

	public int CountFreeSpace()
	{
		int contentsCount = 0;
		for (int i = 0; i < contents.Length; i++)
		{
			if (contents[i] != null) {
				contentsCount ++;
			}
		}

		return contentsCount;
	}

	private void Awake() 
	{
		contents = new LootItem[numberOfSlots];
	}

}
