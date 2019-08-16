using UnityEngine;

namespace Assets.Scripts.AdIntegration.Mobile;

public static class MobileAdManagerCredentials
{
	private static string identifier = string.Empty;

	public static AdMobCredentials GetAdMobCredentials()
	{
		Debug.LogWarning("AdMobCredentials not set for build target");
		return GetDefault();
	}

	public static void Setup(string identifier)
	{
		MobileAdManagerCredentials.identifier = identifier;
	}

	private static AdMobCredentials GetDefault()
	{
		return new AdMobCredentials("AppId not set", "Reward ad unit not set", "Interstitial ad unit not set", "Banner ad unit not");
	}
}
