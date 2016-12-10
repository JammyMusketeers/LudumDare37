using UnityEngine;
using System.Collections;

public class Tram : MonoBehaviour
{
	public float minMoveSpeed = 0f;
	public float maxMoveSpeed = 5f;
	public float moveSpeed = 1f;
	public Vector3 moveVector;
	public GameObject exterior;
	public Collider enterExitTrigger;
	public Collider[] interiorColliders;
	public Transform insideSpawn;
	public Transform outsideSpawn;

	public virtual void PlayerEnter(Player player)
	{
		exterior.GetComponent<Collider>().enabled = false;
		exterior.GetComponent<Renderer>().enabled = false;
	}

	public virtual void PlayerLeave(Player player)
	{
		exterior.GetComponent<Collider>().enabled = true;
		exterior.GetComponent<Renderer>().enabled = true;
	}

	protected virtual void Update()
	{
		var position = transform.position;

		position += (moveVector * Time.deltaTime);

		transform.position = position;
	}

	protected virtual void Start()
	{
		GameManager.Instance.CurrentTram = this;
	}
}