using UnityEngine;
using UnityEngine.UI;

public class StreamedSpriteToImage : StreamingAsset<Sprite, Texture2D>
{
	[Header("Dependencies")]
	[SerializeField]
	protected Image image;

	public void Reset()
	{
		if (image == null)
		{
			image = GetComponent<Image>();
		}
	}

	protected override void OnAssetSet()
	{
		image.sprite = Asset;
	}
}
