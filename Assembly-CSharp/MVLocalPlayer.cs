using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine.Events;

public abstract class MVLocalPlayer : MVPlayer
{
	protected XPEventQueue xpEventQueue;

	public XPProgress.OnXPProgressDataDelegate OnXPProgressData;

	private int oldLevel;

	public XPProgressData XPProgressData => xpEventQueue.XPProgressData;

	public MVLocalPlayer(int actorNumber, int profileID, string userName, string regionCode)
		: base(actorNumber, profileID, userName, regionCode, MVGameControllerBase.BuildTarget)
	{
		OnLevelChanged = (UnityAction<int>)Delegate.Combine(OnLevelChanged, new UnityAction<int>(OnLevelChangedLocal));
	}

	public virtual void InitializeLeveling(InitialLevelData initialLevelData)
	{
		level = initialLevelData.Level;
		xpEventQueue.Initialize(this, initialLevelData);
	}

	public void AddXp(string xpType, MVGameMode gameMode)
	{
		if (LevelingManager.LevelingEnabled && MVGameControllerBase.GameMode == gameMode)
		{
			xpEventQueue.AddXp(xpType);
		}
	}

	protected void SendXpProgressEvent(XPProgressData xpProgressData)
	{
		if (OnXPProgressData != null)
		{
			OnXPProgressData(xpProgressData);
		}
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)4, xpProgressData.XPDelta);
		dictionary.Add((byte)1, xpProgressData.XPString);
		Dictionary<object, object> data = dictionary;
		NotificationController.OnNotificationReceived(NotificationType.XP, data);
		BrowserComm.ToJavaScript.ExternalCall("increaseXP", xpProgressData.XP);
	}

	protected void OnLevelChangedLocal(int level)
	{
		MVGameControllerBase.OperationRequests.LocalPlayerLevelChanged(level);
		if (oldLevel != 0)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)4, level);
			Dictionary<object, object> data = dictionary;
			NotificationController.OnNotificationReceived(NotificationType.LevelUp, data);
		}
		oldLevel = level;
	}
}
