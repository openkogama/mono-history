using System;
using UnityEngine;
using UnityEngine.Networking;

internal static class AssetBundleCacheTest
{
	private const string assetUrl = "Test/bignoise.unity3d";

	public static void Run(int version)
	{
		int localDiscCacheAssetVersion = MVGameControllerBase.KoGaMaSettings.LocalDiscCacheAssetVersion;
		MVGameControllerBase.KoGaMaSettings.SetStreamingAssetVersion(localDiscCacheAssetVersion + version);
		DownloadTestAsset(OnFirstDownloadFinished, localDiscCacheAssetVersion);
	}

	public static void OnFirstDownloadFinished(UnityWebRequest result, float startTime, int currentStreamingAssetVersion)
	{
		if (string.IsNullOrEmpty(result.error))
		{
			TextCommand.NotifyUser($"Download time 1: {Time.realtimeSinceStartup - startTime}");
			DownloadTestAsset(OnSecondDownloadFinished, currentStreamingAssetVersion);
			return;
		}
		MVGameControllerBase.KoGaMaSettings.SetStreamingAssetVersion(currentStreamingAssetVersion);
		string text = $"Error executing command: {result.error}";
		TextCommand.NotifyUser(text);
		Debug.LogError(text);
	}

	public static void OnSecondDownloadFinished(UnityWebRequest result, float startTime, int currentStreamingAssetVersion)
	{
		if (string.IsNullOrEmpty(result.error))
		{
			TextCommand.NotifyUser($"Download time 2: {Time.realtimeSinceStartup - startTime}");
		}
		else
		{
			string text = $"Error executing command: {result.error}";
			TextCommand.NotifyUser(text);
			Debug.LogError(text);
		}
		MVGameControllerBase.KoGaMaSettings.SetStreamingAssetVersion(currentStreamingAssetVersion);
	}

	private static void DownloadTestAsset(Action<UnityWebRequest, float, int> onDownloadFinished, int currentStreamingAssetVersion)
	{
		float startTime = Time.realtimeSinceStartup;
		string text = StreamingAsset.AssetBundleUrl + "Test/bignoise.unity3d";
		Debug.LogFormat("Url: {0}\nUrl appendage: {1}\nVersion: {2}", text, MVGameControllerBase.KoGaMaSettings.UrlCacheAssetVersionArgument, MVGameControllerBase.KoGaMaSettings.LocalDiscCacheAssetVersion);
		AsyncWWWManager.WWWRequest(new CachedAssetBundleRequest(text, (UnityWebRequest www) =>
		{
			onDownloadFinished(www, startTime, currentStreamingAssetVersion);
		}, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}
}
