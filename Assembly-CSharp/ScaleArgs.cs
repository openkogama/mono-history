using System;

public class ScaleArgs : EventArgs
{
	public float scale;

	public ScaleArgs(float aScale)
	{
		scale = aScale;
	}
}
