using System;
using System.Collections.Generic;
using UnityEngine;

public static class AndroidUrls
{
	private static Dictionary<string, string> bundleIdentifierRegionUrlMap = new Dictionary<string, string>
	{
		{ "com.multiverse.devkogama", "http://lw-dev.kogama.com/" },
		{ "com.multiverse.testkogama", "http://lw-test.kogama.com/" },
		{ "com.multiverse.friendskogama", "http://friends.kogama.com/" },
		{ "com.multiverse.brkogama", "http://kogama.com.br/" },
		{ "com.multiverse.kogama", "http://kogama.com/" }
	};

	private static Dictionary<string, string> bundleIdentifierVersionUrlsMap = new Dictionary<string, string>
	{
		{ "com.multiverse.devkogama", "http://api-devv.kgoma.com/v1/api/server/android/version/" },
		{ "com.multiverse.testkogama", "http://api-testt.kgoma.com/v1/api/server/android/version/" },
		{ "com.multiverse.friendskogama", "http://api-friends.kgoma.com/v1/api/server/android/version/" },
		{ "com.multiverse.brkogama", "http://api-br.kgoma.com/v1/api/server/android/version/" },
		{ "com.multiverse.kogama", "http://api-www.kgoma.com/v1/api/server/android/version/" }
	};

	public static string RegionUrl
	{
		get
		{
			string bundleIdentifier = Application.bundleIdentifier;
			if (!bundleIdentifierRegionUrlMap.ContainsKey(bundleIdentifier))
			{
				throw new Exception("No regionUrl defined for " + bundleIdentifier);
			}
			return bundleIdentifierRegionUrlMap[bundleIdentifier];
		}
	}

	public static string VersionUrl
	{
		get
		{
			string bundleIdentifier = Application.bundleIdentifier;
			if (!bundleIdentifierVersionUrlsMap.ContainsKey(bundleIdentifier))
			{
				throw new Exception("No regionUrl defined for " + bundleIdentifier);
			}
			return bundleIdentifierVersionUrlsMap[bundleIdentifier];
		}
	}

	public static string MarketPlaceUrl => "market://details?id=" + Application.bundleIdentifier;
}
