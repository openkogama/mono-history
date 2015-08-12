using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.Security;
using Newtonsoft.Json;
using UnityEngine;

public class XPEventQueueRegistered : XPEventQueue
{
	private class XPUpdateData
	{
		public int XP { get; set; }

		public int XPTypeID { get; set; }

		public override string ToString()
		{
			return $"XP {XP}. XPTypeID {XPTypeID}.";
		}
	}

	private readonly int profileID = -1;

	public XPEventQueueRegistered(MVPlayer player)
	{
		profileID = player.ProfileID;
	}

	private void XPUpdateCallback(WWW result)
	{
		XPUpdateData xPUpdateData = JsonConvert.DeserializeObject<XPUpdateData>(result.text);
		Debug.Log(xPUpdateData);
		xpProgress.Update(xPUpdateData.XP, (byte)xPUpdateData.XPTypeID);
	}

	protected override void RequestXp(XPData xpData)
	{
		if (!Application.isEditor && !Debug.isDebugBuild)
		{
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
			sortedDictionary.Add("profile_id", profileID.ToString());
			sortedDictionary.Add("xp_type_id", xpData.XPId.ToString());
			sortedDictionary.Add("timestamp", Environment.TickCount.ToString());
			sortedDictionary.Add("token", MVGameController.GameSessionData.token);
			string mD5Hash = Encryption.GetMD5Hash(sortedDictionary, MVGameController.Game.XpKey);
			sortedDictionary.Add("signature", mD5Hash);
			WWWForm wWWForm = new WWWForm();
			foreach (KeyValuePair<string, string> item in sortedDictionary)
			{
				wWWForm.AddField(item.Key, item.Value);
			}
			AsyncWWWManager.WWWRequest(new PostRequest(Urls.UpdateXP, wWWForm, XPUpdateCallback));
		}
		else
		{
			Debug.LogWarning("Bypassing www request because of token unavailable in editor or debug build");
			xpProgress.Update(xpProgress.XP + xpData.XPAmount, xpData.XPId);
		}
	}
}
