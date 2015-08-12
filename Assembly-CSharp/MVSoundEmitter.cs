using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class MVSoundEmitter : MVLogicObject
{
	private const string prefabPath = "Prefabs/SoundEmitterObject";

	private string currentUrl = string.Empty;

	private AudioSource currentSrc;

	private MVNetworkGame Game => MVGameController.Game;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVSoundEmitter(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/SoundEmitterObject", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
	}

	public override void Initialize()
	{
		base.Initialize();
		if (!Data.ContainsKey("url") || Data["url"].ToString().StartsWith("file://"))
		{
			StreamingAssetInfo streamingAssetInfo = null;
			foreach (StreamingAssetInfo value in MVGameController.Game.StreamingAssetInfoMap.Values)
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
				Debug.LogError("Failed to get default streaming inventory data");
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
		if (currentSrc != null)
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
				AsyncWWWManager.WWWRequest(new StreamingAssetRequest(Urls.StreamingAssets + streamingAssetInfo.RequestPath, StreamingAssetCallback));
			}
			else
			{
				Debug.LogError("Could not find asset info for audio " + currentUrl);
			}
		}
		else if (currentSrc != null)
		{
			UpdateSound(currentSrc);
		}
	}

	public void StreamingAssetCallback(WWW www)
	{
		try
		{
			Validate(www);
			StopAndDestroySound();
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(www.assetBundle.mainAsset);
			AudioSource component = gameObject.GetComponent<AudioSource>();
			component.transform.parent = transform;
			component.transform.position = transform.position;
			Data["loop"] = component.loop;
			UpdateSound(component);
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed sound update" + ex);
		}
	}

	private void Validate(WWW www)
	{
		if (GameObject == null)
		{
			throw new Exception("gameObject == null");
		}
		if (www == null)
		{
			throw new Exception("www== null");
		}
		if (www.assetBundle == null)
		{
			throw new Exception("www.assetBundle == null");
		}
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		StopAndDestroySound();
		return base.Delete(worldObjectClientManager, ref errorText);
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
			source.rolloffMode = AudioRolloffMode.Custom;
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
			foreach (KeyValuePair<object, object> datum in Data)
			{
				string empty = string.Empty;
				text += string.Format(arg2: (datum.Value == null) ? "null" : datum.Value.GetType().ToString(), format: "Key: {0}, ValueType: {1}, Value: {2}\n", arg0: datum.Key, arg1: datum.Value);
			}
			int planetID = MVGameController.Game.gameSessionData.planetID;
			string message = $"SoundEmitter error on planet: {planetID}. Data {text}. Exception {arg}";
			Debug.LogError(message);
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
		if (currentSrc != null)
		{
			if (currentSrc.isPlaying)
			{
				currentSrc.Stop();
			}
			UnityEngine.Object.Destroy(currentSrc.gameObject);
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
