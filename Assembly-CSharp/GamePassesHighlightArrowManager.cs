using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class GamePassesHighlightArrowManager : MonoBehaviour
{
	[SerializeField]
	private GameObject highlightArrowPrefab;

	[SerializeField]
	private List<Transform> progressBarTransfromsList;

	private GameObject highLightArrow;

	private GamePassTier currentGamePassTierHighlighted;

	private static bool isHighlightingTierUnlocked;

	private static GamePassTier highestTierRewardShown;

	private static bool shouldDestroyHighlighArrow;

	public static void IncreaseHighestTierRewardShown()
	{
		highestTierRewardShown++;
		shouldDestroyHighlighArrow = true;
	}

	public void OnTierBeingShown(GamePassTier tierBeingShown)
	{
		if (tierBeingShown == currentGamePassTierHighlighted)
		{
			DestroyHighlighArrow();
			if (isHighlightingTierUnlocked)
			{
				highestTierRewardShown = tierBeingShown;
				isHighlightingTierUnlocked = false;
			}
		}
	}

	private void Start()
	{
		if (GamePassesManager.GamePassesActive && highestTierRewardShown == GamePassTier.Tier0 && !isHighlightingTierUnlocked)
		{
			highestTierRewardShown = GamePassesManager.PlayerPlanetData.gamePassTier;
		}
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		if (isHighlightingTierUnlocked)
		{
			HandleUnseenTierUnlockReward();
		}
	}

	private void OnEnable()
	{
		if (shouldDestroyHighlighArrow)
		{
			DestroyHighlighArrow();
			shouldDestroyHighlighArrow = false;
		}
	}

	private void OnPlayerPlanetDataUpdated()
	{
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit)
		{
			HandleUnseenTierUnlockReward();
		}
	}

	private void HandleUnseenTierUnlockReward()
	{
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		if ((int)highestTierRewardShown < (int)gamePassTier)
		{
			DestroyHighlighArrow();
			CreateHighlightArrow(gamePassTier);
			isHighlightingTierUnlocked = true;
		}
	}

	private void DestroyHighlighArrow()
	{
		if (!(highLightArrow == null))
		{
			UnityEngine.Object.Destroy(highLightArrow);
			highLightArrow = null;
			currentGamePassTierHighlighted = GamePassTier.Tier0;
		}
	}

	private void CreateHighlightArrow(GamePassTier gamePassTierToHighlight)
	{
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit)
		{
			highLightArrow = UnityEngine.Object.Instantiate(highlightArrowPrefab);
			highLightArrow.transform.SetParent(progressBarTransfromsList[(int)(gamePassTierToHighlight - 1)], worldPositionStays: false);
			currentGamePassTierHighlighted = gamePassTierToHighlight;
		}
	}
}
