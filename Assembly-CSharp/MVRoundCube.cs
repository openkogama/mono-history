using System;
using System.Collections.Generic;
using UnityEngine;

public class MVRoundCube : MVLogicObject
{
	private bool initializedInWorld;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.RoundCube;

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
		MVGameControllerBase.Game.WinningConditionManager.CreateWinnerCondition<TimeLimitClient>(new object[0]);
		initializedInWorld = true;
		SetupCulling(gameObject);
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

	public int GetTimeLeft()
	{
		int num = DurationInMilliseconds - (MVGameControllerBase.Game.ServerTimeInMilliSeconds - MVGameControllerBase.Game.NetworkGameStateListener.StartTime);
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public string MakeTimeIntoText(int time)
	{
		string text = string.Empty;
		time = (int)((float)time / 1000f);
		int num = time % 60;
		int num2 = Mathf.FloorToInt((float)time / 60f);
		if (num2 >= 60)
		{
			int num3 = Mathf.FloorToInt((float)num2 / 60f);
			num2 %= 60;
			text = text + num3 + ":";
		}
		string text2 = string.Empty;
		if (num < 10)
		{
			text2 += "0";
		}
		text2 += num;
		string text3 = string.Empty;
		if (num2 < 10)
		{
			text3 += "0";
		}
		text3 += num2;
		return text + text3 + ":" + text2;
	}

	private void OnResetWorldDone(object sender, EventArgs e)
	{
		TimeLimitClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<TimeLimitClient>();
		if (singletonWinnerConditionByType == null)
		{
			throw new Exception("Couldn't find TimeLimit winning condition.");
		}
	}
}
