using System;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

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

	public void Update(int currentPlayerXP, XPRewardType xpId, int XPdelta, int memberCount)
	{
		xpProgressData.XpID = xpId;
		xpProgressData.XP = currentPlayerXP;
		xpProgressData.XPDelta = XPdelta;
		xpProgressData.MemberCount = memberCount;
		if (OnXPProgressData != null)
		{
			OnXPProgressData(xpProgressData);
		}
	}

	private void UpdateLevel(int level)
	{
		if (xpProgressData.Level != level)
		{
			AsyncWWWManager.WWWRequest(new GetRequest(Urls.XPLimit + level, XPLimitsCallback, WWWRequestPriority.ExecuteWhileSyncronizing));
		}
	}

	public void Destroy()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(XPLimitsCallback);
	}

	private void XPLimitsCallback(UnityWebRequest result)
	{
		Debug.Log(result.url);
		XPLevelLimits xpLevelLimits = JsonConvert.DeserializeObject<XPLevelLimits>(result.downloadHandler.text);
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
