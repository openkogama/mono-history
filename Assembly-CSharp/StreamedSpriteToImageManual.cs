using UnityEngine;
using UnityEngine.UI;

public class StreamedSpriteToImageManual : StreamingAsset<Sprite, Texture2D>
{
	[SerializeField]
	[Header("Dependencies")]
	protected Image image;

	private bool isInitialized;

	public bool IsInitialized => isInitialized;

	protected override void Start()
	{
	}

	public void Initialize(string downloadUrl)
	{
		isInitialized = true;
		url = downloadUrl;
	}

	public void StartDownloading()
	{
		if (!isInitialized)
		{
			Debug.LogError("StreamingAssetManual can't start downloading while being uninitialized.");
		}
		if (string.IsNullOrEmpty(url))
		{
			Debug.LogError("StreamedAsset is missing a reference.");
		}
		else
		{
			DownloadWhenPossible();
		}
	}

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
