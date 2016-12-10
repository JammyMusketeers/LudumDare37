using UnityEngine;
using System.Collections;

public class IsometricCamera : MonoBehaviour
{
	public Player player;
	public Vector3 distance = new Vector3(10f, 10f, 10f);
	public float angle = 45f;

	private Vector3? _lastPosition;
	private Quaternion? _lastRotation;

	protected virtual void Update()
	{
		transform.position = player.gameObject.transform.position + distance;
		transform.LookAt(player.gameObject.transform);

		var rotation = transform.eulerAngles;
			rotation.y = angle;
		transform.eulerAngles = rotation;

		/*
		if (_lastPosition == null)
		{
			_lastPosition = transform.position;
		}

		if (_lastRotation == null)
		{
			_lastRotation = transform.rotation;
		}

		transform.position = Vector3.Lerp(_lastPosition.Value, transform.position, 16 * Time.deltaTime);
		transform.rotation = Quaternion.Lerp(_lastRotation.Value, transform.rotation, 16 * Time.deltaTime);

		_lastRotation = transform.rotation;
		_lastPosition = transform.position;
		*/
	}
}