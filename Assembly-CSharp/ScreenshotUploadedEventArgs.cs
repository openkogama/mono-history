using System;

public class ScreenshotUploadedEventArgs : EventArgs
{
	public readonly bool Uploaded;

	public ScreenshotUploadedEventArgs(bool uploaded)
	{
		Uploaded = uploaded;
	}
}
