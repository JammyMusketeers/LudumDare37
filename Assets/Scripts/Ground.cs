using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
	public void SetSize(int size)
	{
		transform.localScale = new Vector3(size, size, 1f);
	}

	public void SetPosition(int x, int z)
	{
		var size = transform.localScale.x;

		transform.position = new Vector3(x * size, 0f, z * size);
	}
}
