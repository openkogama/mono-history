using System;
using MV.Common;
using MV.WorldObject.MetaData;
using UnityEngine.Events;

public static class FirstTimeEventManager
{
	private static Action<FirstTimeState, FirstTimeEvent> firstTimeStatePublisher;

	private static FirstTimeState firstTimeState;

	public static Action XPRewarded;

	public static bool FirstTimeSystemInitialized { get; private set; }

	public static bool GetProfileMetaDataOk { get; set; }

	public static void Initialize(FirstTimeState firstTimeState)
	{
		FirstTimeEventManager.firstTimeState = firstTimeState;
		FirstTimeSystemInitialized = true;
		if (LevelingManager.IsInitialized)
		{
			if (firstTimeStatePublisher != null)
			{
				firstTimeStatePublisher(firstTimeState, FirstTimeEvent.NoEvent);
			}
		}
		else
		{
			LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, new UnityAction(OnLevelingInitialized));
		}
	}

	public static void Destroy()
	{
		firstTimeState = null;
	}

	public static void SubscribeToFirstTimeState(Action<FirstTimeState, FirstTimeEvent> firstTimeStateReceiver)
	{
		if (firstTimeState != null)
		{
			firstTimeStateReceiver(firstTimeState, FirstTimeEvent.NoEvent);
		}
		firstTimeStatePublisher = (Action<FirstTimeState, FirstTimeEvent>)Delegate.Combine(firstTimeStatePublisher, firstTimeStateReceiver);
	}

	public static void UnSubscribeToFirstTimeState(Action<FirstTimeState, FirstTimeEvent> firstTimeStateReceiver)
	{
		firstTimeStatePublisher = (Action<FirstTimeState, FirstTimeEvent>)Delegate.Remove(firstTimeStatePublisher, firstTimeStateReceiver);
	}

	public static void SetFirstTimeEvent(FirstTimeEvent firstTimeEvent)
	{
		firstTimeState.SetFirstTimeEvent(firstTimeEvent);
		if (firstTimeStatePublisher != null)
		{
			firstTimeStatePublisher(firstTimeState, firstTimeEvent);
		}
		MVGameControllerBase.Game.OperationRequestSender.SetFirstTimeEvent(firstTimeEvent);
	}

	public static void OnFirstTimeEventResponse(FirstTimeEvent firstTimeEvent, XPRewardType xpRewardType)
	{
		if (LevelingManager.IsInitialized && xpRewardType != XPRewardType.None)
		{
			XPRewarded();
		}
	}

	public static void OverrideFirstTimeEvent(FirstTimeEvent firstTimeEvent, bool overrideValue)
	{
		firstTimeState.OverrideFirstTimeEvent(firstTimeEvent, overrideValue);
		MVGameControllerBase.Game.OperationRequestSender.OverrideFirstTimeEvent(firstTimeEvent, overrideValue);
		if (firstTimeStatePublisher != null)
		{
			firstTimeStatePublisher(firstTimeState, FirstTimeEvent.NoEvent);
		}
	}

	public static void ResetFirstTimeEvents(bool overrideValue)
	{
		foreach (FirstTimeEvent value in Enum.GetValues(typeof(FirstTimeEvent)))
		{
			if (value != FirstTimeEvent.NoEvent)
			{
				firstTimeState.OverrideFirstTimeEvent(value, overrideValue);
			}
		}
		MVGameControllerBase.Game.OperationRequestSender.ResetFirstTimeEvents(overrideValue);
		if (firstTimeStatePublisher != null)
		{
			firstTimeStatePublisher(firstTimeState, FirstTimeEvent.NoEvent);
		}
	}

	public static bool HasFirstTimeEventOccured(FirstTimeEvent firstTimeEvent)
	{
		return firstTimeState.HasFirstTimeEventOccured(firstTimeEvent);
	}

	private static void OnLevelingInitialized()
	{
		if (firstTimeStatePublisher != null)
		{
			firstTimeStatePublisher(firstTimeState, FirstTimeEvent.NoEvent);
		}
		LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Remove(LevelingManager.OnLevelingInitialized, new UnityAction(OnLevelingInitialized));
	}
}
