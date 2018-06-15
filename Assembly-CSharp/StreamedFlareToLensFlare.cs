using UnityEngine;

public class StreamedFlareToLensFlare : StreamingAsset<Flare, Flare>
{
	[SerializeField]
	private LensFlare lensFlare;

	protected override void OnAssetSet()
	{
		lensFlare.flare = Asset;
	}

	protected void Reset()
	{
		if (lensFlare == null)
		{
			lensFlare = GetComponent<LensFlare>();
		}
		if (lensFlare == null)
		{
			lensFlare = GetComponentInParent<LensFlare>();
		}
	}
}
