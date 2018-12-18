using System;
using MV.WorldObject;
using UnityEngine;

public class AvatarUIHandler : MonoBehaviour
{
	[SerializeField]
	private ChatAnchor chatBubbleAnchor;

	protected MVAvatar mvAvatar;

	protected bool shouldShowUI = true;

	public ChatAnchor ChatBubbleAnchor => chatBubbleAnchor;

	public virtual void Initialize(bool isLocal, MVAvatar mvAvatar)
	{
		this.mvAvatar = mvAvatar;
		chatBubbleAnchor.Initialize(isLocal, mvAvatar.Avatar);
		MVGameControllerBase.OnFirstFrameUpdateActorReady = (Action)Delegate.Combine(MVGameControllerBase.OnFirstFrameUpdateActorReady, new Action(HandleTeamChange));
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Combine(mVPlayerContainer.OnPlayerListChanged, new Action(HandleTeamChange));
		MVGameControllerBase.Game.TeamManager.OnTeamAdded += HandleTeamChange;
		MVGameControllerBase.Game.TeamManager.OnTeamRemoved += HandleTeamChange;
	}

	public virtual void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
	}

	private void HandleTeamChange(object sender, TeamEventArgs eventArgs)
	{
		HandleTeamChange();
	}

	public virtual void HandleTeamChange()
	{
	}

	public virtual void SetShouldShowUI(bool shouldShow)
	{
		shouldShowUI = shouldShow;
	}

	protected virtual void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.OnFirstFrameUpdateActorReady = (Action)Delegate.Remove(MVGameControllerBase.OnFirstFrameUpdateActorReady, new Action(HandleTeamChange));
			if (MVGameControllerBase.Game != null)
			{
				MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
				mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Remove(mVPlayerContainer.OnPlayerListChanged, new Action(HandleTeamChange));
				MVGameControllerBase.Game.TeamManager.OnTeamAdded -= HandleTeamChange;
				MVGameControllerBase.Game.TeamManager.OnTeamRemoved -= HandleTeamChange;
			}
		}
	}
}
