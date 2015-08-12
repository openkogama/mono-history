using System;
using System.Collections.Generic;

public class MVKillLimit : MVLogicObject
{
	private const string prefabPath = "Prefabs/KillLimitCubeObject";

	private bool initializedInWorld;

	private int KillLimit => (int)Data["killLimit"];

	public MVKillLimit(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/KillLimitCubeObject", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags &= ~InteractionFlags.CanClone;
	}

	public override void Initialize()
	{
		base.Initialize();
		MVGameController.Game.WinningConditionManager.CreateWinnerCondition<KillLimitClient>(new object[1] { KillLimit });
		initializedInWorld = true;
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		KillLimitClient singletonWinnerConditionByType = MVGameController.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>();
		if (singletonWinnerConditionByType == null)
		{
			throw new Exception("Couldn't find killLimitClient winning condition.");
		}
		singletonWinnerConditionByType.SetLimit(KillLimit);
	}

	public override bool IsSingletonObject()
	{
		return true;
	}

	public override void Destroy()
	{
		base.Destroy();
		if (initializedInWorld)
		{
			KillLimitClient singletonWinnerConditionByType = MVGameController.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>();
			if (singletonWinnerConditionByType == null)
			{
				throw new Exception("Couldn't find killLimitClient winning condition.");
			}
			MVGameController.Game.WinningConditionManager.RemoveWinnerCondition(singletonWinnerConditionByType.ID);
		}
	}
}
