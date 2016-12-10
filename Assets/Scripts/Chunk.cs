using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk
{
	public int x;
	public int z;

	private Ground _ground;

	private List<GameObject> _children;

	private float _clearTime;

	public float GetClearTime()
	{
		return _clearTime;
	}

	public void SetClearTime(float clearTime)
	{
		_clearTime = Time.time + clearTime;
	}

	public void AddGround()
	{
		_ground = GameObject.Instantiate(GameManager.Instance.groundPrefab);
		_ground.SetSize(GameManager.Instance.chunkSize);
		_ground.SetPosition(x, z);
	}

	public Ground GetGround()
	{
		return _ground;
	}

	public void AddChild(GameObject child)
	{
		_children.Add(child);
	}

	public void RemoveChild(GameObject child)
	{
		_children.Remove(child);
	}

	public void RemoveChildren()
	{
		GameObject.Destroy(_ground.gameObject);

		foreach (var child in _children)
		{
			GameObject.Destroy(child);
		}
	}

	public Chunk()
	{
		_children = new List<GameObject>();
	}
}
