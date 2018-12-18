using System;
using MV.Common;
using UnityEngine;

public class SoundLoader
{
	public delegate void UpdateSoundCallback();

	private string currentUrl = string.Empty;

	private string url = string.Empty;

	private AudioSource audioSource;

	public UpdateSoundCallback callback;

	public AudioSource AudioSource
	{
		get
		{
			return audioSource;
		}
		set
		{
			audioSource = value;
		}
	}

	public string Url
	{
		set
		{
			url = value;
		}
	}

	public void LoadSound()
	{
		if (url != currentUrl)
		{
			if (Urls.StreamingAssetUrlReady())
			{
				Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Remove(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(LoadSound));
				StopAndDestroySound();
				currentUrl = url;
				Download(currentUrl);
			}
			else
			{
				Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Combine(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(LoadSound));
			}
		}
		else if (audioSource != null)
		{
			callback();
		}
	}

	public void Download(string soundUrl)
	{
		if (soundUrl.Length != 0)
		{
			string path = StreamingAsset.DBUrlToServerUrl(StreamingAsset.AssetBundleUrl + soundUrl);
			AsyncWWWManager.UnsubscribeWWWRequest(OnDownloadFinished);
			AsyncWWWManager.WWWRequest(new CachedAssetBundleRequest(path, OnDownloadFinished, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}
		else
		{
			Debug.LogError("Could not find asset info for audio " + currentUrl);
		}
	}

	private void OnDownloadFinished(WWW www)
	{
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogError("Failed to download, www error: " + www.error);
			return;
		}
		StopAndDestroySound();
		AudioClip clip = StreamingAsset.UnpackBundle_Cached<AudioClip>(www);
		UpdateSound(clip);
	}

	public void Destroy()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(OnDownloadFinished);
	}

	private void UpdateSound(AudioClip clip)
	{
		AudioSource.clip = clip;
		if (callback != null)
		{
			callback();
		}
	}

	public void StopAndDestroySound()
	{
		if (AudioSource.isPlaying)
		{
			AudioSource.Stop();
		}
	}
}
