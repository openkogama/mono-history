using System;
using System.Collections.Generic;
using UnityEngine;

public class AvatarLimbManagerLocal : AvatarLimbManager
{
	private class AvatarEmoteHandlerLocal : AvatarEmoteHandler
	{
		private Dictionary<EmoteTypes, AvatarEmoteRecogniser> emoteRecognisers = new Dictionary<EmoteTypes, AvatarEmoteRecogniser>();

		protected override void CreateLimbEvents(AvatarLimbManager limbManager, AvatarLookDirectionHandler lookDirectionHandler, AvatarPointingHandler pointingHandler, AvatarHeadRotationHandler headRotationHandler, LimbRotator limbRotator)
		{
			AvatarEmoteRecogniser avatarEmoteRecogniser = new AvatarEmoteRecogniser();
			avatarEmoteRecogniser.Initlialize(limbManager, 10f, 2f, 4, shouldRecognisePositiveAngleFirst: true, isActive: false);
			lookDirectionHandler.OnLookDirectionYawChange = (Action<float>)Delegate.Combine(lookDirectionHandler.OnLookDirectionYawChange, new Action<float>(avatarEmoteRecogniser.HandleNewAngle));
			avatarEmoteRecogniser.OnStartEvent = (Action)Delegate.Combine(avatarEmoteRecogniser.OnStartEvent, new Action(OnWaveEmoteStart));
			AvatarPointingHandlerLocal avatarPointingHandlerLocal = (AvatarPointingHandlerLocal)pointingHandler;
			avatarPointingHandlerLocal.OnIsPointingChange = (Action<bool>)Delegate.Combine(avatarPointingHandlerLocal.OnIsPointingChange, new Action<bool>(avatarEmoteRecogniser.SetIsActive));
			emoteRecognisers.Add(EmoteTypes.Wave, avatarEmoteRecogniser);
			AvatarEmoteRecogniser avatarEmoteRecogniser2 = new AvatarEmoteRecogniser();
			avatarEmoteRecogniser2.Initlialize(limbManager, 10f, 2f, 4, shouldRecognisePositiveAngleFirst: true, isActive: true);
			lookDirectionHandler.OnLookDirectionYawChange = (Action<float>)Delegate.Combine(lookDirectionHandler.OnLookDirectionYawChange, new Action<float>(avatarEmoteRecogniser2.HandleNewAngle));
			avatarEmoteRecogniser2.OnStartEvent = (Action)Delegate.Combine(avatarEmoteRecogniser2.OnStartEvent, new Action(OnShakeEmoteStart));
			emoteRecognisers.Add(EmoteTypes.Shake, avatarEmoteRecogniser2);
			AvatarEmoteRecogniser avatarEmoteRecogniser3 = new AvatarEmoteRecogniser();
			avatarEmoteRecogniser3.Initlialize(limbManager, 10f, 2f, 4, shouldRecognisePositiveAngleFirst: false, isActive: true);
			lookDirectionHandler.OnLookDirectionPitchChange = (Action<float>)Delegate.Combine(lookDirectionHandler.OnLookDirectionPitchChange, new Action<float>(avatarEmoteRecogniser3.HandleNewAngle));
			avatarEmoteRecogniser3.OnStartEvent = (Action)Delegate.Combine(avatarEmoteRecogniser3.OnStartEvent, new Action(OnNodEmoteStart));
			emoteRecognisers.Add(EmoteTypes.Nod, avatarEmoteRecogniser3);
			base.CreateLimbEvents(limbManager, lookDirectionHandler, pointingHandler, headRotationHandler, limbRotator);
		}

		public override void UpdateEmotes()
		{
			foreach (KeyValuePair<EmoteTypes, AvatarEmoteRecogniser> emoteRecogniser in emoteRecognisers)
			{
				emoteRecogniser.Value.Update();
			}
			base.UpdateEmotes();
		}

		private void OnShakeEmoteStart()
		{
			if (CanStartEmote(emoteDatas[EmoteTypes.Shake]))
			{
				StartEmote(EmoteTypes.Shake);
				MVGameControllerBase.OperationRequests.StartHeadShake();
				((AvatarLimbManagerLocal)limbManager).DelayHeadRotationNetworkMessage(emoteDatas[EmoteTypes.Shake].emote.LifeTime);
			}
		}

		private void OnNodEmoteStart()
		{
			if (CanStartEmote(emoteDatas[EmoteTypes.Nod]))
			{
				StartEmote(EmoteTypes.Nod);
				MVGameControllerBase.OperationRequests.StartHeadNod();
				((AvatarLimbManagerLocal)limbManager).DelayHeadRotationNetworkMessage(emoteDatas[EmoteTypes.Nod].emote.LifeTime);
			}
		}

		private void OnWaveEmoteStart()
		{
			if (CanStartEmote(emoteDatas[EmoteTypes.Wave]))
			{
				StartEmote(EmoteTypes.Wave);
				MVGameControllerBase.OperationRequests.StartWave();
				((AvatarLimbManagerLocal)limbManager).DelayHeadRotationNetworkMessage(emoteDatas[EmoteTypes.Wave].emote.LifeTime);
				((AvatarLimbManagerLocal)limbManager).DelayPointingNetworkMessage(emoteDatas[EmoteTypes.Wave].emote.LifeTime);
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
			case EmoteTypes.Wave:
				OnWaveEmoteStart();
				break;
			default:
				Debug.LogError(string.Concat("Could not start and network ", emoteType, ". Please add it to the StartEmoteAndNetworkIt function in AvatarEmoteHandler."));
				break;
			}
		}
	}

	private class AvatarEmoteRecogniser
	{
		private const float maxRotationMovement = 45f;

		public Action OnStartEvent;

		private bool isActive;

		private float angleSensitivity = 15f;

		private float previousAngle;

		private short eventRecognitioner;

		private short recognitionsBeforeEvent;

		private float resetInterval = 2f;

		private float resetCooldown;

		private short startModulusOffset;

		public void Initlialize(AvatarLimbManager limbManager, float angleSensitivity, float resetInterval, short recognitionsBeforeEvent, bool shouldRecognisePositiveAngleFirst, bool isActive)
		{
			limbManager.OnAvatarRotate = (Action)Delegate.Combine(limbManager.OnAvatarRotate, new Action(ResetRecognition));
			this.angleSensitivity = angleSensitivity;
			this.resetInterval = resetInterval;
			resetCooldown = resetInterval;
			this.recognitionsBeforeEvent = recognitionsBeforeEvent;
			if (shouldRecognisePositiveAngleFirst)
			{
				startModulusOffset = 0;
			}
			else
			{
				startModulusOffset = 1;
			}
			this.isActive = isActive;
		}

		public void HandleNewAngle(float angle)
		{
			if (!isActive)
			{
				ResetRecognition();
			}
			if (eventRecognitioner % 2 == startModulusOffset)
			{
				HandleRecognition(angle, previousAngle, checkPositiveRotation: true);
			}
			else
			{
				HandleRecognition(angle, previousAngle, checkPositiveRotation: false);
			}
			if (IsEventRecognitionDone() && OnStartEvent != null)
			{
				OnStartEvent();
			}
		}

		private void HandleRecognition(float newAngle, float previousAngle, bool checkPositiveRotation)
		{
			if (IsRotationPositive(newAngle, previousAngle) == checkPositiveRotation)
			{
				if (Mathf.Abs(newAngle - previousAngle) > angleSensitivity)
				{
					eventRecognitioner++;
				}
			}
			else
			{
				this.previousAngle = newAngle;
			}
		}

		private bool IsRotationPositive(float newAngle, float previousAngle)
		{
			if (previousAngle > 315f && newAngle < 45f)
			{
				return true;
			}
			if (previousAngle < 45f && newAngle > 315f)
			{
				return false;
			}
			if (previousAngle < newAngle)
			{
				return true;
			}
			return false;
		}

		private bool IsEventRecognitionDone()
		{
			if (eventRecognitioner >= recognitionsBeforeEvent)
			{
				eventRecognitioner = 0;
				resetCooldown = resetInterval;
				return true;
			}
			return false;
		}

		public void Update()
		{
			if (eventRecognitioner == 0)
			{
				ResetRecognition();
			}
			resetCooldown -= Time.deltaTime;
			if (resetCooldown <= 0f)
			{
				ResetRecognition();
			}
		}

		private void ResetRecognition()
		{
			eventRecognitioner = 0;
			resetCooldown = resetInterval;
		}

		public void SetIsActive(bool shouldBeActive)
		{
			if (shouldBeActive)
			{
				Activate();
			}
			else
			{
				Deactivate();
			}
		}

		public void Deactivate()
		{
			isActive = false;
		}

		public void Activate()
		{
			isActive = true;
		}
	}

	public struct HeadRotationCalculationResult
	{
		public Quaternion YawRotation;

		public Quaternion PitchRotation;

		public bool ShouldLean;
	}

	public class AvatarHeadRotationCalculator
	{
		private const float maxYaw = 90f;

		private const float maxPitch = 45f;

		private const float cameraPitchOffset = 25f;

		private const float yawDeadzone = 135f;

		private bool shouldLean;

		public HeadRotationCalculationResult CalculateHeadRotation(Vector3 localLookDirection)
		{
			shouldLean = true;
			HeadRotationCalculationResult result = default;
			Quaternion clampedYawRotation = GetClampedYawRotation(localLookDirection);
			Quaternion clampedPitchRotation = GetClampedPitchRotation(localLookDirection);
			result.YawRotation = clampedYawRotation;
			result.PitchRotation = clampedPitchRotation;
			result.ShouldLean = shouldLean;
			return result;
		}

		private Quaternion GetClampedYawRotation(Vector3 localDirection)
		{
			float yaw = MathFunctions.SignedYawFromLocalDirection(localDirection);
			yaw = HandleYawDeadZone(yaw);
			float angle = Mathf.Clamp(yaw, -90f, 90f);
			return MathFunctions.QuaternionFromAngleAndAxis(angle, Vector3.up);
		}

		private Quaternion GetClampedPitchRotation(Vector3 localDirection)
		{
			float pitch = MathFunctions.PitchFromLocalDirection(localDirection);
			pitch = HandleCameraPitchOffset(pitch);
			float angle = Mathf.Clamp(pitch, -45f, 45f);
			return MathFunctions.QuaternionFromAngleAndAxis(angle, Vector3.right);
		}

		private float HandleCameraPitchOffset(float pitch)
		{
			if (pitch <= 0f)
			{
				return pitch;
			}
			if (pitch > 0f && pitch < 25f)
			{
				return 0f;
			}
			pitch -= 25f;
			return pitch;
		}

		private float HandleYawDeadZone(float yaw)
		{
			if (yaw < -135f)
			{
				yaw = 0f;
				shouldLean = false;
			}
			if (yaw > 135f)
			{
				yaw = 0f;
				shouldLean = false;
			}
			return yaw;
		}
	}

	private class AvatarHeadRotationHandlerLocal : AvatarHeadRotationHandler
	{
		private const float networkMessageInterval = 1f;

		private float networkMessageCooldown;

		private bool shouldSendNetworkMessage;

		private Quaternion yawRotation = Quaternion.identity;

		private Quaternion pitchRotation = Quaternion.identity;

		public override void UpdateRotation()
		{
			UpdateNetworkMessage(yawRotation * pitchRotation);
			RotateTorso(yawRotation, pitchRotation);
			RotateHead(yawRotation, pitchRotation);
			base.UpdateRotation();
		}

		public override void Initialize(AvatarLimbManager limbManager, LimbRotator limbRotator, AvatarLookDirectionHandler lookDirectionHandler)
		{
			lookDirectionHandler.OnRotationChange = (Action)Delegate.Combine(lookDirectionHandler.OnRotationChange, new Action(HandleOnRotationChange));
			base.Initialize(limbManager, limbRotator, lookDirectionHandler);
		}

		private void UpdateNetworkMessage(Quaternion rotation)
		{
			networkMessageCooldown -= Time.deltaTime;
			if (networkMessageCooldown <= 0f && shouldSendNetworkMessage)
			{
				MVGameControllerBase.OperationRequests.UpdateHeadRotation(rotation);
				ResetNetworkMessageCooldown(1f);
			}
		}

		public void ResetNetworkMessageCooldown(float networkMessageDelay)
		{
			if (networkMessageDelay > networkMessageCooldown)
			{
				networkMessageCooldown = networkMessageDelay;
			}
			shouldSendNetworkMessage = false;
		}

		public void HandleResult(HeadRotationCalculationResult result)
		{
			yawRotation = result.YawRotation;
			pitchRotation = result.PitchRotation;
			shouldLean = result.ShouldLean;
		}

		protected void HandleOnRotationChange()
		{
			ResetIdleTimer();
			shouldSendNetworkMessage = true;
		}
	}

	private class AvatarPointingHandlerLocal : AvatarPointingHandler
	{
		private const float lArmYawRotationOffset = -30f;

		private const float rArmYawRotationOffset = 20f;

		private float networkMessageCooldown;

		private Quaternion yawRotation = Quaternion.identity;

		private Quaternion pitchRotation = Quaternion.identity;

		public Action<bool> OnIsPointingChange;

		public Vector3 PointingDirection => pointingDirection;

		public void StartPointing()
		{
			if (isActive)
			{
				elapsedPointingTime = pointingDuration;
				pointingDirection = prevLookDirection;
				if (OnIsPointingChange != null)
				{
					OnIsPointingChange(obj: true);
				}
			}
		}

		public override void UpdatePointing(Vector3 localLookDirection)
		{
			if (elapsedPointingTime > 0f)
			{
				UpdateNetworkMessage(yawRotation * pitchRotation);
				if (shouldPoint)
				{
					HandlePointing(yawRotation, pitchRotation);
				}
				else
				{
					StopPointing();
				}
			}
			else if (OnIsPointingChange != null)
			{
				OnIsPointingChange(obj: false);
			}
			base.UpdatePointing(localLookDirection);
		}

		private void UpdateNetworkMessage(Quaternion rotation)
		{
			networkMessageCooldown -= Time.deltaTime;
			if (networkMessageCooldown <= 0f)
			{
				if (shouldPoint)
				{
					MVGameControllerBase.OperationRequests.UpdatePointingAndHeadRotation(rotation);
				}
				else
				{
					MVGameControllerBase.OperationRequests.UpdateHeadRotation(Quaternion.identity);
				}
				ResetNetworkMessageDelay(pointingDuration);
				if (((AvatarLimbManagerLocal)limbManager).DelayHeadRotationNetworkMessage != null)
				{
					((AvatarLimbManagerLocal)limbManager).DelayHeadRotationNetworkMessage(1.1f);
				}
			}
		}

		public void ResetNetworkMessageDelay(float networkMessageDelay)
		{
			networkMessageCooldown = networkMessageDelay;
		}

		public void HandleResult(PointingRotationCalculationResult result)
		{
			yawRotation = ApplyYawOffset(result.YawRotation);
			pitchRotation = result.PitchRotation;
			shouldPoint = result.ShouldPoint;
		}

		private Quaternion ApplyYawOffset(Quaternion newYawRotation)
		{
			Quaternion identity = Quaternion.identity;
			if (yawRotation.eulerAngles.y < 180f || yawRotation.eulerAngles.y > 340f)
			{
				identity.eulerAngles = new Vector3(0f, 20f, 0f);
			}
			else
			{
				identity.eulerAngles = new Vector3(0f, -30f, 0f);
			}
			return newYawRotation * identity;
		}
	}

	public struct PointingRotationCalculationResult
	{
		public Quaternion YawRotation;

		public Quaternion PitchRotation;

		public bool ShouldPoint;
	}

	public class AvatarPointingRotationCalculator
	{
		private const float maxYaw = 90f;

		private const float maxPitch = 90f;

		private bool shouldPoint;

		public PointingRotationCalculationResult CalculateRotation(Vector3 pointingDirection)
		{
			shouldPoint = true;
			PointingRotationCalculationResult result = default;
			Quaternion clampedYawRotation = GetClampedYawRotation(pointingDirection);
			Quaternion clampedPitchRotation = GetClampedPitchRotation(pointingDirection);
			result.YawRotation = clampedYawRotation;
			result.PitchRotation = clampedPitchRotation;
			result.ShouldPoint = shouldPoint;
			return result;
		}

		private Quaternion GetClampedYawRotation(Vector3 localDirection)
		{
			float num = MathFunctions.SignedYawFromLocalDirection(localDirection);
			HandleYawDeadZone(num);
			float angle = Mathf.Clamp(num, -90f, 90f);
			return MathFunctions.QuaternionFromAngleAndAxis(angle, Vector3.up);
		}

		private Quaternion GetClampedPitchRotation(Vector3 localDirection)
		{
			float value = MathFunctions.PitchFromLocalDirection(localDirection);
			float angle = Mathf.Clamp(value, -90f, 90f);
			return Quaternion.AngleAxis(angle, Vector3.right);
		}

		protected void HandleYawDeadZone(float yaw)
		{
			if (yaw < -135f)
			{
				shouldPoint = false;
			}
			if (yaw > 135f)
			{
				shouldPoint = false;
			}
		}
	}

	private AvatarHeadRotationHandlerLocal headRotationHandler;

	private AvatarHeadRotationCalculator headRotationCalculator;

	private AvatarPointingHandlerLocal pointingHandler;

	private AvatarPointingRotationCalculator pointingRotationCalculator;

	public Action<float> DelayHeadRotationNetworkMessage;

	public Action<float> DelayPointingNetworkMessage;

	public override void Initialize(AvatarPickupOwner avatarPickupOwner, MVAvatar mvAvatar)
	{
		base.Initialize(avatarPickupOwner, mvAvatar);
		headRotationCalculator = new AvatarHeadRotationCalculator();
		headRotationHandler = new AvatarHeadRotationHandlerLocal();
		headRotationHandler.Initialize(this, limbRotator, lookDirectionHandler);
		pointingRotationCalculator = new AvatarPointingRotationCalculator();
		pointingHandler = new AvatarPointingHandlerLocal();
		pointingHandler.Initialize(this, limbRotator, mvAvatar.Avatar.EnabledChangeHandler);
		emoteHandler = new AvatarEmoteHandlerLocal();
		emoteHandler.Initialize(this, lookDirectionHandler, pointingHandler, headRotationHandler, limbRotator, mvAvatar.Avatar.EnabledChangeHandler);
		DelayHeadRotationNetworkMessage = (Action<float>)Delegate.Combine(DelayHeadRotationNetworkMessage, new Action<float>(headRotationHandler.ResetNetworkMessageCooldown));
		DelayPointingNetworkMessage = (Action<float>)Delegate.Combine(DelayPointingNetworkMessage, new Action<float>(pointingHandler.ResetNetworkMessageDelay));
		AvatarEmoteHandler avatarEmoteHandler = emoteHandler;
		avatarEmoteHandler.OnEmoteStart = (Action<string>)Delegate.Combine(avatarEmoteHandler.OnEmoteStart, new Action<string>(OnStartEmote));
	}

	public override void UpdateLimbRotations()
	{
		base.UpdateLimbRotations();
		Vector3 localLookDirection = lookDirectionHandler.LocalLookDirection;
		emoteHandler.UpdateEmotes();
		headRotationHandler.HandleResult(headRotationCalculator.CalculateHeadRotation(localLookDirection));
		headRotationHandler.UpdateRotation();
		pointingHandler.HandleResult(pointingRotationCalculator.CalculateRotation(pointingHandler.PointingDirection));
		pointingHandler.UpdatePointing(localLookDirection);
		CheckAvatarRotation();
		limbRotator.UpdateLimbs();
	}

	public override void StartEmote(EmoteTypes emoteType)
	{
		((AvatarEmoteHandlerLocal)emoteHandler).StartEmoteAndNetworkIt(emoteType);
	}

	public void StartPointing()
	{
		pointingHandler.StartPointing();
	}
}
