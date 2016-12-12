using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : Singleton<UIManager>
{
	public Text speedAmount;
	public float speedSigFigs = 2;
	public string speedUnits = "km/h";
	public Text distanceAmount;
	public string distanceUnits = "meters";
	public float distanceChangeover = 1000f;
	public string distanceThousands = "km";
	public RectTransform healthAmount;
	public RectTransform hungerAmount;
	private float hungerBarFullSize;
	public Text fuelConsumpAmount;
	public float fuelConsumpSigFigs = 1;
	public RectTransform fuelAmount;

	public InventoryUI inventory;


	private float fuelBarFullSize;
	private Vector2 healthDisplaySize;
	private bool _isInputLocked;

	public void Reset()
	{

	}

	protected override void OnSetup()
	{
		hungerBarFullSize = hungerAmount.rect.width;
		fuelBarFullSize = fuelAmount.rect.height;
		healthDisplaySize = new Vector2(healthAmount.rect.width, healthAmount.rect.height);
	}

	protected virtual void Update()
	{
		
	}

	public bool IsInputLocked()
	{
		return _isInputLocked;
	}

	public void LockInput(bool isInputLocked)
	{
		_isInputLocked = isInputLocked;
	}

	public void SetSpeed(float newValue)
	{
		float divisor = Mathf.Pow(10,speedSigFigs);
		newValue = newValue * 1000f / 360f;
		float roundVal = Mathf.Round(newValue  * divisor);

		speedAmount.text = (roundVal / divisor) + " " + speedUnits;
	}

	public void SetDistance(float newValue)
	{
		float meters = Mathf.Round(newValue);

		if (meters > distanceChangeover)
		{
			// convert to km
			float kilometers = meters / distanceChangeover;
			distanceAmount.text = kilometers +" "+ distanceThousands;
		}
		else
		{
			// show meters
			distanceAmount.text = meters +" "+ distanceUnits;
		}
	}

	public void SetHealth(float newValue)
	{
		healthAmount.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newValue * healthDisplaySize.x);
		healthAmount.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newValue * healthDisplaySize.y);
	}

	public void SetHunger(float newValue)
	{
		hungerAmount.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newValue * hungerBarFullSize);
	}

	public void SetFuelConsumption(float newValue)
	{
		float divisor = Mathf.Pow(10,fuelConsumpSigFigs);
		float roundVal = Mathf.Round(newValue * divisor);

		fuelConsumpAmount.text = "-" + (roundVal / divisor);
	}

	public void SetFuel(float newValue)
	{
		fuelAmount.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newValue * fuelBarFullSize);
	}

	public void ShowInventoryPanel(bool toggle)
	{
		inventory.gameObject.SetActive(toggle);

		if (toggle)
		{
			inventory.SelectFirst();
		}
	}
}
