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
	public Image bloodSplatter;
	public InventoryUI inventory;
	public Image tramLocation;


	private float fuelBarFullSize;
	private Vector2 healthDisplaySize;
	private bool _isInputLocked;
	private float _bloodSplatterAlpha;
	private float _tramLocationAlpha;

	public void Reset()
	{
		_bloodSplatterAlpha = 0f;

		var bloodColor = bloodSplatter.color;
			bloodColor.a = 0f;
		bloodSplatter.color = bloodColor;

		_isInputLocked = false;
	}

	public void DoBloodSplatter()
	{
		if (_bloodSplatterAlpha == 0f && bloodSplatter.color.a < 0.1f)
		{
			_bloodSplatterAlpha = 0.35f;
		}
	}

	protected override void OnSetup()
	{
		hungerBarFullSize = hungerAmount.rect.width;
		fuelBarFullSize = fuelAmount.rect.height;
		healthDisplaySize = new Vector2(healthAmount.rect.width, healthAmount.rect.height);
	}

	protected virtual void Update()
	{
		var bloodColor = bloodSplatter.color;

		bloodColor.a = Mathf.Lerp(bloodColor.a, _bloodSplatterAlpha, Time.deltaTime * 8f);

		bloodSplatter.color = bloodColor;

		if (_bloodSplatterAlpha > 0f && bloodColor.a >= (_bloodSplatterAlpha * 0.8f))
		{
			_bloodSplatterAlpha = 0f;
		}

		var tram = GameManager.Instance.CurrentTram;

		if (tram != null)
		{
			var tramLocationColor = tramLocation.color;
				tramLocationColor.a = Mathf.Lerp(tramLocationColor.a, _tramLocationAlpha, Time.deltaTime * 8f);
			tramLocation.color = tramLocationColor;

			var screenPos = Camera.main.WorldToViewportPoint(tram.transform.position);

			if (screenPos.x < 0f || screenPos.x > 1f || screenPos.y < 0f || screenPos.y > 1f)
			{
				_tramLocationAlpha = 1f;

				screenPos.x = Mathf.Clamp(screenPos.x, 0.05f, 0.95f);
				screenPos.y = Mathf.Clamp(screenPos.y, 0.05f, 0.95f);

				tramLocation.rectTransform.anchorMin = screenPos;
				tramLocation.rectTransform.anchorMax = screenPos;
			}
			else
			{
				_tramLocationAlpha = 0f;
			}
		}
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
