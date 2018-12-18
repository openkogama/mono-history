using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.Security;

public class TouristPromotionDesktop : TouristPromotion
{
	private void Start()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
	}

	public void SignupCallback()
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

	public override void SkipCallback()
	{
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState != MVGameStateType.RoundEnded)
		{
			MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		}
		base.SkipCallback();
	}

	public void LoginCallback()
	{
		BrowserComm.ToJavaScript.ExternalCall("gotoLogin");
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.loginURL);
	}

	private void OnWinningConditionFulfilled(IWinningCondition winningCondition)
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = false;
	}

	private void OnDestroy()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
	}

	public void ContinueCallback()
	{
		SkipCallback();
	}
}
