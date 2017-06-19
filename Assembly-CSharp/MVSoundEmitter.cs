using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class MVSoundEmitter : MVLogicObject, ILogicWorldObject
{
	private string currentUrl = string.Empty;

	private SoundEmitterObject soundEmitterObject;

	private MVNetworkGame Game => MVGameControllerBase.Game;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.SoundEmitter;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVSoundEmitter(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSoundEmitterPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		interactionFlags |= InteractionFlags.HasSettings | InteractionFlags.Sounds;
		soundEmitterObject = (SoundEmitterObject)component;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(soundEmitterObject.VisualObject);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, InputStateUpdateCallback);
		InitializeData();
		if (((string)Data["url"]).Length > 0)
		{
			LoadSound();
		}
	}

	public override void OnDataUpdate()
	{
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	public override void Reset()
	{
		base.Reset();
		LoadSound();
	}

	private void InitializeData()
	{
		if (Data.ContainsKey("url") && !Data["url"].ToString().StartsWith("file://"))
		{
			return;
		}
		StreamingAssetInfo streamingAssetInfo = null;
		foreach (StreamingAssetInfo value in MVGameControllerBase.Game.StreamingAssetInfoMap.Values)
		{
			if (value.StreamedAssetType == StreamingAssetType.AmbientAudio && value.ShopInfo.PriceGold == 0)
			{
				streamingAssetInfo = value;
				break;
			}
		}
		if (streamingAssetInfo != null)
		{
			Data["name"] = streamingAssetInfo.Name;
			Data["id"] = streamingAssetInfo.ProductID;
			Data["url"] = streamingAssetInfo.AssetPath;
		}
		else
		{
			Data["name"] = "ForestBirds";
			Data["id"] = 1;
			Data["url"] = "AmbientAudio/Nature/kgm_amb_forest.unity3d";
			Debug.LogError("Failed to get default streaming inventory data");
		}
	}

	private void InputStateUpdateCallback(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState != LogicInputState.Cold && logicInputState != LogicInputState.Hot && soundEmitterObject.AudioSource != null)
		{
			if (ShouldPlay() && !soundEmitterObject.AudioSource.isPlaying)
			{
				UpdateSound(soundEmitterObject.AudioSource.clip);
			}
			if (!ShouldPlay() && soundEmitterObject.AudioSource.isPlaying)
			{
				UpdateSound(soundEmitterObject.AudioSource.clip);
			}
		}
	}

	private void LoadSound()
	{
		if ((string)Data["url"] != currentUrl)
		{
			if (Urls.StreamingAssetUrlReady())
			{
				Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Remove(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(LoadSound));
				StopAndDestroySound();
				currentUrl = (string)Data["url"];
				StreamingAssetInfo saInfo = Game.StreamingAssetInfoMap.Values.FirstOrDefault((StreamingAssetInfo sai) => sai.AssetPath == currentUrl);
				Download(saInfo);
			}
			else
			{
				Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Combine(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(LoadSound));
			}
		}
		else if (soundEmitterObject.AudioSource != null)
		{
			UpdateSound(soundEmitterObject.AudioSource.clip);
		}
	}

	private void Download(StreamingAssetInfo saInfo)
	{
		if (saInfo != null)
		{
			string path = StreamingAsset.DBUrlToServerUrl(StreamingAsset.AssetBundleUrl + saInfo.RequestPath);
			AsyncWWWManager.WWWRequest(new CachedGetRequest(path, OnDownloadFinished, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}
		else
		{
			Debug.LogError("Could not find asset info for audio " + currentUrl);
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		AsyncWWWManager.UnsubscribeWWWRequest(OnDownloadFinished);
	}

	public void OnDownloadFinished(WWW www)
	{
		if (string.IsNullOrEmpty(www.error))
		{
			StopAndDestroySound();
			AudioClip clip = StreamingAsset.UnpackBundle<AudioClip>(www);
			AudioSource audioSource = soundEmitterObject.AudioSource;
			Data["loop"] = audioSource.loop;
			UpdateSound(clip);
		}
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		StopAndDestroySound();
		return base.Delete(worldObjectClientManager, ref errorText);
	}

	private void UpdateSound(AudioClip clip)
	{
		AudioSource audioSource = soundEmitterObject.AudioSource;
		audioSource.clip = clip;
		audioSource.volume = (float)Data["volume"];
		audioSource.pitch = (float)Data["pitch"];
		audioSource.loop = (bool)Data["loop"];
		audioSource.rolloffMode = AudioRolloffMode.Custom;
		audioSource.minDistance = GetMinDistanceFromRangeAmbient((SoundRangeDistance)(int)Data["range"]);
		audioSource.maxDistance = GetMaxDistanceFromRangeAmbient((SoundRangeDistance)(int)Data["range"]);
		if ((string)Data["url"] == currentUrl || ShouldPlay() != audioSource.isPlaying)
		{
			if (ShouldPlay())
			{
				audioSource.Play();
			}
			else
			{
				audioSource.Stop();
			}
		}
	}

	private bool ShouldPlay()
	{
		if ((bool)Data["mute"])
		{
			return false;
		}
		if (InputSignalReceiver.CurrentlyIsHot)
		{
			return true;
		}
		return false;
	}

	private void StopAndDestroySound()
	{
		AudioSource audioSource = soundEmitterObject.AudioSource;
		if (audioSource.isPlaying)
		{
			audioSource.Stop();
		}
	}

	private static float GetMinDistanceFromRangeAmbient(SoundRangeDistance range)
	{
		return range switch
		{
			SoundRangeDistance.Short => 7f, 
			SoundRangeDistance.Medium => 12f, 
			SoundRangeDistance.Long => 25f, 
			_ => 5f, 
		};
	}

	private static float GetMaxDistanceFromRangeAmbient(SoundRangeDistance range)
	{
		return range switch
		{
			SoundRangeDistance.Short => 14f, 
			SoundRangeDistance.Medium => 30f, 
			SoundRangeDistance.Long => 65f, 
			_ => 20f, 
		};
	}
}
