using System;
using MV.WorldObject;
using UnityEngine;

public class AvatarLimbManagerRemote : AvatarLimbManager
{
	private class AvatarHeadRotationHandlerRemote : AvatarHeadRotationHandler
	{
		private Quaternion remoteYawRotation;

		private Quaternion remotePitchRotation;

		public override void Initialize(AvatarLimbManager limbManager, LimbRotator limbRotator, AvatarLookDirectionHandler lookDirectionHandler)
		{
			remoteYawRotation = Quaternion.identity;
			remotePitchRotation = Quaternion.identity;
			lookDirectionHandler.OnRotationChange = (Action)Delegate.Combine(lookDirectionHandler.OnRotationChange, new Action(ResetIdleTimer));
			base.Initialize(limbManager, limbRotator, lookDirectionHandler);
		}

		public override void UpdateRotation()
		{
			RotateTorso(remoteYawRotation, remotePitchRotation);
			RotateHead(remoteYawRotation, remotePitchRotation);
			base.UpdateRotation();
		}

		public void SetRotationRemotely(float yaw, float pitch)
		{
			ResetIdleTimer();
			remoteYawRotation.eulerAngles = new Vector3(0f, yaw, 0f);
			remoteYawRotation = ClampQuaternion(remoteYawRotation);
			remotePitchRotation.eulerAngles = new Vector3(pitch, 0f, 0f);
			remotePitchRotation = ClampQuaternion(remotePitchRotation);
		}
	}

	private class AvatarLimbDataManagerRemote
	{
		private float newHeadYawValue;

		private float newHeadPitchValue;

		private float newPointYawValue;

		private float newPointPitchValue;

		private AvatarLimbManagerRemote limbManager;

		public void Initialize(LimbRotationRuntimeData limbRotationRuntimeData, AvatarLimbManagerRemote limbManager)
		{
			this.limbManager = limbManager;
			MVRuntimeDataVariable headRotationYaw = limbRotationRuntimeData.HeadRotationYaw;
			headRotationYaw.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(headRotationYaw.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnHeadYawChange));
			MVRuntimeDataVariable headRotationPitch = limbRotationRuntimeData.HeadRotationPitch;
			headRotationPitch.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(headRotationPitch.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnHeadPitchChange));
			MVRuntimeDataVariable pointRotationYaw = limbRotationRuntimeData.PointRotationYaw;
			pointRotationYaw.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(pointRotationYaw.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnPointYawChange));
			MVRuntimeDataVariable pointRotationPitch = limbRotationRuntimeData.PointRotationPitch;
			pointRotationPitch.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(pointRotationPitch.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnPointPitchChange));
			MVRuntimeDataVariable emote = limbRotationRuntimeData.Emote;
			emote.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(emote.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnEmoteDataChange));
		}

		private void OnHeadYawChange(object headYaw)
		{
			newHeadYawValue = (float)headYaw;
			UpdateHeadRotation();
		}

		private void OnHeadPitchChange(object headPitch)
		{
			newHeadPitchValue = (float)headPitch;
			UpdateHeadRotation();
		}

		private void UpdateHeadRotation()
		{
			limbManager.UpdateHeadRotationRemotely(newHeadYawValue, newHeadPitchValue);
		}

		private void OnPointYawChange(object pointYaw)
		{
			newPointYawValue = (float)pointYaw;
			UpdatePointRotation();
		}

		private void OnPointPitchChange(object pointPitch)
		{
			newPointPitchValue = (float)pointPitch;
			UpdatePointRotation();
		}

		private void UpdatePointRotation()
		{
			limbManager.UpdatePointingRemotely(newPointYawValue, newPointPitchValue);
		}

		private void OnEmoteDataChange(object newEmoteData)
		{
			EmoteTypes emoteType = (EmoteTypes)(int)newEmoteData;
			limbManager.StartEmote(emoteType);
		}
	}

	private class AvatarPointingHandlerRemote : AvatarPointingHandler
	{
		private Quaternion remoteYawRotation;

		private Quaternion remotePitchRotation;

		public override void Initialize(AvatarLimbManager limbManager, LimbRotator limbRotator, AvatarEnabledChangeHandler enableChangeHandler)
		{
			base.Initialize(limbManager, limbRotator, enableChangeHandler);
			remoteYawRotation = Quaternion.identity;
			remotePitchRotation = Quaternion.identity;
			pointingDuration = 1.5f;
			shouldPoint = false;
		}

		public override void UpdatePointing(Vector3 localLookDirection)
		{
			if (shouldPoint)
			{
				HandlePointing(remoteYawRotation, remotePitchRotation);
			}
			base.UpdatePointing(localLookDirection);
		}

		public void SetRotationRemotely(float yaw, float pitch)
		{
			if (isActive)
			{
				if (yaw == Quaternion.identity.eulerAngles.y && pitch == Quaternion.identity.eulerAngles.x)
				{
					shouldPoint = false;
					StopPointing();
					return;
				}
				remoteYawRotation.eulerAngles = new Vector3(0f, yaw, 0f);
				remotePitchRotation.eulerAngles = new Vector3(pitch, 0f, 0f);
				elapsedPointingTime = pointingDuration;
				shouldPoint = true;
			}
		}
	}

	private AvatarHeadRotationHandlerRemote headRotationHandler;

	private AvatarPointingHandlerRemote pointingHandler;

	private AvatarLimbDataManagerRemote dataManager;

	public override void Initialize(MVWorldObjectClient avatarWO, MVBody body, AvatarEnabledChangeHandler enabledChangeHandler, LimbRotationRuntimeData limbRotationRuntimeData)
	{
		base.Initialize(avatarWO, body, enabledChangeHandler, limbRotationRuntimeData);
		headRotationHandler = new AvatarHeadRotationHandlerRemote();
		headRotationHandler.Initialize(this, limbRotator, lookDirectionHandler);
		pointingHandler = new AvatarPointingHandlerRemote();
		pointingHandler.Initialize(this, limbRotator, enabledChangeHandler);
		emoteHandler = new AvatarEmoteHandler();
		emoteHandler.Initialize(this, lookDirectionHandler, pointingHandler, headRotationHandler, limbRotator, enabledChangeHandler);
		AvatarEmoteHandler avatarEmoteHandler = emoteHandler;
		avatarEmoteHandler.OnEmoteStart = (Action<string>)Delegate.Combine(avatarEmoteHandler.OnEmoteStart, new Action<string>(OnStartEmote));
		dataManager = new AvatarLimbDataManagerRemote();
		dataManager.Initialize(limbRotationRuntimeData, this);
	}

	public override void UpdateLimbRotations(Vector3 lookDirection)
	{
		base.UpdateLimbRotations(lookDirection);
		Vector3 localLookDirection = lookDirectionHandler.LocalLookDirection;
		emoteHandler.UpdateEmotes();
		headRotationHandler.UpdateRotation();
		pointingHandler.UpdatePointing(localLookDirection);
		CheckAvatarRotation();
		limbRotator.UpdateLimbs();
	}

	public void UpdateHeadRotationRemotely(float yaw, float pitch)
	{
		headRotationHandler.SetRotationRemotely(yaw, pitch);
	}

	public void UpdatePointingRemotely(float yaw, float pitch)
	{
		pointingHandler.SetRotationRemotely(yaw, pitch);
	}

	public override void StartEmote(EmoteTypes emoteType)
	{
		emoteHandler.TryStartEmote(emoteType);
	}

	private void OnHeadRotationDataChange(object newHeadRotationData)
	{
		Quaternion quaternion = QuaternionCompression.ToQuaternion((byte[])newHeadRotationData);
		UpdateHeadRotationRemotely(quaternion.eulerAngles.y, quaternion.eulerAngles.x);
	}

	private void OnPointRotationDataChange(object newPointRotationData)
	{
		Quaternion quaternion = QuaternionCompression.ToQuaternion((byte[])newPointRotationData);
		UpdateHeadRotationRemotely(quaternion.eulerAngles.y, quaternion.eulerAngles.x);
		UpdatePointingRemotely(quaternion.eulerAngles.y, quaternion.eulerAngles.x);
	}
}
