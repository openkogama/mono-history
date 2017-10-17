using UnityEngine;

public class StreamedAudioClipToAvatarWaterRippleEffect : StreamingAsset<AudioClip, AudioClip>
{
	[SerializeField]
	[Header("Dependencies")]
	protected AvatarWaterRippleEffect rippleEffect;

	public void Reset()
	{
		if (rippleEffect == null)
		{
			rippleEffect = GetComponent<AvatarWaterRippleEffect>();
		}
	}

	protected override void OnAssetSet()
	{
		rippleEffect.SetSplashSound(Asset);
	}
}
