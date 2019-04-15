using System;
using UnityEngine;

public class GamePassesUIEnabler : MonoBehaviour
{
	private void Start()
	{
		gameObject.SetActive(GamePassProgressionController.IsProgressionEnabled);
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Combine(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(OnGamePassProgression));
	}

	private void OnDestroy()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnGamePassProgression));
	}

	private void OnGamePassProgression()
	{
		gameObject.SetActive(GamePassProgressionController.IsProgressionEnabled);
	}
}
