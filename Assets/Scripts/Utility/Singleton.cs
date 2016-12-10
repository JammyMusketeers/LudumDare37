using UnityEngine;

public abstract class Singleton<T> : Singleton where T : MonoBehaviour
{
	private static bool _didSetup;

	public static T Instance
	{
		get
		{
			CheckInstance();

			return instance;
		}
	}

	private static void CheckInstance()
	{
		if (_didSetup || instance != null)
		{
			return;
		}

		instance = Component.FindObjectOfType<T>();

		var singleton = (instance as Singleton<T>);

		if (singleton != null)
		{
			singleton.Setup();
		}
	}

	private static T instance = default(T);

	protected override void OnSetup() {}
	
	public override void Setup()
	{
		if (!_didSetup)
		{
			if (instance != this)
			{
				instance = this as T;
			}

			_didSetup = true;

			OnSetup();
		}
	}

	public override void Clear()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}

public abstract class Singleton : MonoBehaviour
{
	public abstract void Setup();
	public abstract void Clear();

	protected virtual void Awake()
	{
		GameObject.DontDestroyOnLoad(transform.root.gameObject);

		Setup();
	}

	protected virtual void OnDestroy()
	{
		Clear();
	}

	protected virtual void OnSetup() {}
}