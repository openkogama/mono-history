using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AvatarLimbManager
{
	protected class AvatarEmote
	{
		public Action<EmoteTypes> OnEmoteEnd;

		protected EmoteTypes emote;

		protected LimbRotator limbRotator;

		protected bool isActive;

		protected float duration;

		protected float lifeTime;

		public float LifeTime => lifeTime;

		public virtual void Initialize(LimbRotator limbRotator, float lifeTime)
		{
			this.limbRotator = limbRotator;
			this.lifeTime = lifeTime;
		}

		public virtual void StartEmote()
		{
			isActive = true;
			duration = lifeTime;
		}

		public virtual void Update()
		{
		}

		public virtual void StopEmote()
		{
			duration = 0f;
			isActive = false;
			if (OnEmoteEnd != null)
			{
				OnEmoteEnd(emote);
			}
		}
	}

	protected class EmoteData
	{
		public AvatarEmote emote;

		public short priority;
	}

	protected class AvatarEmoteHandler
	{
		protected AvatarLimbManager limbManager;

		protected Dictionary<EmoteTypes, EmoteData> emoteDatas = new Dictionary<EmoteTypes, EmoteData>();

		protected EmoteData currentRunningEmoteData;

		protected bool isActive = true;

		public Action<string> OnEmoteStart;

		public Action<int> OnEmoteUpdate;

		public void Initialize(AvatarLimbManager limbManager, AvatarLookDirectionHandler lookDirectionHandler, AvatarPointingHandler pointingHandler, AvatarHeadRotationHandler headRotationHandler, LimbRotator limbRotator, AvatarEnabledChangeHandler enableChangeHandler)
		{
			this.limbManager = limbManager;
			CreateLimbEvents(limbManager, lookDirectionHandler, pointingHandler, headRotationHandler, limbRotator);
			enableChangeHandler.OnEnabled = (Action)Delegate.Combine(enableChangeHandler.OnEnabled, new Action(OnEnable));
			enableChangeHandler.OnDisabled = (Action)Delegate.Combine(enableChangeHandler.OnDisabled, new Action(OnDisable));
		}

		protected virtual void CreateLimbEvents(AvatarLimbManager limbManager, AvatarLookDirectionHandler lookDirectionHandler, AvatarPointingHandler pointingHandler, AvatarHeadRotationHandler headRotationHandler, LimbRotator limbRotator)
		{
			AvatarShakeEmote avatarShakeEmote = new AvatarShakeEmote();
			EmoteData value = CreateEmoteData(avatarShakeEmote, limbRotator, 2f, 1);
			avatarShakeEmote.OnEmoteEnd = (Action<EmoteTypes>)Delegate.Combine(avatarShakeEmote.OnEmoteEnd, new Action<EmoteTypes>(headRotationHandler.ResetIdleTimer));
			emoteDatas.Add(EmoteTypes.Shake, value);
			AvatarNodEmote avatarNodEmote = new AvatarNodEmote();
			EmoteData value2 = CreateEmoteData(avatarNodEmote, limbRotator, 2f, 1);
			avatarNodEmote.OnEmoteEnd = (Action<EmoteTypes>)Delegate.Combine(avatarNodEmote.OnEmoteEnd, new Action<EmoteTypes>(headRotationHandler.ResetIdleTimer));
			emoteDatas.Add(EmoteTypes.Nod, value2);
			AvatarWaveEmote emote = new AvatarWaveEmote();
			EmoteData value3 = CreateEmoteData(emote, limbRotator, 1.5f, 2);
			emoteDatas.Add(EmoteTypes.Wave, value3);
		}

		private EmoteData CreateEmoteData(AvatarEmote emote, LimbRotator limbRotator, float lifeTime, short priority)
		{
			emote.Initialize(limbRotator, lifeTime);
			emote.OnEmoteEnd = (Action<EmoteTypes>)Delegate.Combine(emote.OnEmoteEnd, new Action<EmoteTypes>(OnEmoteEnd));
			EmoteData emoteData = new EmoteData();
			emoteData.emote = emote;
			emoteData.priority = priority;
			return emoteData;
		}

		public virtual void UpdateEmotes()
		{
			foreach (KeyValuePair<EmoteTypes, EmoteData> emoteData in emoteDatas)
			{
				emoteData.Value.emote.Update();
			}
		}

		protected bool CanStartEmote(EmoteData emoteData)
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
			if (emoteType != EmoteTypes.None && CanStartEmote(emoteDatas[emoteType]))
			{
				StartEmote(emoteType);
			}
		}

		protected void StartEmote(EmoteTypes emoteType)
		{
			if (emoteDatas.ContainsKey(emoteType))
			{
				emoteDatas[emoteType].emote.StartEmote();
				currentRunningEmoteData = emoteDatas[emoteType];
				if (OnEmoteStart != null)
				{
					OnEmoteStart(emoteType.ToString());
				}
				if (OnEmoteUpdate != null)
				{
					OnEmoteUpdate((int)emoteType);
				}
			}
		}

		private void OnEmoteEnd(EmoteTypes emoteType)
		{
			if (currentRunningEmoteData == emoteDatas[emoteType])
			{
				currentRunningEmoteData = null;
				if (OnEmoteUpdate != null)
				{
					OnEmoteUpdate(0);
				}
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

	protected class AvatarHeadRotationHandler
	{
		protected float idleTime;

		protected LimbRotator limbRotator;

		protected const float maxYaw = 90f;

		protected const float maxPitch = 45f;

		protected bool shouldLean = true;

		public virtual void Initialize(AvatarLimbManager limbManager, LimbRotator limbRotator, AvatarLookDirectionHandler lookDirectionHandler)
		{
			this.limbRotator = limbRotator;
			limbManager.OnAvatarRotate = (Action)Delegate.Combine(limbManager.OnAvatarRotate, new Action(ResetIdleTimer));
		}

		public virtual void UpdateRotation()
		{
			UpdateIdleTimer();
			shouldLean = true;
		}

		protected virtual void UpdateIdleTimer()
		{
			idleTime += Time.deltaTime;
			if (idleTime >= 2f)
			{
				limbRotator.StartBlendingWithAnimation(BodyData.PartIndex.Head, "Idle");
			}
		}

		protected void RotateTorso(Quaternion yawRotation, Quaternion pitchRotation)
		{
			float num = CalculateTorsoRotationModifier(yawRotation.eulerAngles.y);
			Quaternion limbPitchRotation = pitchRotation;
			limbPitchRotation.x /= num;
			limbRotator.TrySetLimbRotation(BodyData.PartIndex.Torso, Quaternion.identity, limbPitchRotation);
		}

		protected void RotateHead(Quaternion yawRotation, Quaternion pitchRotation)
		{
			float num = CalculateHeadPitchRotationModifier(yawRotation.eulerAngles.y);
			Quaternion limbPitchRotation = pitchRotation;
			limbPitchRotation.x /= num;
			limbRotator.TrySetLimbRotation(BodyData.PartIndex.Head, yawRotation, limbPitchRotation);
		}

		protected float CalculateTorsoRotationModifier(float yawAngle)
		{
			if (yawAngle > 180f)
			{
				yawAngle = 360f - yawAngle;
			}
			if (!shouldLean)
			{
				yawAngle = 90f;
			}
			return 1.5f / (1.01f - yawAngle / 90f);
		}

		protected float CalculateHeadPitchRotationModifier(float yawAngle)
		{
			if (yawAngle > 180f)
			{
				yawAngle = 360f - yawAngle;
			}
			if (!shouldLean)
			{
				yawAngle = 90f;
			}
			float num = 0.3f;
			return 1f / (1.01f - num - yawAngle / 90f + num * 2f);
		}

		protected Quaternion ClampQuaternion(Quaternion rotation)
		{
			Vector3 eulerAngles = rotation.eulerAngles;
			float num = eulerAngles.x;
			float num2 = eulerAngles.y;
			if (num > 45f && num <= 180f)
			{
				num = 45f;
			}
			else if (num < 315f && num > 180f)
			{
				num = 315f;
			}
			if (num2 > 90f && num2 <= 180f)
			{
				num2 = 90f;
			}
			else if (num2 < 270f && num2 > 180f)
			{
				num2 = 270f;
			}
			eulerAngles.x = num;
			eulerAngles.y = num2;
			rotation.eulerAngles = eulerAngles;
			return rotation;
		}

		protected void ResetIdleTimer()
		{
			idleTime = 0f;
			limbRotator.StopBlendingWithAnimation(BodyData.PartIndex.Head, "Idle");
		}

		public void ResetIdleTimer(EmoteTypes emoteType)
		{
			ResetIdleTimer();
		}
	}

	protected class AvatarLookDirectionHandler
	{
		public Action<float> OnLookDirectionYawChange;

		public Action<float> OnLookDirectionPitchChange;

		public Action OnRotationChange;

		private MVWorldObjectClient avatarWO;

		private Vector3 localLookDirection;

		private float previousYaw;

		private float previousPitch;

		private Vector3 previousLookDirection;

		public Vector3 LocalLookDirection => localLookDirection;

		public void Initialize(MVWorldObjectClient avatarWO)
		{
			this.avatarWO = avatarWO;
		}

		public void Update(Vector3 lookDirection)
		{
			Transform transform = avatarWO.Transform;
			localLookDirection = transform.InverseTransformPoint(transform.position + lookDirection);
			UpdateYaw();
			UpdatePitch();
			CheckForRotation();
		}

		private void UpdateYaw()
		{
			float y = MVGameControllerBase.MainCameraManager.transform.rotation.eulerAngles.y;
			if (y != previousYaw)
			{
				if (OnLookDirectionYawChange != null)
				{
					OnLookDirectionYawChange(y);
				}
				previousYaw = y;
			}
		}

		private void UpdatePitch()
		{
			float x = MVGameControllerBase.MainCameraManager.transform.rotation.eulerAngles.x;
			if (x != previousPitch)
			{
				if (OnLookDirectionPitchChange != null)
				{
					OnLookDirectionPitchChange(x);
				}
				previousPitch = x;
			}
		}

		private void CheckForRotation()
		{
			if ((localLookDirection - previousLookDirection).magnitude > 0.001f && OnRotationChange != null)
			{
				OnRotationChange();
			}
			previousLookDirection = localLookDirection;
		}
	}

	protected class AvatarNodEmote : AvatarEmote
	{
		private LimbController headController;

		private float originalInterpolationSpeed;

		private const float nodAngle = 15f;

		private const float amountOfRotations = 5f;

		public override void Initialize(LimbRotator limbRotator, float lifeTime)
		{
			base.Initialize(limbRotator, lifeTime);
			headController = limbRotator.GetLimbController(BodyData.PartIndex.Head);
			originalInterpolationSpeed = headController.InterpolationSpeed;
			emote = EmoteTypes.Nod;
		}

		public override void StartEmote()
		{
			base.StartEmote();
			headController.InterpolationSpeed = 5f / lifeTime * 0.5f;
			headController.IsEventControllingLimb = true;
		}

		public override void StopEmote()
		{
			base.StopEmote();
			headController.InterpolationSpeed = originalInterpolationSpeed;
			headController.IsEventControllingLimb = false;
		}

		public override void Update()
		{
			if (isActive)
			{
				duration -= Time.deltaTime;
				if (duration < 0f)
				{
					StopEmote();
				}
				HandleRotation();
			}
		}

		private void HandleRotation()
		{
			float num = duration / lifeTime;
			num = num * 5f / 10f;
			num = Mathf.Floor(num * 10f) + 1f;
			Quaternion identity = Quaternion.identity;
			if (num % 2f == 0f)
			{
				identity.eulerAngles = new Vector3(15f, 0f, 0f);
			}
			else
			{
				identity.eulerAngles = new Vector3(345f, 0f, 0f);
			}
			RotateHead(identity);
		}

		private void RotateHead(Quaternion pitchRotation)
		{
			if (headController.InterpolateTowardsPitchRotation != pitchRotation)
			{
				headController.ResetInterpolation();
				headController.SetNewRotation(headController.InterpolateTowardsYawRotation, pitchRotation);
			}
		}
	}

	protected class AvatarPointingHandler
	{
		protected Vector3 prevLookDirection;

		protected AvatarLimbManager limbManager;

		protected LimbRotator limbRotator;

		protected float pointingDuration = 0.8f;

		protected float elapsedPointingTime;

		protected Vector3 pointingDirection;

		protected bool shouldPoint = true;

		protected bool isActive = true;

		public virtual void Initialize(AvatarLimbManager limbManager, LimbRotator limbRotator, AvatarEnabledChangeHandler enableChangeHandler)
		{
			this.limbManager = limbManager;
			this.limbRotator = limbRotator;
			enableChangeHandler.OnEnabled = (Action)Delegate.Combine(enableChangeHandler.OnEnabled, new Action(OnEnable));
			enableChangeHandler.OnDisabled = (Action)Delegate.Combine(enableChangeHandler.OnDisabled, new Action(OnDisable));
		}

		public virtual void UpdatePointing(Vector3 localLookDirection)
		{
			elapsedPointingTime -= Time.deltaTime;
			prevLookDirection = localLookDirection;
		}

		protected void HandlePointing(Quaternion yawRotation, Quaternion pitchRotation)
		{
			if (yawRotation.eulerAngles.y < 180f || yawRotation.eulerAngles.y > 340f)
			{
				limbRotator.SetLimbRotation(BodyData.PartIndex.RArm, yawRotation, pitchRotation, elapsedPointingTime);
				limbRotator.StopLimbRotation(BodyData.PartIndex.LArm);
			}
			else
			{
				limbRotator.SetLimbRotation(BodyData.PartIndex.LArm, yawRotation, pitchRotation, elapsedPointingTime);
				limbRotator.StopLimbRotation(BodyData.PartIndex.RArm);
			}
		}

		protected void StopPointing()
		{
			limbRotator.StopLimbRotation(BodyData.PartIndex.RArm);
			limbRotator.StopLimbRotation(BodyData.PartIndex.LArm);
		}

		private void OnDisable()
		{
			isActive = false;
		}

		private void OnEnable()
		{
			isActive = true;
		}
	}

	protected class AvatarShakeEmote : AvatarEmote
	{
		private LimbController headController;

		private float originalInterpolationSpeed;

		private const float shakeAngle = 25f;

		private const float amountOfRotations = 5f;

		public override void Initialize(LimbRotator limbRotator, float lifeTime)
		{
			base.Initialize(limbRotator, lifeTime);
			headController = limbRotator.GetLimbController(BodyData.PartIndex.Head);
			originalInterpolationSpeed = headController.InterpolationSpeed;
			emote = EmoteTypes.Shake;
		}

		public override void StartEmote()
		{
			base.StartEmote();
			headController.InterpolationSpeed = 5f / lifeTime * 0.5f;
			headController.IsEventControllingLimb = true;
		}

		public override void StopEmote()
		{
			base.StopEmote();
			headController.InterpolationSpeed = originalInterpolationSpeed;
			headController.IsEventControllingLimb = false;
		}

		public override void Update()
		{
			if (isActive)
			{
				duration -= Time.deltaTime;
				if (duration < 0f)
				{
					StopEmote();
				}
				else
				{
					HandleRotation();
				}
			}
		}

		private void HandleRotation()
		{
			float num = duration / lifeTime;
			num = num * 5f / 10f;
			num = Mathf.Floor(num * 10f) + 1f;
			Quaternion identity = Quaternion.identity;
			if (num % 2f == 0f)
			{
				identity.eulerAngles = new Vector3(0f, 25f, 0f);
			}
			else
			{
				identity.eulerAngles = new Vector3(0f, 335f, 0f);
			}
			RotateHead(identity);
		}

		private void RotateHead(Quaternion yawRotation)
		{
			if (headController.InterpolateTowardsYawRotation != yawRotation)
			{
				headController.ResetInterpolation();
				headController.SetNewRotation(yawRotation, headController.InterpolateTowardsPitchRotation);
			}
		}
	}

	protected class AvatarWaveEmote : AvatarEmote
	{
		private LimbController RArmController;

		private LimbController LArmController;

		private float originalInterpolationSpeed;

		private const float waveAngle = 25f;

		private const float amountOfRotations = 8f;

		public override void Initialize(LimbRotator limbRotator, float lifeTime)
		{
			base.Initialize(limbRotator, lifeTime);
			RArmController = limbRotator.GetLimbController(BodyData.PartIndex.RArm);
			LArmController = limbRotator.GetLimbController(BodyData.PartIndex.LArm);
			originalInterpolationSpeed = RArmController.InterpolationSpeed;
			emote = EmoteTypes.Wave;
		}

		public override void StartEmote()
		{
			base.StartEmote();
			RArmController.InterpolationSpeed = 8f / lifeTime * 0.5f;
			RArmController.IsEventControllingLimb = true;
			LArmController.InterpolationSpeed = 8f / lifeTime * 0.5f;
			LArmController.IsEventControllingLimb = true;
		}

		public override void StopEmote()
		{
			base.StopEmote();
			RArmController.InterpolationSpeed = originalInterpolationSpeed;
			RArmController.IsEventControllingLimb = false;
			RArmController.StopRotating();
			LArmController.InterpolationSpeed = originalInterpolationSpeed;
			LArmController.IsEventControllingLimb = false;
			LArmController.StopRotating();
		}

		public override void Update()
		{
			if (isActive)
			{
				duration -= Time.deltaTime;
				if (duration <= 0f)
				{
					StopEmote();
					return;
				}
				HandleArmRotation(RArmController, 65f);
				HandleArmRotation(LArmController, 295f);
			}
		}

		private void HandleArmRotation(LimbController armController, float yawAngle)
		{
			Quaternion identity = Quaternion.identity;
			identity.eulerAngles = new Vector3(0f, yawAngle, 0f);
			float num = duration / lifeTime;
			num *= 0.8f;
			num = Mathf.Floor(num * 10f) + 1f;
			if (num % 2f == 0f)
			{
				MoveArmDownwards(armController, identity);
			}
			else
			{
				MoveArmUpwards(armController, identity);
			}
		}

		private void MoveArmUpwards(LimbController armController, Quaternion yawRotation)
		{
			float num = -25f;
			Quaternion identity = Quaternion.identity;
			identity.eulerAngles = new Vector3(335f + num, 0f, 0f);
			if (armController.InterpolateTowardsPitchRotation != identity)
			{
				armController.ResetInterpolation();
				armController.SetNewRotation(yawRotation, identity);
			}
		}

		private void MoveArmDownwards(LimbController armController, Quaternion yawRotation)
		{
			float num = -25f;
			Quaternion identity = Quaternion.identity;
			identity.eulerAngles = new Vector3(25f + num, 0f, 0f);
			if (armController.InterpolateTowardsPitchRotation != identity)
			{
				armController.ResetInterpolation();
				armController.SetNewRotation(yawRotation, identity);
			}
		}
	}

	protected class LimbRotator
	{
		private Dictionary<BodyData.PartIndex, LimbController> limbControllers;

		private bool isActive = true;

		public LimbController GetLimbController(BodyData.PartIndex partIndex)
		{
			if (HasLimbController(partIndex))
			{
				return limbControllers[partIndex];
			}
			return null;
		}

		private bool HasLimbController(BodyData.PartIndex partIndex)
		{
			if (limbControllers.ContainsKey(partIndex))
			{
				return true;
			}
			Debug.LogError(string.Concat("LimbRotator does not have a limb controller for  ", partIndex, ". Returning null instead"));
			return false;
		}

		public void Initialize(MVWorldObjectClient avatarWO, MVBody body, AvatarLimbManager limbManager)
		{
			limbControllers = new Dictionary<BodyData.PartIndex, LimbController>();
			BoneAnimation animation = body.Animation;
			animation.OnAnimationChange = (Action<string>)Delegate.Combine(animation.OnAnimationChange, new Action<string>(OnAnimationChange));
			CreateLimbController(BodyData.PartIndex.Torso, avatarWO, body, limbManager);
			CreateLimbController(BodyData.PartIndex.Head, avatarWO, body, limbManager);
			CreateLimbController(BodyData.PartIndex.RArm, avatarWO, body, limbManager);
			CreateLimbController(BodyData.PartIndex.LArm, avatarWO, body, limbManager);
		}

		private void OnAnimationChange(string newAnimation)
		{
			foreach (KeyValuePair<BodyData.PartIndex, LimbController> limbController in limbControllers)
			{
				limbController.Value.CurrentAnimation = newAnimation;
			}
		}

		private void CreateLimbController(BodyData.PartIndex partIndex, MVWorldObjectClient avatarWO, MVBody body, AvatarLimbManager limbManager)
		{
			switch (partIndex)
			{
			case BodyData.PartIndex.Head:
			{
				LimbController limbController4 = new LimbController();
				List<string> blendAnimations4 = new List<string>();
				List<string> list4 = new List<string>();
				list4.Add("Dead");
				limbController4.Initialize(limbManager, avatarWO, body, partIndex, Quaternion.identity, Quaternion.identity, blendAnimations4, list4, 89f, 45f);
				limbControllers.Add(partIndex, limbController4);
				break;
			}
			case BodyData.PartIndex.Torso:
			{
				LimbController limbController3 = new LimbController();
				List<string> blendAnimations3 = new List<string>();
				List<string> list3 = new List<string>();
				list3.Add("Dead");
				limbController3.Initialize(limbManager, avatarWO, body, partIndex, Quaternion.identity, Quaternion.identity, blendAnimations3, list3, 90f, 20f);
				limbControllers.Add(partIndex, limbController3);
				break;
			}
			case BodyData.PartIndex.RArm:
			{
				LimbController limbController2 = new LimbController();
				List<string> blendAnimations2 = new List<string>();
				List<string> list2 = new List<string>();
				list2.Add("Dead");
				list2.Add("Jump");
				Quaternion identity3 = Quaternion.identity;
				identity3.eulerAngles = new Vector3(0f, 270f, 0f);
				Quaternion identity4 = Quaternion.identity;
				identity4.eulerAngles = new Vector3(355.2f, 359f, 282f);
				limbController2.Initialize(limbManager, avatarWO, body, partIndex, identity3, identity4, blendAnimations2, list2, 90f, 45f);
				limbControllers.Add(partIndex, limbController2);
				break;
			}
			case BodyData.PartIndex.LArm:
			{
				LimbController limbController = new LimbController();
				List<string> blendAnimations = new List<string>();
				List<string> list = new List<string>();
				list.Add("Dead");
				list.Add("Jump");
				Quaternion identity = Quaternion.identity;
				identity.eulerAngles = new Vector3(0f, 90f, 0f);
				Quaternion identity2 = Quaternion.identity;
				identity2.eulerAngles = new Vector3(6.2f, 358.5f, 76.2f);
				limbController.Initialize(limbManager, avatarWO, body, partIndex, identity, identity2, blendAnimations, list, 90f, 45f);
				limbControllers.Add(partIndex, limbController);
				break;
			}
			}
		}

		public void UpdateLimbs()
		{
			if (!isActive || MVGameControllerBase.MainCameraManager.BlueModeEnabled)
			{
				return;
			}
			foreach (KeyValuePair<BodyData.PartIndex, LimbController> limbController in limbControllers)
			{
				limbController.Value.UpdateRotation();
			}
		}

		public void TrySetLimbRotation(BodyData.PartIndex partIndex, Quaternion limbYawRotation, Quaternion limbPitchRotation)
		{
			if (HasLimbController(partIndex))
			{
				limbControllers[partIndex].TrySetNewRotation(limbYawRotation, limbPitchRotation);
			}
		}

		public void SetLimbRotation(BodyData.PartIndex partIndex, Quaternion limbYawRotation, Quaternion limbPitchRotation, float duration)
		{
			if (HasLimbController(partIndex))
			{
				limbControllers[partIndex].TrySetNewRotation(limbYawRotation, limbPitchRotation, duration);
			}
		}

		public void StopLimbRotation(BodyData.PartIndex partIndex)
		{
			limbControllers[partIndex].StopRotating();
		}

		public void StartBlendingWithAnimation(BodyData.PartIndex partIndex, string animation)
		{
			if (HasLimbController(partIndex))
			{
				limbControllers[partIndex].StartBlendingWithAnimation(animation);
			}
		}

		public void StopBlendingWithAnimation(BodyData.PartIndex partIndex, string animation)
		{
			if (HasLimbController(partIndex))
			{
				limbControllers[partIndex].StopBlendingWithAnimation(animation);
			}
		}

		public void SetIsActive(bool shouldBeActive)
		{
			isActive = shouldBeActive;
		}
	}

	public Action OnAvatarRotate;

	public Action<string> OnEmoteStart;

	protected MVWorldObjectClient avatarWO;

	protected AvatarEmoteHandler emoteHandler;

	protected LimbRotator limbRotator;

	protected AvatarLookDirectionHandler lookDirectionHandler;

	private Quaternion previousTransformRotation;

	private const float maxYawAllowed = 30f;

	public virtual void Initialize(MVWorldObjectClient avatarWO, MVBody body, AvatarEnabledChangeHandler enabledChangeHandler, LimbRotationRuntimeData limbRotationRuntimeData)
	{
		this.avatarWO = avatarWO;
		lookDirectionHandler = new AvatarLookDirectionHandler();
		lookDirectionHandler.Initialize(avatarWO);
		limbRotator = new LimbRotator();
		limbRotator.Initialize(avatarWO, body, this);
	}

	public virtual void UpdateLimbRotations(Vector3 lookDirection)
	{
		lookDirectionHandler.Update(lookDirection);
	}

	protected void CheckAvatarRotation()
	{
		float f = QuaternionAngleToNormalAngle(avatarWO.Transform.rotation.eulerAngles.y) - QuaternionAngleToNormalAngle(previousTransformRotation.eulerAngles.y);
		if (Mathf.Abs(f) > 30f && OnAvatarRotate != null)
		{
			OnAvatarRotate();
		}
		previousTransformRotation = avatarWO.Transform.rotation;
	}

	protected void OnStartEmote(string newAnimation)
	{
		if (OnEmoteStart != null)
		{
			OnEmoteStart(newAnimation);
		}
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

	public void SetLimbRotatorActivity(bool shouldBeActive)
	{
		limbRotator.SetIsActive(shouldBeActive);
	}

	public abstract void StartEmote(EmoteTypes emoteType);
}
