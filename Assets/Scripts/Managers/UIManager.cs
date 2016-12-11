using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	public Text speedAmount;
	public float speedSigFigs = 2;
	public Text distanceAmount;
	public string distanceUnits = "meters";
	public float distanceChangeover = 1000f;
	public string distanceThousands = "km";
	public RectTransform healthAmount;
	public RectTransform hungerAmount;
	private float hungerBarFullSize;
	public Text fuelConsumpAmount;
	public RectTransform fuelAmount;
	private float fuelBarFullSize;

	void Awake()
	{
		hungerBarFullSize = hungerAmount.rect.width;
		fuelBarFullSize = fuelAmount.rect.width;
	}

	void Update()
	{
		//SetDistance(Time.timeSinceLevelLoad);
	}

	public void SetSpeed(float newValue)
	{
		float divisor = Mathf.Pow(10,speedSigFigs);
		float roundVal = Mathf.Round(newValue * divisor);

		speedAmount.text = "" + (roundVal / divisor);
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
		healthAmount.sizeDelta = new Vector2(newValue, newValue);
	}

	public void SetHunger(float newValue)
	{
		hungerAmount.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newValue * hungerBarFullSize);
	}

	public void SetFuelConsumption(float newValue)
	{
		fuelConsumpAmount.text = "" + newValue;
	}

	public void SetFuel(float newValue)
	{
		hungerAmount.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newValue * fuelBarFullSize);
	}
}
