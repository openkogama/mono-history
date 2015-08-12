using System;
using UnityEngine;

public class MVGUIDeltaRewardCreator : UXGroup
{
	private const string _deltaRewardPrefab = "Prefabs/GUI/Leveling/DeltaReward";

	public override void Awake()
	{
		base.Awake();
		if (LevelingManager.IsInitialized)
		{
			OnLevelingInitialized();
		}
		else
		{
			LevelingManager.OnLevelingInitialized = (LevelingManager.OnlevelingInitializedDelegate)Delegate.Combine(LevelingManager.OnLevelingInitialized, new LevelingManager.OnlevelingInitializedDelegate(OnLevelingInitialized));
		}
	}

	private void OnLevelingInitialized()
	{
		MVLocalPlayer localPlayer = MVGameController.Game.LocalPlayer;
		localPlayer.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnXPProgressData));
	}

	private void OnXPProgressData(XPProgressData xpProgressData)
	{
		Create(xpProgressData.XPDelta);
	}

	private void Create(int amount)
	{
		Debug.LogWarning("Delta reward effect is disabled");
	}
}
