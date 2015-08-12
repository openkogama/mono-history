using System;
using MV.Common;

public class StreamingAssetInfo
{
	public MVProductType ProductType;

	public int ProductID;

	public string Name;

	public string Desc;

	public ProductShopInfo ShopInfo;

	public static readonly TimeSpan ExpiringFetchThreshold = TimeSpan.FromMinutes(3.0);

	public StreamingAssetType StreamedAssetType;

	public int CategoryID;

	public string _assetPath;

	private bool _isEditorPreview;

	public string AssetPath
	{
		get
		{
			return _assetPath;
		}
		set
		{
			_assetPath = value;
			_isEditorPreview = _assetPath.StartsWith("file://");
		}
	}

	public bool IsEditorPreview
	{
		get
		{
			return _isEditorPreview;
		}
		set
		{
			_isEditorPreview = value;
		}
	}

	public string RequestPath => AssetPath;

	public string CategoryName => StreamedAssetType switch
	{
		StreamingAssetType.AmbientAudio => Enum.GetName(typeof(AmbientAudioCategory), CategoryID), 
		StreamingAssetType.AvatarAccessory => Enum.GetName(typeof(AvatarAccessoryCategory), CategoryID), 
		_ => null, 
	};

	public bool IsEquippable => StreamedAssetType == StreamingAssetType.AvatarAccessory;

	public StreamingAssetInfo()
	{
		ProductType = MVProductType.StreamingAsset;
	}

	public override string ToString()
	{
		return "assetID: " + ProductID + ", name: " + Name + ", " + ((ShopInfo != null) ? ShopInfo.ToString() : string.Empty);
	}
}
