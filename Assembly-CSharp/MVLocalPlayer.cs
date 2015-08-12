using MV.Common;
using UnityEngine;

public abstract class MVLocalPlayer : MVPlayer
{
	protected XPEventQueue xpEventQueue;

	public XPProgress.OnXPProgressDataDelegate OnXPProgressData;

	public XPProgressData XPProgressData => xpEventQueue.XPProgressData;

	public MVLocalPlayer(int actorNumber, int profileID, string userName, string regionCode)
		: base(actorNumber, profileID, userName, regionCode)
	{
	}

	public virtual void InitializeLeveling(InitialLevelData initialLevelData)
	{
		level = initialLevelData.Level;
		xpEventQueue.Initialize(this, initialLevelData);
	}

	public void AddXp(string xpType, MVGameMode gameMode)
	{
		if (LevelingManager.LevelingEnabled && MVGameController.GameMode == gameMode)
		{
			Debug.Log("Adding xp");
			xpEventQueue.AddXp(xpType);
		}
	}

	protected void SendXpProgressEvent(XPProgressData xpProgressData)
	{
		if (OnXPProgressData != null)
		{
			OnXPProgressData(xpProgressData);
		}
		BrowserComm.ToJavaScript.ExternalCall("increaseXP", xpProgressData.XP);
	}
}
