using System;

public class FirstFrameUpdateActorReady : IUpdatecontrollerSubscriber
{
	private bool localPlayerIsReady;

	private bool firstFrameCallbackDone;

	public Action callbacks;

	public FirstFrameUpdateActorReady()
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerReady, new Action(OnLocalPlayerReady));
	}

	private void OnLocalPlayerReady()
	{
		localPlayerIsReady = true;
	}

	public void UpdateControllerUpdate()
	{
		if (localPlayerIsReady && !firstFrameCallbackDone)
		{
			if (callbacks != null)
			{
				callbacks();
			}
			firstFrameCallbackDone = true;
			UpdateController.RemoveUpdateObject(this);
		}
	}

	public void UpdateControllerFixedUpdate()
	{
	}
}
