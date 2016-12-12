using UnityEngine;
using System.Collections;

public class IsometricCamera : MonoBehaviour
{
	public Player player;
	public Vector3 distance = new Vector3(10f, 10f, 10f);
	public float angle = 45f;

	public float minViewSize = 6f;
	public float speedViewMultiplier = 100f;

	private Camera _camera;
	private Vector3? _lastPosition;
	private Quaternion? _lastRotation;
	public float movingSpeed;

	public void Reset()
	{
		_lastPosition = null;
	}

	protected virtual void Awake()
	{
		_camera = GetComponent<Camera>() as Camera;
	}

	protected virtual void Update()
	{
		transform.position = player.gameObject.transform.position + distance;
		transform.LookAt(player.gameObject.transform);

		var rotation = transform.eulerAngles;
			rotation.y = angle;
		transform.eulerAngles = rotation;

		// speed multiplier
		if (_lastPosition != null)
		{
			movingSpeed = Vector3.Distance(transform.position, _lastPosition.Value);
			movingSpeed *= movingSpeed;
			_camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, minViewSize + (speedViewMultiplier * movingSpeed), Time.smoothDeltaTime);
		}
		_lastPosition = transform.position;

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