using System;

namespace MV.Common;

public static class Urls
{
	public delegate void OnStreamingAssetsUrlAvailable();

	public static OnStreamingAssetsUrlAvailable onStreamingAssetsUrlAvailable;

	private static string streamingAssets = "";

	private static string api = "";

	public static string Badges => API + "xp_level/badges/";

	public static string XPData => API + "xp_level/xp_data/";

	public static string XPLimit => API + "xp_level/xp_limits_data/?level=";

	public static string Level => API + "xp_level/level/?profile_id=";

	public static string XP => API + "xp_level/xp/?profile_id=";

	public static string UpdateXP => API + "xp_level/xp/";

	public static string InitialData => api + "xp_level/init_data/?profile_id=";

	public static string StreamingAssets
	{
		get
		{
			ValidateGet(streamingAssets);
			return streamingAssets;
		}
	}

	public static string API
	{
		get
		{
			ValidateGet(api);
			return api;
		}
	}

	public static void Init(string apiUrl, string streamingAssetsUrl)
	{
		api = apiUrl;
		streamingAssets = streamingAssetsUrl;
		if (onStreamingAssetsUrlAvailable != null)
		{
			onStreamingAssetsUrlAvailable();
		}
	}

	public static bool StreamingAssetUrlReady()
	{
		return !string.IsNullOrEmpty(streamingAssets);
	}

	private static void ValidateGet(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			throw new Exception("Url not set");
		}
	}
}
