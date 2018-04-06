using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NoWinningCondition : MonoBehaviour
{
	[SerializeField]
	private RoundTimer roundTimerPrefab;

	private RoundTimer roundTimer;

	private WinningConditionNotificationManager notificationManager;

	public void Initialize(RectTransform lobbyStateUI)
	{
		notificationManager = new WinningConditionNotificationManager();
		notificationManager.Initialize();
		TryInitializeRoundCube();
	}

	public void TryInitializeRoundCube()
	{
		WorldObjectClientRef<MVRoundCube> singletonWorldObjectRef = MVGameControllerBase.WOCM.GetSingletonWorldObjectRef<MVRoundCube>();
		if (singletonWorldObjectRef != null)
		{
			CreateRoundTimer(singletonWorldObjectRef);
		}
	}

	public virtual void RoundEndReset()
	{
		if (roundTimer != null)
		{
			roundTimer.ResetOnRoundEnd();
		}
	}

	public void Clear()
	{
		if (this.roundTimer != null)
		{
			UnityEngine.Object.Destroy(this.roundTimer.gameObject);
			RoundTimer roundTimer = this.roundTimer;
			roundTimer.OnTimeNotificationSend = (Action<NotificationType, Dictionary<object, object>>)Delegate.Remove(roundTimer.OnTimeNotificationSend, new Action<NotificationType, Dictionary<object, object>>(notificationManager.SendNotificaion));
			this.roundTimer = null;
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
	}
}
