using System;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;

public class MVLocalPlayerRegistered : MVLocalPlayer
{
	public MVLocalPlayerRegistered(int actorNumber, int profileID, string userName, string regionCode, int planetOwnershipTypeId)
		: base(actorNumber, profileID, userName, regionCode, planetOwnershipTypeId)
	{
		xpEventQueue = new XPEventQueueRegistered(this);
	}

	public override void InitializeLeveling(InitialLevelData initialLevelData)
	{
		base.InitializeLeveling(initialLevelData);
		XPEventQueue xPEventQueue = xpEventQueue;
		xPEventQueue.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(xPEventQueue.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnXPProgressDataChangeRegistered));
		OnLevelChangedLocalReceivedLevelData(Level);
	}

	private void OnXPProgressDataChangeRegistered(XPProgressData xpProgress)
	{
		if (xpProgress.XPLimitExceeded)
		{
			if (!MVGameControllerBase.LevelingTestMode)
			{
				AsyncWWWManager.WWWRequest(new GetRequest(Urls.Level + ProfileID, LevelCallback, WWWRequestPriority.ExecuteWhileSyncronizing));
			}
			else
			{
				Level++;
			}
		}
		else
		{
			SendXpProgressEvent(xpProgress);
		}
	}

	private void LevelCallback(WWW result)
	{
		Level = JsonConvert.DeserializeObject<int>(result.text);
	}

	private void OnLevelChangedLocalReceivedLevelData(int level)
	{
		OnLevelChangedLocal(level);
		MVGameControllerBase.OperationRequests.JoinNotification();
	}

	public override void Destroy()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(LevelCallback);
		xpEventQueue.Destroy();
	}
}
