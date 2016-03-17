using System.Collections.Generic;
using UnityEngine;

public class GameMeterHandler : MonoBehaviour
{
	[SerializeField]
	private GameMeterAndroidHealth healthBar;

	[SerializeField]
	private GameMeterAndroidGameCoin gameCoinBar;

	[SerializeField]
	private GameMeterAndroidCollectible collectiblesBar;

	[SerializeField]
	private GameMeterAndroidKillLimit killBar;

	[SerializeField]
	private GameMeterAndroidOculus oculusBar;

	[SerializeField]
	private GameMeterAndroidRoundTime timeBar;

	[SerializeField]
	private GameMeterAndroidFlag flagBar;

	private List<GameMeterAndroidBase> gameMeters = new List<GameMeterAndroidBase>();

	private void Awake()
	{
		gameMeters.Add(healthBar);
		gameMeters.Add(gameCoinBar);
		gameMeters.Add(collectiblesBar);
		gameMeters.Add(killBar);
		gameMeters.Add(oculusBar);
		gameMeters.Add(timeBar);
		gameMeters.Add(flagBar);
	}

	private void Update()
	{
		for (int i = 0; i < gameMeters.Count; i++)
		{
			gameMeters[i].UpdateShowGameMeter();
		}
	}
}
