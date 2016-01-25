using System;
using System.Collections.Generic;
using MV.WorldObject.Security;
using UnityEngine;

public class MVGUITouristRegister : UXViewScript
{
	[SerializeField]
	private UXToggleIconButton registerButton;

	public void Start()
	{
		if (MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion)
		{
			BindButtons();
		}
		else
		{
			gameObject.SetActive(value: false);
		}
	}

	private void BindButtons()
	{
		UXToggleIconButton uXToggleIconButton = registerButton;
		uXToggleIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXToggleIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			GotoSignup();
		}));
	}

	private void GotoSignup()
	{
		if (!LevelingManager.IsInitialized)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoSignup");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.signupURL);
			return;
		}
		SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
		int xP = MVGameControllerBase.Game.LocalPlayer.XPProgressData.XP;
		sortedDictionary.Add("xp", xP.ToString());
		string mD5Hash = Encryption.GetMD5Hash(sortedDictionary, MVGameControllerBase.Game.XpKey);
		BrowserComm.ToJavaScript.ExternalCall("gotoSignupWithXP", xP, mD5Hash);
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.signupURL);
	}
}
