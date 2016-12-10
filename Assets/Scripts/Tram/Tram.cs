using UnityEngine;
using System.Collections;

public class Tram : MonoBehaviour
{
	public float minMoveSpeed = 0f;
	public float maxMoveSpeed = 5f;
	public float moveSpeed = 1f;

	protected virtual void Update()
	{
		var position = transform.position;

		position.x += moveSpeed * Time.deltaTime;
		position.z += moveSpeed * Time.deltaTime;

		transform.position = position;
	}

	protected virtual void Start()
	{
		GameManager.Instance.CurrentTram = this;
	}
}
