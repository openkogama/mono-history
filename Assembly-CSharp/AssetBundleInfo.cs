using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MV.Common;
using UnityEngine;

public class AssetBundleInfo : ScriptableObject
{
	public int Version;

	public StreamingAssetInfo StreamingAssetInfo = new StreamingAssetInfo();

	public string TypeRelativeBuildPath;

	public bool GatherDependencies = true;

	public bool Compressed = true;

	public List<AssetBundleEntry> AssetEntries = new List<AssetBundleEntry>();

	public string RelativeBuildDirPath
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(StreamingAssetInfo.StreamedAssetType.ToString());
			string categoryName = StreamingAssetInfo.CategoryName;
			if (categoryName != null)
			{
				stringBuilder.Append("/").Append(categoryName);
			}
			return stringBuilder.ToString();
		}
	}

	public string RelativeBuildPath => new StringBuilder(RelativeBuildDirPath).Append("/").Append(BundleName).ToString();

	public string BundleName => new StringBuilder(name).Append(".unity3d").ToString();

	public AssetBundleInfo()
	{
		StreamingAssetInfo = new StreamingAssetInfo();
		StreamingAssetInfo.ShopInfo = new ProductShopInfo();
		StreamingAssetInfo.Desc = string.Empty;
		StreamingAssetInfo.Name = string.Empty;
	}

	public override int GetHashCode()
	{
		return 17 * GetInstanceID();
	}

	public List<Object> GetAssetsForPlatform(string platform)
	{
		return AssetEntries.ConvertAll((AssetBundleEntry e) => e.GetAssetForPlatform(platform));
	}

	public static bool IsValidText(string txt)
	{
		if (txt == null || txt == string.Empty)
		{
			return false;
		}
		Regex regex = new Regex("[A-Za-z0-9',./:!?\\s_-]+");
		Match match = regex.Match(txt);
		return match.Value == txt;
	}

	public static bool IsValid(AssetBundleInfo bundle)
	{
		if (bundle.StreamingAssetInfo.StreamedAssetType == StreamingAssetType.Undefined)
		{
			return false;
		}
		if (bundle.StreamingAssetInfo.CategoryID == 0)
		{
			return false;
		}
		if (!IsValidText(bundle.StreamingAssetInfo.Name))
		{
			return false;
		}
		if (!IsValidText(bundle.StreamingAssetInfo.Desc))
		{
			return false;
		}
		if (bundle.AssetEntries.Count == 0)
		{
			return false;
		}
		if (bundle.AssetEntries[0] == null)
		{
			return false;
		}
		return true;
	}
}
