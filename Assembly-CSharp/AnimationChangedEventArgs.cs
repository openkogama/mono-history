using System;

public class AnimationChangedEventArgs : EventArgs
{
	public readonly string Animation;

	public AnimationChangedEventArgs(string animation)
	{
		Animation = animation;
	}
}
