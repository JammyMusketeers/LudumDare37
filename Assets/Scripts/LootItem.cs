using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LootItem : MonoBehaviour
{
	public string itemName;
	public ResourceType resourceType;
	public RarityType rarityType;
	public int spawnChance = 50;
	public int fillAmount;
	public bool canUseAnywhere;
	public AudioClip collectSound;
	public AudioClip useSounds;
	public Image icon;

	public void Collect(Player player)
	{
		player.CollectLoot(this);
	}

	public void Use(Player player)
	{
		if (resourceType == ResourceType.HUNGER)
		{	
			player.Feed(fillAmount);
			Destroy(gameObject);
		}

		if (resourceType == ResourceType.HEALTH)
		{	
			player.Heal(fillAmount);
			Destroy(gameObject);
		}
	}

	public void UseEngine (Tram tram)
	{
		if (resourceType == ResourceType.FUEL)
		{	
			tram.Fill(fillAmount);
			Destroy(gameObject);
		}
	}
}