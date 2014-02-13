using System;
using MV.Common;

[Serializable]
public class StreamingAssetInfo : ProductInfo
{
	public static readonly TimeSpan ExpiringFetchThreshold = TimeSpan.FromMinutes(3.0);

	public StreamingAssetType StreamedAssetType;

	public int CategoryID;

	public string AssetPath;

	public int Version;

	public string RequestPath => (Version != 0) ? (AssetPath + "?v=" + Version) : AssetPath;

	public string CategoryName => StreamedAssetType switch
	{
		StreamingAssetType.AmbientAudio => Enum.GetName(typeof(AmbientAudioCategory), CategoryID), 
		StreamingAssetType.AvatarAccessory => Enum.GetName(typeof(AvatarAccessoryCategory), CategoryID), 
		_ => null, 
	};

	public override bool IsEquippable => StreamedAssetType == StreamingAssetType.AvatarAccessory;

	public StreamingAssetInfo()
	{
		ProductType = MVProductType.StreamingAsset;
	}

	public override string ToString()
	{
		return "assetID: " + ProductID + ", name: " + Name + ", ver: " + Version + ", " + ((ShopInfo != null) ? ShopInfo.ToString() : string.Empty);
	}
}
