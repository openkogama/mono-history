using System;
using UnityEngine.Events;

public class AvatarRemote : Avatar
{
	public override void Initialize(MVAvatar mvAvatar, bool isLocal)
	{
		base.Initialize(mvAvatar, isLocal);
		mvAvatar.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(mvAvatar.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
	}

	private void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		avatarUIHandler.OnPositionChanged(arg0, positionChangedEventArgs);
	}
}
