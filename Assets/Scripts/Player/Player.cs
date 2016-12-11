using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public PlayerControl controller;

	private bool _isInsideTram;
	private bool _canEnterExitTram;

	protected virtual void Update()
	{
		if (Input.GetButtonDown("Use"))
		{
			if (_canEnterExitTram)
			{
				var tram = GameManager.Instance.CurrentTram;

				if (_isInsideTram)
				{
					GameManager.Instance.FadeToBlack(() =>
					{
						_isInsideTram = false;
						transform.parent = null;

						transform.position = tram.outsideSpawn.position;
						transform.rotation = tram.outsideSpawn.rotation;

						controller.SetPosition(transform.position);
						controller.SetRotation(transform.rotation);

						tram.PlayerLeave(this);

						GameManager.Instance.FadeFromBlack();
					});
				}
				else
				{
					GameManager.Instance.FadeToBlack(() =>
					{
						_isInsideTram = true;

						transform.parent = tram.transform;
						transform.position = tram.insideSpawn.position;
						transform.rotation = tram.insideSpawn.rotation;

						controller.SetPosition(transform.position);
						controller.SetRotation(transform.rotation);

						tram.PlayerEnter(this);

						GameManager.Instance.FadeFromBlack();
					});
				}
			}
		}
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
		var tram = GameManager.Instance.CurrentTram;

		if (!_isInsideTram && collider == tram.enterExitTrigger)
		{
			_canEnterExitTram = true;

		}
 	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		var tram = GameManager.Instance.CurrentTram;

		if (_isInsideTram && collider == tram.enterExitTrigger)
		{
			_canEnterExitTram = true;
		}
 	}
}