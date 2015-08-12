using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AssetBundleEntry
{
	public string name = string.Empty;

	public List<PlatformAssetPair> platformAssetList = new List<PlatformAssetPair>();

	public void Add(UnityEngine.Object obj, string platform)
	{
		if (name == string.Empty)
		{
			name = obj.name;
		}
		PlatformAssetPair platformAssetPair = platformAssetList.FirstOrDefault((PlatformAssetPair pap) => pap.platform == platform);
		if (platformAssetPair == null)
		{
			platformAssetPair = new PlatformAssetPair();
			platformAssetList.Add(platformAssetPair);
		}
		platformAssetPair.platform = platform;
		platformAssetPair.asset = obj;
	}

	public UnityEngine.Object GetAssetForPlatform(string platform)
	{
		if (platformAssetList == null)
		{
			return null;
		}
		return platformAssetList.FirstOrDefault((PlatformAssetPair p) => p.platform == platform)?.asset;
	}
}
