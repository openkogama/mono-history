using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine.Events;

public abstract class MVLocalPlayer : MVPlayer
{
	public enum PlanetOwnershipType
	{
		None,
		Editor,
		Owner
	}

	public Action OnInitializeLeveling;

	protected XPProgress xpProgress;

	private int planetOwnershipTypeID;

	public XPProgress.OnXPProgressDataDelegate OnXPProgressData;

	protected int joinTime;

	private int oldLevel;

	public int PlanetOwnershipTypeID
	{
		get
		{
			if (MVGameControllerBase.GameMode != MVGameMode.Edit)
			{
				throw new Exception("There are currently no way to access MVLocalPlayer:PlanetOwnership in play mode. Server side refactoring is required to fix this.");
			}
			return planetOwnershipTypeID;
		}
		private set
		{
			planetOwnershipTypeID = value;
		}
	}

	public PlanetOwnershipType PlanetOwnership => (PlanetOwnershipType)PlanetOwnershipTypeID;

	public virtual bool IsAdmin { get; private set; }

	public XPProgressData XPProgressData => xpProgress.XPProgressData;

	public int JoinTime => joinTime;

	public bool CanGetXPProgressData => xpProgress != null;

	public MVLocalPlayer(int actorNumber, int profileID, string userName, string regionCode, int planetOwnershipTypeID, bool isAdmin)
		: base(actorNumber, profileID, userName, regionCode, MVGameControllerBase.BuildTarget, isReady: false)
	{
		OnLevelChanged = (UnityAction<int>)Delegate.Combine(OnLevelChanged, new UnityAction<int>(OnLevelChangedLocal));
		PlanetOwnershipTypeID = planetOwnershipTypeID;
		joinTime = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
		IsAdmin = isAdmin;
	}

	public virtual void InitializeLeveling(InitialLevelData initialLevelData)
	{
		level = initialLevelData.Level;
		xpProgress = new XPProgress(this, initialLevelData);
		if (OnInitializeLeveling != null)
		{
			OnInitializeLeveling();
		}
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
