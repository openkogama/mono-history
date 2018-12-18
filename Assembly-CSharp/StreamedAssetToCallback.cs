using System;
using UnityEngine;

public class StreamedAssetToCallback<AssetType, PreviewType> : StreamingAsset<AssetType, PreviewType> where AssetType : UnityEngine.Object where PreviewType : UnityEngine.Object
{
	public Action<AssetType> onAssetSet;

	protected override void OnAssetSet()
	{
		onAssetSet(Asset);
	}
}
