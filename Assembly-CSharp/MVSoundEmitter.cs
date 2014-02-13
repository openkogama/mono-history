using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Localize;
using MV.Common;
using UnityEngine;

public class MVSoundEmitter : MVLogicObject
{
	private const string prefabPath = "Prefabs/SoundEmitterObject";

	private string currentUrl = string.Empty;

	private AudioSource currentSrc;

	private MVNetworkGame Game => MVGameController.Instance.Game;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVSoundEmitter(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/SoundEmitterObject", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
	}

	public override void Initialize()
	{
		base.Initialize();
		if (!Data.ContainsKey("url"))
		{
			StreamingAssetInfo streamingAssetInfo = null;
			foreach (StreamingAssetInfo value in MVGameController.Instance.Game.StreamingAssetInfoMap.Values)
			{
				if (value.StreamedAssetType == StreamingAssetType.AmbientAudio && value.ShopInfo.PriceGold == 0)
				{
					streamingAssetInfo = value;
					break;
				}
			}
			if (streamingAssetInfo == null)
			{
				Data["name"] = "ForestBirds";
				Data["id"] = 1;
				Data["url"] = "AmbientAudio/Nature/kgm_amb_forest.unity3d";
				Debug.LogError((object)"Failed to get default streaming inventory data");
				return;
			}
			Data["name"] = streamingAssetInfo.Name;
			Data["id"] = streamingAssetInfo.ProductID;
			Data["url"] = streamingAssetInfo.AssetPath;
		}
		if (((string)Data["url"]).Length > 0)
		{
			OnDataUpdate();
		}
	}

	public override void OnInputStateChanged()
	{
		if ((Object)(object)currentSrc != (Object)null)
		{
			if (ShouldPlay() && !currentSrc.isPlaying)
			{
				UpdateSound(currentSrc);
			}
			if (!ShouldPlay() && currentSrc.isPlaying)
			{
				UpdateSound(currentSrc);
			}
		}
	}

	public override void OnInputLinkChanged()
	{
		OnInputStateChanged();
	}

	public override void OnDataUpdate()
	{
		if ((string)Data["url"] != currentUrl)
		{
			StopAndDestroySound();
			currentUrl = (string)Data["url"];
			StreamingAssetInfo streamingAssetInfo = Game.StreamingAssetInfoMap.Values.FirstOrDefault((StreamingAssetInfo sai) => sai.AssetPath == currentUrl);
			if (streamingAssetInfo != null)
			{
				MVGameController.Instance.Game.AssetBundleMgr.RequestAssetBundle(streamingAssetInfo.RequestPath, StreamingAssetCallback, autoRetry: true, highPriority: true);
			}
			else
			{
				Debug.LogError((object)("Could not find asset info for audio " + currentUrl));
			}
		}
		else if ((Object)(object)currentSrc != (Object)null)
		{
			UpdateSound(currentSrc);
		}
	}

	public void StreamingAssetCallback(AssetBundle assetBundle, string bundlePath)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected Obj, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		MVGameController.Instance.Game.AssetBundleMgr.UnsubscribeBundleCallback(bundlePath, StreamingAssetCallback);
		Debug.Log((object)("SOUND EMITTER StreamingAssetCallback, url = " + bundlePath + ", currentUrl = " + currentUrl));
		if ((Object)(object)GameObject != (Object)null && (Object)(object)assetBundle != (Object)null)
		{
			StopAndDestroySound();
			GameObject val = (GameObject)Object.Instantiate(assetBundle.mainAsset);
			AudioSource component = val.GetComponent<AudioSource>();
			((Component)component).transform.parent = transform;
			((Component)component).transform.position = transform.position;
			Data["loop"] = component.loop;
			UpdateSound(component);
		}
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref TextSlotIndex errorTextIndex)
	{
		StopAndDestroySound();
		return base.Delete(worldObjectClientManager, ref errorTextIndex);
	}

	private void UpdateSound(AudioSource source)
	{
		try
		{
			currentSrc = source;
			source.playOnAwake = false;
			source.volume = (float)Data["volume"];
			source.pitch = (float)Data["pitch"];
			source.loop = (bool)Data["loop"];
			source.rolloffMode = (AudioRolloffMode)2;
			source.dopplerLevel = 0f;
			source.minDistance = GetMinDistanceFromRangeAmbient((SoundRangeDistance)(int)Data["range"]);
			source.maxDistance = GetMaxDistanceFromRangeAmbient((SoundRangeDistance)(int)Data["range"]);
			if ((string)Data["url"] == currentUrl || ShouldPlay() != source.isPlaying)
			{
				if (ShouldPlay())
				{
					source.Play();
				}
				else
				{
					source.Stop();
				}
			}
		}
		catch (Exception arg)
		{
			string text = string.Empty;
			foreach (DictionaryEntry datum in Data)
			{
				string empty = string.Empty;
				text += string.Format(arg2: (datum.Value == null) ? "null" : datum.Value.GetType().ToString(), format: "Key: {0}, ValueType: {1}, Value: {2}\n", arg0: datum.Key, arg1: datum.Value);
			}
			int planetID = MVGameController.Instance.PlanetID;
			string text2 = $"SoundEmitter error on planet: {planetID}. Data {text}. Exception {arg}";
			Debug.LogError((object)text2);
		}
	}

	private bool ShouldPlay()
	{
		if ((bool)Data["mute"])
		{
			return false;
		}
		if (InputLinkRefs.Count == 0)
		{
			return true;
		}
		return InputState;
	}

	private void StopAndDestroySound()
	{
		if ((Object)(object)currentSrc != (Object)null)
		{
			if (currentSrc.isPlaying)
			{
				currentSrc.Stop();
			}
			Object.Destroy((Object)(object)((Component)currentSrc).gameObject);
			currentSrc = null;
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
