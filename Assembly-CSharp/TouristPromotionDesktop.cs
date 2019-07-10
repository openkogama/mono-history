using System;

public class TouristPromotionDesktop : TouristPromotion
{
	private void Start()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
	}

	public void SignupCallback()
	{
		BrowserCommGotoRequests.GotoSignup(newTab: false, modalPopup: true);
	}

	public override void SkipCallback()
	{
		base.SkipCallback();
	}

	public void LoginCallback()
	{
		BrowserCommGotoRequests.GotoLogin(newTab: false, modalPopup: true);
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
