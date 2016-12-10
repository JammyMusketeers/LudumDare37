using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public PlayerControl controller;

	public bool IsTramCollision(Collision collision)
	{
		var tram = collision.gameObject.GetComponentInParent<Tram>();

		if (tram != null && tram == GameManager.Instance.CurrentTram)
		{
			return true;
		}

		return false;
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		if (IsTramCollision(collision))
		{
			transform.parent = collision.gameObject.transform.parent;
		}
 	}

	protected virtual void OnCollisionExit(Collision collision)
	{
		if (IsTramCollision(collision))
		{
			transform.parent = null;
		}
 	}
}
