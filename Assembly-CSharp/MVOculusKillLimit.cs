using System;
using System.Collections.Generic;

public class MVOculusKillLimit : MVLogicObject
{
	private bool initializedInWorld;

	private int KillLimit => (int)Data["killLimit"];

	public MVOculusKillLimit(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVOculusKillLimitPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
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
			throw new Exception("Couldn't find killLimitClient winning condition.");
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
				throw new Exception("Couldn't find killLimitClient winning condition.");
			}
			MVGameControllerBase.Game.WinningConditionManager.RemoveWinnerCondition(singletonWinnerConditionByType.ID);
		}
	}
}
