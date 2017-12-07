using System;
using UnityEngine;

public class AvatarLimbManager
{
	private const float maxYawAllowed = 30f;

	public Action<float> DelayHeadRotationNetworkMessage;

	public Action<float> DelayPointingNetworkMessage;

	public Action OnAvatarRotate;

	private MVAvatar mvAvatar;

	private AvatarPickupOwner avatarPickupOwner;

	private AvatarHeadRotationHandler headRotationHandler;

	private AvatarPointingHandler pointingHandler;

	private AvatarEmoteHandler emoteHandler;

	private LimbRotator limbRotator;

	private AvatarLookDirectionHandler lookDirectionHandler;

	private Quaternion previousTransformRotation;

	public LimbRotator LimbRotator => limbRotator;

	public AvatarLookDirectionHandler LookDirectionHandler => lookDirectionHandler;

	public void Initialize(AvatarPickupOwner avatarPickupOwner, MVAvatar mvAvatar)
	{
		bool isLocal = false;
		if (mvAvatar is MVAvatarLocal)
		{
			isLocal = true;
		}
		this.avatarPickupOwner = avatarPickupOwner;
		this.mvAvatar = mvAvatar;
		lookDirectionHandler = new AvatarLookDirectionHandler();
		lookDirectionHandler.Initialize(mvAvatar);
		limbRotator = new LimbRotator();
		limbRotator.Initialize(mvAvatar, this);
		headRotationHandler = mvAvatar.Transform.gameObject.AddComponent<AvatarHeadRotationHandler>();
		headRotationHandler.Initialize(isLocal, this);
		pointingHandler = mvAvatar.Transform.gameObject.AddComponent<AvatarPointingHandler>();
		pointingHandler.Initialize(isLocal, this);
		emoteHandler = mvAvatar.Transform.gameObject.AddComponent<AvatarEmoteHandler>();
		emoteHandler.Initialize(this, lookDirectionHandler, pointingHandler, headRotationHandler, isLocal);
		DelayHeadRotationNetworkMessage = (Action<float>)Delegate.Combine(DelayHeadRotationNetworkMessage, new Action<float>(headRotationHandler.ResetNetworkMessageCooldown));
		DelayPointingNetworkMessage = (Action<float>)Delegate.Combine(DelayPointingNetworkMessage, new Action<float>(pointingHandler.ResetNetworkMessageDelay));
	}

	public void UpdateLimbRotations()
	{
		Vector3 lookDirection = avatarPickupOwner.LookDirection;
		lookDirectionHandler.Update(lookDirection);
		Vector3 localLookDirection = lookDirectionHandler.LocalLookDirection;
		emoteHandler.UpdateEmotes();
		headRotationHandler.UpdateRotation(localLookDirection);
		pointingHandler.UpdatePointing(localLookDirection);
		CheckAvatarRotation();
		limbRotator.UpdateLimbs();
	}

	private void CheckAvatarRotation()
	{
		float f = QuaternionAngleToNormalAngle(mvAvatar.Transform.rotation.eulerAngles.y) - QuaternionAngleToNormalAngle(previousTransformRotation.eulerAngles.y);
		if (Mathf.Abs(f) > 30f && OnAvatarRotate != null)
		{
			OnAvatarRotate();
		}
		previousTransformRotation = mvAvatar.Transform.rotation;
	}

	private float QuaternionAngleToNormalAngle(float angle)
	{
		if (angle > 180f)
		{
			angle -= 180f;
			angle = 180f - angle;
		}
		return angle;
	}

	public void StartPointing()
	{
		pointingHandler.StartPointing();
	}

	public void UpdateHeadRotationRemotely(float yaw, float pitch)
	{
		headRotationHandler.SetRotationRemotely(yaw, pitch);
	}

	public void UpdatePointingRemotely(float yaw, float pitch)
	{
		pointingHandler.SetRotationRemotely(yaw, pitch);
	}

	public void SetLimbRotatorActivity(bool shouldBeActive)
	{
		limbRotator.SetIsActive(shouldBeActive);
	}

	public void HandleEmote(EmoteTypes emoteType)
	{
		emoteHandler.TryStartEmote(emoteType);
	}

	public void StartEmote(EmoteTypes emoteType)
	{
		emoteHandler.StartEmoteAndNetworkIt(emoteType);
	}
}
