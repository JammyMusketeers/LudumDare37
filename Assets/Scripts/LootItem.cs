using UnityEngine;
using System.Collections;

public class LootItem : MonoBehaviour
{
	public string itemName;
	public ResourceType resourceType;
	public RarityType rarityType;
	public int spawnChance = 50;
	public int fillAmount;

	public void Collect (Player player)
	{

		player.CollectLoot(this);

	}

	public void Use (Player player)
	{
		if (resourceType == ResourceType.HUNGER)
		{	
			player.Feed(fillAmount);
			Destroy(gameObject);
		}
	}
}