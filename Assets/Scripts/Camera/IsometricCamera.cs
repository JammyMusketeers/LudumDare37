using UnityEngine;
using System.Collections;

public class IsometricCamera : MonoBehaviour
{
	public Player player;
	public float distance = 10f;

	protected virtual void Update()
	{
		transform.position = player.gameObject.transform.position + new Vector3(-distance, distance, -distance);

		transform.LookAt(player.gameObject.transform);

		var rotation = transform.eulerAngles;

		rotation.y = 45f;

		transform.eulerAngles = rotation;
	}
}