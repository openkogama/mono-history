using System.Collections.Generic;
using UnityEngine;

public class GameMeterHandler : MonoBehaviour
{
	[SerializeField]
	private List<GameMeterAndroidBase> gameMeters;

	private void Update()
	{
		for (int i = 0; i < gameMeters.Count; i++)
		{
			gameMeters[i].UpdateShowGameMeter();
		}
	}
}
