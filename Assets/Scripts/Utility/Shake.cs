using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour {

	public bool shakePosition;
	public Vector3 positionVariation;

	public bool shakeRotation;
	public Vector3 angleVariation;

	public float rate = 0.2f;
	private float rateCounter;
	private float rotProgreess;

	private Vector3 _originalLocalPosition;
	private Vector3 _originalLocalAngles;

	void Update () {
		if (shakePosition)
		{
			Vector3 randomOffset = new Vector3(Random.Range(-positionVariation.x,positionVariation.x),
								Random.Range(-positionVariation.x,positionVariation.y),
								Random.Range(-positionVariation.x,positionVariation.z));
			transform.localPosition = Vector3.Lerp(transform.localPosition, _originalLocalPosition + randomOffset, Time.deltaTime);
		}

		if (shakeRotation)
		{/* too tired
			if (rateCounter <= rate)
			{
				//me start new lerp
				rateCounter += Time.deltaTime;
			}
			else
			{
				rateCounter = 0;
				rotProgreess += Time.deltaTime;
			}
			// new rotation

		

			Vector3 randomRotation = new Vector3(Random.Range(-angleVariation.x,angleVariation.x),
								Random.Range(-angleVariation.x,angleVariation.y),
								Random.Range(-angleVariation.x,angleVariation.z));
			transform.localEulerAngles = new Vector3 (Mathf.LerpAngle(randomRotation.x, transform.localEulerAngles.x, rotProgress),
								Mathf.LerpAngle(randomRotation.x, transform.localEulerAngles.x, rotProgreess),
								Mathf.LerpAngle(randomRotation.x, transform.localEulerAngles.x, rotProgreess))
		*/}
	}
}
