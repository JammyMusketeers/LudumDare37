using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
	public Tram CurrentTram { get; set; }

	protected override void OnSetup()
	{
		
	}
}
