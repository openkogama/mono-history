using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine.Events;

public abstract class MVLocalPlayer : MVPlayer
{
	protected XPProgress xpProgress;

	public XPProgress.OnXPProgressDataDelegate OnXPProgressData;

	private int oldLevel;

	public int PlanetOwnershipTypeID { get; private set; }

	public XPProgressData XPProgressData => xpProgress.XPProgressData;

	public MVLocalPlayer(int actorNumber, int profileID, string userName, string regionCode, int planetOwnershipTypeID)
		: base(actorNumber, profileID, userName, regionCode, MVGameControllerBase.BuildTarget, isReady: false)
	{
		OnLevelChanged = (UnityAction<int>)Delegate.Combine(OnLevelChanged, new UnityAction<int>(OnLevelChangedLocal));
		PlanetOwnershipTypeID = planetOwnershipTypeID;
	}

	public virtual void InitializeLeveling(InitialLevelData initialLevelData)
	{
		level = initialLevelData.Level;
		xpProgress = new XPProgress(this, initialLevelData);
	}

	public void AddXp(int currentPlayerXP, XPRewardType typeId, int xpDelta)
	{
		if (LevelingManager.IsInitialized)
		{
			xpProgress.Update(currentPlayerXP, typeId, xpDelta);
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

	public virtual void Destroy()
	{
		xpProgress.Destroy();
	}
}
