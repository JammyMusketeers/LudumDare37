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
	public Image healthAmount;
	public RectTransform hungerAmount;
	public Text fuelConsumpAmount;
	public RectTransform fuelAmount;

	void Update()
	{
		SetDistance(Time.timeSinceLevelLoad);
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
}
