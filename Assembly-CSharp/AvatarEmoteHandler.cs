using System;
using System.Collections.Generic;
using UnityEngine;

public class AvatarEmoteHandler : MonoBehaviour
{
	private AvatarLimbManager limbManager;

	private Dictionary<EmoteTypes, AvatarEmoteRecogniser> emoteRecognisers = new Dictionary<EmoteTypes, AvatarEmoteRecogniser>();

	private Dictionary<EmoteTypes, EmoteData> emoteDatas = new Dictionary<EmoteTypes, EmoteData>();

	private EmoteData currentRunningEmoteData;

	private bool isActive = true;

	public void Initialize(AvatarLimbManager limbManager, AvatarLookDirectionHandler lookDirectionHandler, AvatarPointingHandler pointingHandler, AvatarHeadRotationHandler headRotationHandler, bool isLocal)
	{
		this.limbManager = limbManager;
		CreateLimbEvents(limbManager, lookDirectionHandler, pointingHandler, headRotationHandler, isLocal);
	}

	private void CreateLimbEvents(AvatarLimbManager limbManager, AvatarLookDirectionHandler lookDirectionHandler, AvatarPointingHandler pointingHandler, AvatarHeadRotationHandler headRotationHandler, bool isLocal)
	{
		if (isLocal)
		{
			AvatarEmoteRecogniser avatarEmoteRecogniser = new AvatarEmoteRecogniser();
			avatarEmoteRecogniser.Initlialize(limbManager, 5f, 2f, 4, shouldRecognisePositiveAngleFirst: true, isActive: false);
			lookDirectionHandler.OnLookDirectionYawChange = (Action<float>)Delegate.Combine(lookDirectionHandler.OnLookDirectionYawChange, new Action<float>(avatarEmoteRecogniser.HandleNewAngle));
			avatarEmoteRecogniser.OnStartEvent = (Action)Delegate.Combine(avatarEmoteRecogniser.OnStartEvent, new Action(OnWaveEmoteStart));
			pointingHandler.OnIsPointingChange = (Action<bool>)Delegate.Combine(pointingHandler.OnIsPointingChange, new Action<bool>(avatarEmoteRecogniser.SetIsActive));
			emoteRecognisers.Add(EmoteTypes.wave, avatarEmoteRecogniser);
			AvatarEmoteRecogniser avatarEmoteRecogniser2 = new AvatarEmoteRecogniser();
			avatarEmoteRecogniser2.Initlialize(limbManager, 5f, 2f, 4, shouldRecognisePositiveAngleFirst: true, isActive: true);
			lookDirectionHandler.OnLookDirectionYawChange = (Action<float>)Delegate.Combine(lookDirectionHandler.OnLookDirectionYawChange, new Action<float>(avatarEmoteRecogniser2.HandleNewAngle));
			avatarEmoteRecogniser2.OnStartEvent = (Action)Delegate.Combine(avatarEmoteRecogniser2.OnStartEvent, new Action(OnShakeEmoteStart));
			emoteRecognisers.Add(EmoteTypes.Shake, avatarEmoteRecogniser2);
			AvatarEmoteRecogniser avatarEmoteRecogniser3 = new AvatarEmoteRecogniser();
			avatarEmoteRecogniser3.Initlialize(limbManager, 5f, 2f, 4, shouldRecognisePositiveAngleFirst: false, isActive: true);
			lookDirectionHandler.OnLookDirectionPitchChange = (Action<float>)Delegate.Combine(lookDirectionHandler.OnLookDirectionPitchChange, new Action<float>(avatarEmoteRecogniser3.HandleNewAngle));
			avatarEmoteRecogniser3.OnStartEvent = (Action)Delegate.Combine(avatarEmoteRecogniser3.OnStartEvent, new Action(OnNodEmoteStart));
			emoteRecognisers.Add(EmoteTypes.Nod, avatarEmoteRecogniser3);
		}
		AvatarShakeEmote avatarShakeEmote = new AvatarShakeEmote();
		EmoteData value = CreateEmoteData(avatarShakeEmote, 2f, 1);
		avatarShakeEmote.OnEmoteEnd = (Action<EmoteTypes>)Delegate.Combine(avatarShakeEmote.OnEmoteEnd, new Action<EmoteTypes>(headRotationHandler.ResetIdleTimer));
		emoteDatas.Add(EmoteTypes.Shake, value);
		AvatarNodEmote avatarNodEmote = new AvatarNodEmote();
		EmoteData value2 = CreateEmoteData(avatarNodEmote, 2f, 1);
		avatarNodEmote.OnEmoteEnd = (Action<EmoteTypes>)Delegate.Combine(avatarNodEmote.OnEmoteEnd, new Action<EmoteTypes>(headRotationHandler.ResetIdleTimer));
		emoteDatas.Add(EmoteTypes.Nod, value2);
		AvatarWaveEmote emote = new AvatarWaveEmote();
		EmoteData value3 = CreateEmoteData(emote, 1.5f, 2);
		emoteDatas.Add(EmoteTypes.wave, value3);
	}

	private EmoteData CreateEmoteData(AvatarEmote emote, float lifeTime, short priority)
	{
		emote.Initialize(limbManager, lifeTime);
		emote.OnEmoteEnd = (Action<EmoteTypes>)Delegate.Combine(emote.OnEmoteEnd, new Action<EmoteTypes>(OnEmoteEnd));
		EmoteData emoteData = new EmoteData();
		emoteData.emote = emote;
		emoteData.priority = priority;
		return emoteData;
	}

	public void UpdateEmotes()
	{
		foreach (KeyValuePair<EmoteTypes, AvatarEmoteRecogniser> emoteRecogniser in emoteRecognisers)
		{
			emoteRecogniser.Value.Update();
		}
		foreach (KeyValuePair<EmoteTypes, EmoteData> emoteData in emoteDatas)
		{
			emoteData.Value.emote.Update();
		}
	}

	private void OnShakeEmoteStart()
	{
		if (CanStartEmote(emoteDatas[EmoteTypes.Shake]))
		{
			StartEmote(EmoteTypes.Shake);
			MVGameControllerBase.OperationRequests.StartHeadShake();
			limbManager.DelayHeadRotationNetworkMessage(emoteDatas[EmoteTypes.Shake].emote.LifeTime);
		}
	}

	private void OnNodEmoteStart()
	{
		if (CanStartEmote(emoteDatas[EmoteTypes.Nod]))
		{
			StartEmote(EmoteTypes.Nod);
			MVGameControllerBase.OperationRequests.StartHeadNod();
			limbManager.DelayHeadRotationNetworkMessage(emoteDatas[EmoteTypes.Nod].emote.LifeTime);
		}
	}

	private void OnWaveEmoteStart()
	{
		if (CanStartEmote(emoteDatas[EmoteTypes.wave]))
		{
			StartEmote(EmoteTypes.wave);
			MVGameControllerBase.OperationRequests.StartWave();
			limbManager.DelayHeadRotationNetworkMessage(emoteDatas[EmoteTypes.wave].emote.LifeTime);
			limbManager.DelayPointingNetworkMessage(emoteDatas[EmoteTypes.wave].emote.LifeTime);
		}
	}

	private bool CanStartEmote(EmoteData emoteData)
	{
		if (!isActive)
		{
			return false;
		}
		if (currentRunningEmoteData != null)
		{
			if (currentRunningEmoteData.priority >= emoteData.priority)
			{
				return false;
			}
			currentRunningEmoteData.emote.StopEmote();
			return true;
		}
		return true;
	}

	public void TryStartEmote(EmoteTypes emoteType)
	{
		if (CanStartEmote(emoteDatas[emoteType]))
		{
			StartEmote(emoteType);
		}
	}

	private void StartEmote(EmoteTypes emoteType)
	{
		if (emoteDatas.ContainsKey(emoteType))
		{
			emoteDatas[emoteType].emote.StartEmote();
			currentRunningEmoteData = emoteDatas[emoteType];
		}
	}

	public void StartEmoteAndNetworkIt(EmoteTypes emoteType)
	{
		switch (emoteType)
		{
		case EmoteTypes.Shake:
			OnShakeEmoteStart();
			break;
		case EmoteTypes.Nod:
			OnNodEmoteStart();
			break;
		case EmoteTypes.wave:
			OnWaveEmoteStart();
			break;
		default:
			Debug.LogError(string.Concat("Could not start and network ", emoteType, ". Please add it to the StartEmoteAndNetworkIt function in AvatarEmoteHandler."));
			break;
		}
	}

	private void OnEmoteEnd(EmoteTypes emoteType)
	{
		if (currentRunningEmoteData == emoteDatas[emoteType])
		{
			currentRunningEmoteData = null;
		}
	}

	public void StopAllEmotes()
	{
		foreach (KeyValuePair<EmoteTypes, EmoteData> emoteData in emoteDatas)
		{
			emoteData.Value.emote.StopEmote();
		}
	}

	private void OnEnable()
	{
		isActive = true;
	}

	private void OnDisable()
	{
		isActive = false;
		StopAllEmotes();
	}
}
