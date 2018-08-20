using System.Collections.Generic;
using UnityEngine;

public class MVGlobalSoundEmitter : MVLogicObject
{
	private SoundEmitterObject soundEmitterObject;

	private SoundLoader soundLoader = new SoundLoader();

	private const string defaultUrl = "AmbientAudio/Music/slowstones.unity3d";

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.GlobalSoundEmitter;

	public MVGlobalSoundEmitter(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVGlobalSoundEmitterPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings | InteractionFlags.GlobalSounds;
		interactionFlags &= ~InteractionFlags.CanClone;
		soundEmitterObject = (SoundEmitterObject)component;
	}

	public override void Initialize()
	{
		soundEmitterObject.SoundCheck.Initialize(this);
		base.Initialize();
		soundLoader.AudioSource = soundEmitterObject.AudioSource;
		soundLoader.callback = UpdateSound;
		SetupCulling(soundEmitterObject.VisualObject);
		if (((string)Data["url"]).Length > 0)
		{
			soundLoader.Url = (string)Data["url"];
			soundLoader.LoadSound();
		}
	}

	public override void OnDataUpdate()
	{
		if (((string)Data["url"]).Length > 0)
		{
			soundLoader.Url = (string)Data["url"];
			soundLoader.LoadSound();
		}
		else if (((string)Data["url"]).Length <= 0)
		{
			Data["url"] = "AmbientAudio/Music/slowstones.unity3d";
			soundLoader.Url = (string)Data["url"];
			soundLoader.LoadSound();
		}
	}

	public void UpdateSound()
	{
		if (soundEmitterObject == null)
		{
			Debug.LogError("soundEmitterObject is null");
			return;
		}
		AudioSource audioSource = soundEmitterObject.AudioSource;
		if (audioSource == null)
		{
			Debug.LogError("audioSource for soundEmitterObject is null");
			return;
		}
		audioSource.volume = (float)Data["volume"];
		audioSource.pitch = (float)Data["pitch"];
		audioSource.spatialBlend = 0f;
		audioSource.loop = true;
		if (!audioSource.isPlaying)
		{
			audioSource.Play();
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		soundLoader.Destroy();
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		soundLoader.StopAndDestroySound();
		return base.Delete(worldObjectClientManager, ref errorText);
	}

	public override bool IsSingletonObject()
	{
		return true;
	}
}
