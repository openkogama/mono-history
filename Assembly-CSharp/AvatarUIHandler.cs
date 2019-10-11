using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class AvatarUIHandler : MonoBehaviour
{
	protected ChatAnchor chatBubbleAnchor;

	protected MVWorldObjectClient worldObject;

	protected int ownerActorNr;

	protected bool shouldShowUI = true;

	public virtual void Initialize(bool isLocal, MVWorldObjectClient wo, int ownerActorNr, ChatAnchor chatBubbleAnchor)
	{
		worldObject = wo;
		this.ownerActorNr = ownerActorNr;
		this.chatBubbleAnchor = chatBubbleAnchor;
		if (isLocal)
		{
			SayChatBubbleVisibilityManager.OnSayChatMessageRecieved = (Action<int, Dictionary<object, object>>)Delegate.Combine(SayChatBubbleVisibilityManager.OnSayChatMessageRecieved, new Action<int, Dictionary<object, object>>(OnSayChatMessageRecieved));
		}
		MVGameControllerBase.OnFirstFrameUpdateActorReady = (Action)Delegate.Combine(MVGameControllerBase.OnFirstFrameUpdateActorReady, new Action(HandleTeamChange));
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Combine(mVPlayerContainer.OnPlayerListChanged, new Action(HandleTeamChange));
		MVGameControllerBase.Game.TeamManager.OnTeamAdded += HandleTeamChange;
		MVGameControllerBase.Game.TeamManager.OnTeamRemoved += HandleTeamChange;
	}

	public virtual void Activate()
	{
	}

	public virtual void Deactivate()
	{
	}

	public virtual void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
	}

	public void OnSayChatMessageRecieved(int actorNr, Dictionary<object, object> data)
	{
		if (ownerActorNr == actorNr && gameObject.activeInHierarchy)
		{
			int instanceID = chatBubbleAnchor.GetInstanceID();
			string text = (string)data[(byte)5];
			ChatBubbleManager.ShowChatBubble(text, instanceID, chatBubbleAnchor);
		}
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

	public void ForceDestroy()
	{
		OnDestroy();
	}

	protected virtual void OnDestroy()
	{
		SayChatBubbleVisibilityManager.OnSayChatMessageRecieved = (Action<int, Dictionary<object, object>>)Delegate.Remove(SayChatBubbleVisibilityManager.OnSayChatMessageRecieved, new Action<int, Dictionary<object, object>>(OnSayChatMessageRecieved));
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
