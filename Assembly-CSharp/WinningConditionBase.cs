using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public abstract class WinningConditionBase : MonoBehaviour
{
	[SerializeField]
	private RoundTimer roundTimerPrefab;

	private RoundTimer roundTimer;

	protected WinningConditionNotificationManager notificationManager;

	protected abstract GameStatCounterType StatType { get; }

	public virtual bool WinningConditionAbleToBeFulfilled => true;

	public abstract void UpdateValue(int newValue);

	public void UpdateStats(int actorNumber, GameStatCounterType counterType, int scoreCount)
	{
		if (StatType == counterType)
		{
			UpdateProgressNotification(actorNumber, counterType, scoreCount);
		}
	}

	protected void UpdateProgressNotification(int actorNumber, GameStatCounterType counterType, int scoreCount)
	{
		notificationManager.UpdateNotification(actorNumber, counterType, scoreCount);
	}

	public virtual bool CanWinningConditionBeFullfilledForTeam(MVTeam team)
	{
		return true;
	}

	public virtual void Clear()
	{
		if (this.roundTimer != null)
		{
			RoundTimer roundTimer = this.roundTimer;
			roundTimer.OnTimeNotificationSend = (Action<NotificationType, Dictionary<object, object>>)Delegate.Remove(roundTimer.OnTimeNotificationSend, new Action<NotificationType, Dictionary<object, object>>(notificationManager.SendNotificaion));
			UnityEngine.Object.Destroy(this.roundTimer.gameObject);
			this.roundTimer = null;
		}
	}

	public virtual void RoundEndReset()
	{
		if (roundTimer != null)
		{
			roundTimer.ResetOnRoundEnd();
		}
	}

	public virtual void InitializeGameUI(RectTransform lobbyState)
	{
		notificationManager = new WinningConditionNotificationManager();
		notificationManager.Initialize();
		WorldObjectClientRef<MVRoundCube> singletonWorldObjectRef = MVGameControllerBase.WOCM.GetSingletonWorldObjectRef<MVRoundCube>();
		if (singletonWorldObjectRef != null)
		{
			CreateRoundTimer(singletonWorldObjectRef);
		}
	}

	private void CreateRoundTimer(WorldObjectClientRef<MVRoundCube> roundCube)
	{
		this.roundTimer = UnityEngine.Object.Instantiate(roundTimerPrefab);
		this.roundTimer.transform.SetParent(transform.parent, worldPositionStays: false);
		this.roundTimer.transform.SetAsFirstSibling();
		this.roundTimer.Initialize(roundCube);
		RoundTimer roundTimer = this.roundTimer;
		roundTimer.OnTimeNotificationSend = (Action<NotificationType, Dictionary<object, object>>)Delegate.Combine(roundTimer.OnTimeNotificationSend, new Action<NotificationType, Dictionary<object, object>>(notificationManager.SendNotificaion));
		transform.SetAsFirstSibling();
	}
}
