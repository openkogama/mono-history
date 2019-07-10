using System;

public class BoostAdController
{
	public void RequestAdAvailability(Action<bool> onAdAvailable)
	{
		onAdAvailable(obj: false);
	}

	public void ShowAd(Action<bool> OnAdFinished)
	{
		OnAdFinished(obj: false);
	}
}
