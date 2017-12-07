using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public class MVAvatarRemote : MVAvatar, IBulletImpactVisualizer
{
	private const float initialCullingRadius = 3.5f;

	private const float hitTimeOut = 2f;

	private CullingSubscriberDynamic cullingSubscriberDynamic;

	private HealthBar healthBar;

	private CapsuleCollider triggerCollider;

	private AvatarRemoteMovementCalculator avatarRemoteMovementCalculator;

	private float impulseMagnitudeFactor = 0.6f;

	private float velocityMinMagnitude = 1500f;

	private float velocityMaxMagnitude = 5000f;

	private float minVelocity = 700f;

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
		: base(data, worldObjects)
	{
		SetNetworkObject(local: false);
		IsInVehicle = false;
	}

	public override void Initialize()
	{
		base.Initialize();
		avatar.UpdateNameTag();
		if (MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(OwnerActorNr).BuildTarget == BuildTarget.Android)
		{
			avatar.ShowMobileIcon();
		}
		healthBar = gameObject.GetComponentInChildren<HealthBar>();
		healthBar.Oxygen = 0f;
		InitializeHealth();
		triggerCollider = CreateTriggerCollider();
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerReady, new Action(InitAvatarState));
		avatarRemoteMovementCalculator = gameObject.AddComponent<AvatarRemoteMovementCalculator>();
		InitializeCulling();
	}

	private void InitAvatarState()
	{
		AvatarStateChangedHandler(avatarModeTypeFlags.Value);
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(InitAvatarState));
	}

	public override void Destroy()
	{
		base.Destroy();
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(InitAvatarState));
		if (cullingSubscriberDynamic != null)
		{
			cullingSubscriberDynamic.Destroy();
			cullingSubscriberDynamic = null;
		}
	}

	private void InitializeCulling()
	{
		cullingSubscriberDynamic = new CullingSubscriberDynamic(3.5f, 3, gameObject);
		ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Combine(ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(UpdateCullingRadius));
	}

	private void UpdateCullingRadius(MVWorldObjectClient objArg, ScaleChangedEventArgs scaleArg)
	{
		CullingSubscriberDynamic cullingSubscriberDynamic = this.cullingSubscriberDynamic;
		Vector3 newScale = scaleArg.NewScale;
		cullingSubscriberDynamic.SetCullingRadius(3.5f * newScale.y);
	}

	private void InitializeHealth()
	{
		MVRuntimeDataVariableClampedFloat health = Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			healthBar.Health = (float)obj;
		}));
		healthBar.Health = Health.Value;
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
		base.AvatarStateChangedHandler(a);
		int num = (int)a;
		if ((num & 4) > 0)
		{
			Body.Visible = false;
			avatar.NameTagLabelVisible = false;
			triggerCollider.enabled = false;
		}
		else
		{
			Body.Visible = true;
			avatar.SetHealthBarColor(MVGameControllerBase.Game.TeamManager.IsOnSameTeam(this, MVGameControllerBase.Game.LocalPlayer.Avatar));
			avatar.NameTagLabelVisible = true;
			triggerCollider.enabled = true;
		}
	}

	public override void AttachBody(MVBody newBody)
	{
		base.AttachBody(newBody);
		newBody.Visible = true;
	}

	public override void OnEnterVehicle()
	{
		IsInVehicle = true;
	}

	public override void OnLeaveVehicle()
	{
		IsInVehicle = false;
		HandleLeaveVehicle();
	}

	public void VisualizeBulletImpact(VoxelHit voxelHit, Ray lineOfFire, int shooterActorNumber, float damage = 100f)
	{
		MVPlayer player = null;
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(shooterActorNumber, out player) && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(this, player.Avatar) && !IsInMode(AvatarModeTypes.Dead) && !avatar.HasModifierEffect(AvatarModifierEffect.Invulnerable))
		{
			avatar.VisualizeBulletImpact(voxelHit, lineOfFire, shooterActorNumber, damage);
			if (shooterActorNumber == MVGameControllerBase.Game.LocalPlayer.ActorNr)
			{
				MVGameControllerBase.CameraController.PlayPlingSound();
				MVGameControllerBase.IPlayModeUI.GetCrossHair().ShowHasHitEffect();
			}
		}
	}
}
