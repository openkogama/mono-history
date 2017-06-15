using UnityEngine;

public class StreamedAudioClipToAudioSource : StreamingAsset<AudioClip, AudioClip>
{
	[SerializeField]
	[Header("Dependencies")]
	protected AudioSource audioSource;

	public void Reset()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	protected override void OnAssetSet()
	{
		bool isPlaying = audioSource.isPlaying;
		audioSource.clip = Asset;
		if (isPlaying && audioSource.loop)
		{
			audioSource.Play();
		}
	}
}
