using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public class MVAvatarRemote : MVAvatar, IBulletImpactVisualizer, ISpawnRoleRemote
{
	private DynamicCullingHandler cullingHandler = new DynamicCullingHandler(3.5f);

	private CapsuleCollider triggerCollider;

	private AvatarRemoteMovementCalculator avatarRemoteMovementCalculator;

	private const float initialCullingRadius = 3.5f;

	private float impulseMagnitudeFactor = 0.6f;

	private float velocityMinMagnitude = 1500f;

	private float velocityMaxMagnitude = 5000f;

	private float minVelocity = 700f;

	private const float hitTimeOut = 2f;

	private float prevHitTime = Time.time - 2f;

	public bool IsInVehicle { get; private set; }

	public override Vector3 VelocityRelative => (!IsInVehicle) ? avatarRemoteMovementCalculator.VelocityEstimate : new Vector3(0f, 0f, 0f);

	public override Vector3 VelocityAbsolute
	{
		get
		{
			if (avatarRemoteMovementCalculator == null)
			{
				return Vector3.zero;
			}
			return avatarRemoteMovementCalculator.VelocityEstimate;
		}
	}

	public MVAvatarRemote(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVRemoteAvatarPrefab, worldObjects)
	{
		SetNetworkObject(local: false);
		IsInVehicle = false;
		MVRuntimeDataVariable animation = Animation;
		animation.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(animation.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnAnimationChange));
	}

	public override void Initialize()
	{
		base.Initialize();
		((AvatarUIHandlerRemote)avatar.AvatarUIHandler).UpdateNameTag();
		BuildTarget buildTarget = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(OwnerActorNr).BuildTarget;
		if (buildTarget == BuildTarget.Android || buildTarget == BuildTarget.IOS)
		{
			((AvatarUIHandlerRemote)avatar.AvatarUIHandler).ShowMobileIcon(buildTarget);
		}
		((AvatarUIHandlerRemote)avatar.AvatarUIHandler).HealthBar.Oxygen = 0f;
		InitializeHealth();
		InitializeShield();
		triggerCollider = CreateTriggerCollider();
		if (MVGameControllerBase.Game.MVPlayerContainer.LocalPlayer.IsReady)
		{
			InitAvatarState();
		}
		else
		{
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerReady, new Action(InitAvatarState));
		}
		avatarRemoteMovementCalculator = gameObject.AddComponent<AvatarRemoteMovementCalculator>();
		limbManager = new AvatarLimbManagerRemote();
		limbManager.Initialize(this, Body, avatar.EnabledChangeHandler, LimbRotationRuntimeData);
		gameObject.SetActive(value: false);
		MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(OwnerActorNr).NotifyAvatarCreated(Id);
	}

	private void InitAvatarState()
	{
		AvatarStateChangedHandler(SpawnRoleModeTypes.Value);
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(InitAvatarState));
	}

	public override void Destroy()
	{
		base.Destroy();
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(InitAvatarState));
		cullingHandler.DeActivateCulling();
		ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Remove(ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(cullingHandler.UpdateCullingRadius));
	}

	private void InitializeHealth()
	{
		MVRuntimeDataVariable<float> health = Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			TrySpawningHealParticles(((AvatarUIHandlerRemote)avatar.AvatarUIHandler).HealthBar.Health, Health.Value);
			((AvatarUIHandlerRemote)avatar.AvatarUIHandler).HealthBar.Health = (float)obj;
		}));
		MVRuntimeDataVariable<float> maxHealth = MaxHealth;
		maxHealth.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(maxHealth.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			((AvatarUIHandlerRemote)avatar.AvatarUIHandler).HealthBar.MaxHealth = (float)obj;
		}));
		((AvatarUIHandlerRemote)avatar.AvatarUIHandler).HealthBar.Health = Health.Value;
		Body.InitializeHealth(Health.Value);
	}

	private void InitializeShield()
	{
		MVRuntimeDataVariableClampedFloat mVRuntimeDataVariableClampedFloat = Shield;
		mVRuntimeDataVariableClampedFloat.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(mVRuntimeDataVariableClampedFloat.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object shield) =>
		{
			TrySpawningHealParticles(((AvatarUIHandlerRemote)avatar.AvatarUIHandler).ShieldBar.Shield, Shield.Value);
			((AvatarUIHandlerRemote)avatar.AvatarUIHandler).ShieldBar.Shield = (float)shield;
		}));
		((AvatarUIHandlerRemote)avatar.AvatarUIHandler).ShieldBar.Shield = Shield.Value;
		Body.InitializeShield(Shield.Value);
	}

	private CapsuleCollider CreateTriggerCollider()
	{
		GameObject gameObject = new GameObject("triggerCollider");
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.layer = LayerMask.NameToLayer("Player");
		CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.center = new Vector3(0f, 1f, 0f);
		capsuleCollider.isTrigger = true;
		CapsuleCollider capsuleCollider2 = (CapsuleCollider)base.gameObject.GetComponent<Collider>();
		capsuleCollider.height = capsuleCollider2.height;
		capsuleCollider.radius = capsuleCollider2.radius;
		TriggerBoxEvents triggerBoxEvents = gameObject.AddComponent<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		return capsuleCollider;
	}

	protected override void OnAnimationChange(object newAnimationData)
	{
		base.OnAnimationChange(newAnimationData);
		if (((int)SpawnRoleModeTypes.Value & 1) > 0)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)newAnimationData;
			Body.Animation.Play((string)dictionary["state"]);
		}
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (Time.time - prevHitTime < 2f)
		{
			return;
		}
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID);
		if (!(worldObjectClient is MVVehicleBase))
		{
			return;
		}
		MVRigidBody mVRigidBody = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if (!(mVRigidBody != null))
		{
			return;
		}
		Vector3 velocity = mVRigidBody.Velocity;
		velocity /= Time.deltaTime;
		if (!(velocity.magnitude < minVelocity))
		{
			float num = Mathf.Clamp(velocity.magnitude, velocityMinMagnitude, velocityMaxMagnitude);
			num *= impulseMagnitudeFactor;
			velocity.y = 0f;
			velocity.Normalize();
			velocity.y = 1f;
			velocity.Normalize();
			velocity *= num;
			InteractionDataHandlerBase interactionDataHandlerBase = gameObject.GetComponent<InteractionDataHandlerBase>();
			if (interactionDataHandlerBase != null)
			{
				Debug.Log("Applying impulse " + velocity);
				interactionDataHandlerBase.HandleInteraction(ImpulseHitPackage.Create(velocity), interactionIsLocal: false);
				prevHitTime = Time.time;
			}
		}
	}

	protected override void AvatarStateChangedHandler(object a)
	{
		healParticleSpawnTime = Time.time;
		base.AvatarStateChangedHandler(a);
		int num = (int)a;
		if ((num & 4) > 0)
		{
			Body.Visible = false;
			((AvatarUIHandlerRemote)avatar.AvatarUIHandler).NameTagLabelVisible = false;
			triggerCollider.enabled = false;
		}
		else
		{
			Body.Visible = true;
			((AvatarUIHandlerRemote)avatar.AvatarUIHandler).SetHealthBarColor(MVGameControllerBase.Game.LocalPlayer.IsOnSameTeam(this));
			((AvatarUIHandlerRemote)avatar.AvatarUIHandler).NameTagLabelVisible = true;
			triggerCollider.enabled = true;
		}
	}

	protected override void AttachBody(MVBody newBody)
	{
		base.AttachBody(newBody);
		newBody.Visible = true;
	}

	public override void OnEnterVehicle()
	{
		base.OnEnterVehicle();
		IsInVehicle = true;
	}

	public override void OnLeaveVehicle()
	{
		base.OnLeaveVehicle();
		IsInVehicle = false;
		HandleLeaveVehicle();
	}

	public void VisualizeBulletImpact(VoxelHit voxelHit, Ray lineOfFire, int shooterActorNumber, float damage = 100f)
	{
		MVPlayer player = null;
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(shooterActorNumber, out player) && !player.IsOnSameTeam(this) && !IsInMode(SpawnRoleModeType.Dead) && !avatar.HasModifierEffect(AvatarModifierEffect.Invulnerable))
		{
			avatar.VisualizeBulletImpact(voxelHit, lineOfFire, shooterActorNumber, damage);
			if (shooterActorNumber == MVGameControllerBase.Game.LocalPlayer.ActorNr)
			{
				MVGameControllerBase.MainCameraManager.PlayPlingSound();
				MVGameControllerBase.PlayModeUI.GetCrossHair().ShowHasHitEffect();
			}
		}
	}

	public void Activate(int idFrom, Vector3 position, Quaternion rotation)
	{
		Debug.Log("MVAvatarRemote Activate");
		Position = position;
		Rotation = rotation;
		((MVNetworkListener)MVGameControllerBase.Game.TransformNetworkManager.GetNetworkObject(Id))?.SetToCurrentPosition();
		gameObject.SetActive(value: true);
		avatar.AvatarUIHandler.Activate();
		cullingHandler.ActivateCulling(gameObject);
		ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Combine(ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(cullingHandler.UpdateCullingRadius));
	}

	public void DeActivate(int idTo)
	{
		gameObject.SetActive(value: false);
		avatar.AvatarUIHandler.Deactivate();
		cullingHandler.DeActivateCulling();
		ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Remove(ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(cullingHandler.UpdateCullingRadius));
	}
}
