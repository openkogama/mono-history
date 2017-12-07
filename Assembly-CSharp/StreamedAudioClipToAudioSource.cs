using UnityEngine;

public class StreamedAudioClipToAudioSource : StreamingAsset<AudioClip, AudioClip>
{
	[Header("Dependencies")]
	[SerializeField]
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
		bool flag = audioSource.isPlaying && audioSource.loop;
		bool flag2 = audioSource.playOnAwake && audioSource.clip == null;
		audioSource.clip = Asset;
		if (flag || flag2)
		{
			audioSource.Play();
		}
	}
}
