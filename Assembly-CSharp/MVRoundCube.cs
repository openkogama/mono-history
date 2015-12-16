using System;
using System.Collections.Generic;

public class MVRoundCube : MVLogicObject
{
	private bool initializedInWorld;

	private GameStatCounterType WinningCondition => (GameStatCounterType)(int)Data["winningCondition"];

	public int DurationInMilliseconds => (int)Data["interval"] * 1000;

	public MVRoundCube(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVRoundCubePrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags &= ~InteractionFlags.CanClone;
	}

	public override void Initialize()
	{
		base.Initialize();
		MVWorldObjectClientManager wOCM = MVGameControllerBase.WOCM;
		wOCM.OnResetWorldDone = (EventHandler<EventArgs>)Delegate.Combine(wOCM.OnResetWorldDone, new EventHandler<EventArgs>(OnResetWorldDone));
		MVGameControllerBase.Game.WinningConditionManager.CreateWinnerCondition<TimeLimitClient>(new object[1] { WinningCondition });
		initializedInWorld = true;
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
			MVWorldObjectClientManager wOCM = MVGameControllerBase.WOCM;
			wOCM.OnResetWorldDone = (EventHandler<EventArgs>)Delegate.Remove(wOCM.OnResetWorldDone, new EventHandler<EventArgs>(OnResetWorldDone));
			TimeLimitClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<TimeLimitClient>();
			if (singletonWinnerConditionByType == null)
			{
				throw new Exception("Couldn't find TimeLimit winning condition.");
			}
			MVGameControllerBase.Game.WinningConditionManager.RemoveWinnerCondition(singletonWinnerConditionByType.ID);
		}
	}

	private void OnResetWorldDone(object sender, EventArgs e)
	{
		TimeLimitClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<TimeLimitClient>();
		if (singletonWinnerConditionByType == null)
		{
			throw new Exception("Couldn't find TimeLimit winning condition.");
		}
		singletonWinnerConditionByType.CounterType = WinningCondition;
	}
}
