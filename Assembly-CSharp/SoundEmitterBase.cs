using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public abstract class SoundEmitterBase : MVLogicObject
{
	protected string currentUrl = string.Empty;

	protected SoundEmitterObject soundEmitterObject;

	protected SoundEmitterBase(Dictionary<object, object> data, ObjectPrefab soundObject, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, soundObject, worldObjects)
	{
		soundEmitterObject = (SoundEmitterObject)component;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(soundEmitterObject.VisualObject);
		if (((string)Data["url"]).Length > 0)
		{
			LoadSound();
		}
	}

	protected void LoadSound()
	{
		if ((string)Data["url"] != currentUrl)
		{
			if (Urls.StreamingAssetUrlReady())
			{
				Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Remove(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(LoadSound));
				StopAndDestroySound();
				currentUrl = (string)Data["url"];
				Download(currentUrl);
			}
			else
			{
				Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Combine(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(LoadSound));
			}
		}
		else if (soundEmitterObject.AudioSource != null)
		{
			UpdateSound(null);
		}
	}

	protected void Download(string saInfo)
	{
		if (saInfo.Length != 0)
		{
			string path = StreamingAsset.DBUrlToServerUrl(StreamingAsset.AssetBundleUrl + saInfo);
			AsyncWWWManager.UnsubscribeWWWRequest(OnDownloadFinished);
			AsyncWWWManager.WWWRequest(new CachedGetRequest(path, OnDownloadFinished, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}
		else
		{
			Debug.LogError("Could not find asset info for audio " + currentUrl);
		}
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		StopAndDestroySound();
		return base.Delete(worldObjectClientManager, ref errorText);
	}

	public override void Destroy()
	{
		base.Destroy();
		AsyncWWWManager.UnsubscribeWWWRequest(OnDownloadFinished);
	}

	protected void OnDownloadFinished(WWW www)
	{
		if (string.IsNullOrEmpty(www.error))
		{
			StopAndDestroySound();
			AudioClip clip = StreamingAsset.UnpackBundle<AudioClip>(www);
			UpdateSound(clip);
		}
	}

	public virtual void UpdateSound(AudioClip clip)
	{
		AudioSource audioSource = soundEmitterObject.AudioSource;
		if (clip != null)
		{
			audioSource.clip = clip;
		}
		audioSource.volume = (float)Data["volume"];
		audioSource.pitch = (float)Data["pitch"];
	}

	protected void StopAndDestroySound()
	{
		AudioSource audioSource = soundEmitterObject.AudioSource;
		if (audioSource.isPlaying)
		{
			audioSource.Stop();
		}
	}
}
