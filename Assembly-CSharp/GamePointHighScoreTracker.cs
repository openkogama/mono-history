using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePointHighScoreTracker : MonoBehaviour
{
	[SerializeField]
	private Text gamePointHighScoreAmountText;

	private void Start()
	{
		UpdateHighScoreText();
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
	}

	private void OnDestroy()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
	}

	private void UpdateHighScoreText()
	{
		if (GamePassesManager.GamePassesActive)
		{
			gamePointHighScoreAmountText.text = GamePassesManager.PlayerPlanetData.highScoreGamePoints.ToString();
		}
	}

	private void OnPlayerPlanetDataUpdated()
	{
		UpdateHighScoreText();
	}
}
