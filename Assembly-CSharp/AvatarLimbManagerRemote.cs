using System;
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

	private class AvatarPointingHandlerRemote : AvatarPointingHandler
	{
		private Quaternion remoteYawRotation;

		private Quaternion remotePitchRotation;

		public override void Initialize(AvatarLimbManager limbManager, LimbRotator limbRotator)
		{
			base.Initialize(limbManager, limbRotator);
			remoteYawRotation = Quaternion.identity;
			remotePitchRotation = Quaternion.identity;
			pointingDuration = 1.5f;
		}

		public override void UpdatePointing(Vector3 localLookDirection)
		{
			if (elapsedPointingTime > 0f)
			{
				if (shouldPoint)
				{
					HandlePointing(remoteYawRotation, remotePitchRotation);
				}
				else
				{
					StopPointing();
				}
			}
			base.UpdatePointing(localLookDirection);
		}

		public void SetRotationRemotely(float yaw, float pitch)
		{
			if (isActive)
			{
				remoteYawRotation.eulerAngles = new Vector3(0f, yaw, 0f);
				remotePitchRotation.eulerAngles = new Vector3(pitch, 0f, 0f);
				elapsedPointingTime = pointingDuration;
			}
		}
	}

	private AvatarHeadRotationHandlerRemote headRotationHandler;

	private AvatarPointingHandlerRemote pointingHandler;

	public override void Initialize(AvatarPickupOwner avatarPickupOwner, MVAvatar mvAvatar)
	{
		base.Initialize(avatarPickupOwner, mvAvatar);
		headRotationHandler = new AvatarHeadRotationHandlerRemote();
		headRotationHandler.Initialize(this, limbRotator, lookDirectionHandler);
		pointingHandler = new AvatarPointingHandlerRemote();
		pointingHandler.Initialize(this, limbRotator);
		emoteHandler = new AvatarEmoteHandler();
		emoteHandler.Initialize(this, lookDirectionHandler, pointingHandler, headRotationHandler, limbRotator);
	}

	public override void UpdateLimbRotations()
	{
		base.UpdateLimbRotations();
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
}
