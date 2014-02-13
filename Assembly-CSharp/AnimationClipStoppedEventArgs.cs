using System;

public class AnimationClipStoppedEventArgs : EventArgs
{
	public readonly string ClipName;

	public AnimationClipStoppedEventArgs(string clipName)
	{
		ClipName = clipName;
	}
}
