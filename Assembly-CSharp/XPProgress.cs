using System;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

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
		player.OnLevelChanged = (UnityAction<int>)Delegate.Combine(player.OnLevelChanged, new UnityAction<int>(UpdateLevel));
	}

	public void Update(int currentPlayerXP, XPRewardType xpId, int XPdelta)
	{
		xpProgressData.XpID = xpId;
		xpProgressData.XP = currentPlayerXP;
		xpProgressData.XPDelta = XPdelta;
		if (OnXPProgressData != null)
		{
			OnXPProgressData(xpProgressData);
		}
	}

	private void UpdateLevel(int level)
	{
		if (xpProgressData.Level != level)
		{
			if (!MVGameControllerBase.LevelingTestMode)
			{
				Debug.LogWarning("Implement XPAPI");
				AsyncWWWManager.WWWRequest(new GetRequest(Urls.XPLimit + level, XPLimitsCallback, WWWRequestPriority.ExecuteWhileSyncronizing));
			}
			else
			{
				OnXPLevelLimitsUpdated(LevelingManager.TestLevelToLimits[level]);
			}
		}
	}

	public void Destroy()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(XPLimitsCallback);
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
