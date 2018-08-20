using System.Collections.Generic;
using UnityEngine;

public class MVSoundEmitter : MVLogicObject, ILogicWorldObject
{
	private SoundEmitterObject soundEmitterObject;

	private SoundLoader soundLoader = new SoundLoader();

	private const string defaultUrl = "AmbientAudio/Nature/kgm_amb_forest.unity3d";

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.SoundEmitter;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVSoundEmitter(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSoundEmitterPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings | InteractionFlags.Sounds;
		soundEmitterObject = (SoundEmitterObject)component;
	}

	public override void Initialize()
	{
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, InputStateUpdateCallback);
		soundEmitterObject.SoundCheck.Initialize(this);
		base.Initialize();
		soundLoader.AudioSource = soundEmitterObject.AudioSource;
		soundLoader.callback = UpdateSound;
		SetupCulling(soundEmitterObject.VisualObject);
		if (!Data.ContainsKey("url"))
		{
			soundLoader.Url = "AmbientAudio/Nature/kgm_amb_forest.unity3d";
			soundLoader.LoadSound();
		}
		else if (((string)Data["url"]).Length > 0)
		{
			soundLoader.Url = (string)Data["url"];
			soundLoader.LoadSound();
		}
	}

	public override void OnDataUpdate()
	{
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
		if (!Data.ContainsKey("url"))
		{
			soundLoader.Url = "AmbientAudio/Nature/kgm_amb_forest.unity3d";
			soundLoader.LoadSound();
		}
		else if (((string)Data["url"]).Length > 0)
		{
			soundLoader.Url = (string)Data["url"];
			soundLoader.LoadSound();
		}
		else if (((string)Data["url"]).Length <= 0)
		{
			Data["url"] = "AmbientAudio/Nature/kgm_amb_forest.unity3d";
			soundLoader.Url = (string)Data["url"];
			soundLoader.LoadSound();
		}
	}

	private void InputStateUpdateCallback(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState != LogicInputState.Cold && logicInputState != LogicInputState.Hot && soundEmitterObject.AudioSource != null)
		{
			if (ShouldPlay() && !soundEmitterObject.AudioSource.isPlaying)
			{
				UpdateSound();
			}
			if (!ShouldPlay() && soundEmitterObject.AudioSource.isPlaying)
			{
				UpdateSound();
			}
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
		audioSource.loop = (bool)Data["loop"];
		audioSource.rolloffMode = AudioRolloffMode.Custom;
		audioSource.minDistance = GetMinDistanceFromRangeAmbient((SoundRangeDistance)(int)Data["range"]);
		audioSource.maxDistance = GetMaxDistanceFromRangeAmbient((SoundRangeDistance)(int)Data["range"]);
		if (ShouldPlay() != audioSource.isPlaying)
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
