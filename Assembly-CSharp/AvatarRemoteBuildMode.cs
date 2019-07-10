using System;
using UnityEngine;
using UnityEngine.Events;

public class AvatarRemoteBuildMode : MonoBehaviour
{
	[SerializeField]
	private AvatarUIHandlerRemote avatarUIHandlerRemote;

	[SerializeField]
	private ChatAnchor chatBubbleAnchor;

	[SerializeField]
	private AvatarEnabledChangeHandler enabledChangeHandler;

	public AvatarEnabledChangeHandler EnabledChangeHandler => enabledChangeHandler;

	public void Initialize(int ownerActorNr, MVBuildModeAvatar avatar)
	{
		avatarUIHandlerRemote.Initialize(isLocal: false, avatar, ownerActorNr, chatBubbleAnchor);
		avatar.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(avatar.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
	}

	private void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		avatarUIHandlerRemote.OnPositionChanged(arg0, positionChangedEventArgs);
	}

	public void Activate()
	{
		avatarUIHandlerRemote.Activate();
	}

	public void Deactivate()
	{
		avatarUIHandlerRemote.Deactivate();
	}
}
