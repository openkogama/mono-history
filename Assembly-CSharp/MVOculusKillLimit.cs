using System;
using System.Collections.Generic;

public class MVOculusKillLimit : MVGamePointRewardLogicObject
{
	private bool initializedInWorld;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.OculusKillWinCondition;

	protected override bool HasVisualsInPlaymode => false;

	private int KillLimit => (int)Data["killLimit"];

	protected override int GamePointRewardAmount => GetGamePointsRewardAmount(Data) * KillLimit;

	public MVOculusKillLimit(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVOculusKillLimitPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanEarnGamePointsMinor;
		interactionFlags &= ~InteractionFlags.CanClone;
	}

	public override void Initialize()
	{
		base.Initialize();
		MVGameControllerBase.Game.WinningConditionManager.CreateWinnerCondition<OculusKillLimitClient>(new object[1] { KillLimit });
		initializedInWorld = true;
	}

	public override bool IsSingletonObject()
	{
		return true;
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		OculusKillLimitClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>();
		if (singletonWinnerConditionByType == null)
		{
			throw new Exception("Couldn't find OculusKillLimitClient winning condition.");
		}
		singletonWinnerConditionByType.SetLimit(KillLimit);
	}

	public override void Destroy()
	{
		base.Destroy();
		if (initializedInWorld)
		{
			OculusKillLimitClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>();
			if (singletonWinnerConditionByType == null)
			{
				throw new Exception("Couldn't find OculusKillLimitClient winning condition.");
			}
			MVGameControllerBase.Game.WinningConditionManager.RemoveWinnerCondition(singletonWinnerConditionByType.ID);
		}
	}
}
