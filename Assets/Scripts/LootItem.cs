using UnityEngine;
using System.Collections;

public class LootItem : MonoBehaviour
{
	public string itemName;
	public ResourceType resourceType;
	public RarityType rarityType;
	public int spawnChance = 50;
	public int fillAmount;
}
