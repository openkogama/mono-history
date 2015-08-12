using System;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;

public class XPProgress
{
	public delegate void OnXPProgressDataDelegate(XPProgressData xpProgress);

	public OnXPProgressDataDelegate OnXPProgressData;

	private XPProgressData xpProgressData;

	public XPProgressData XPProgressData => xpProgressData;

	public int XP => xpProgressData.XP;

	public XPProgress(MVLocalPlayer player, InitialLevelData initialLevelData)
	{
		xpProgressData = new XPProgressData(initialLevelData.XP, initialLevelData.XPLevelLimits);
		player.OnLevelChanged = (MVPlayer.OnLevelChangedDelegate)Delegate.Combine(player.OnLevelChanged, new MVPlayer.OnLevelChangedDelegate(UpdateLevel));
	}

	public void Update(int xp, byte xpId)
	{
		xpProgressData.XpID = xpId;
		xpProgressData.XP = xp;
		if (OnXPProgressData != null && !LevelingManager.silentMode)
		{
			OnXPProgressData(xpProgressData);
		}
	}

	private void UpdateLevel(int level)
	{
		if (xpProgressData.Level != level)
		{
			if (!MVGameController.LevelingTestMode)
			{
				Debug.LogWarning("Implement XPAPI");
				AsyncWWWManager.WWWRequest(new GetRequest(Urls.XPLimit + level, XPLimitsCallback));
			}
			else
			{
				OnXPLevelLimitsUpdated(LevelingManager.TestLevelToLimits[level]);
			}
		}
	}

	private void XPLimitsCallback(WWW result)
	{
		Debug.Log(result.url);
		XPLevelLimits xpLevelLimits = JsonConvert.DeserializeObject<XPLevelLimits>(result.text);
		OnXPLevelLimitsUpdated(xpLevelLimits);
	}

	private void OnXPLevelLimitsUpdated(XPLevelLimits xpLevelLimits)
	{
		Debug.Log(xpLevelLimits);
		xpProgressData.XPLevelLimits = xpLevelLimits;
		if (OnXPProgressData != null)
		{
			OnXPProgressData(xpProgressData);
		}
	}
}
