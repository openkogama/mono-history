using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVAvatarRemote : MVAvatar
{
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

	public override Vector3 Velocity => avatarRemoteMovementCalculator.VelocityEstimate;

	public MVAvatarRemote(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		SetNetworkObject(local: false);
	}

	public override void Initialize()
	{
		base.Initialize();
		avatar.UpdateNameTag();
		if (MVGameControllerBase.Game.Players[avatar.mvAvatar.OwnerActorNr].BuildTarget == BuildTarget.Android)
		{
			avatar.ShowMobileIcon();
		}
		healthBar = gameObject.GetComponentInChildren<HealthBar>();
		healthBar.Oxygen = 0f;
		InitializeHealth();
		triggerCollider = CreateTriggerCollider();
		AvatarStateChangedHandler(AvatarRuntimeDataState.Value);
		avatarRemoteMovementCalculator = gameObject.AddComponent<AvatarRemoteMovementCalculator>();
		cullingSubscriberDynamic = new CullingSubscriberDynamic(3.5f, 3, 2, gameObject);
	}

	public override void Destroy()
	{
		base.Destroy();
		if (cullingSubscriberDynamic != null)
		{
			cullingSubscriberDynamic.Destroy();
			cullingSubscriberDynamic = null;
		}
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
		if ((byte)a == 0)
		{
			Body.Visible = false;
			avatar.NameTagLabelVisible = false;
			triggerCollider.enabled = false;
		}
		else
		{
			Body.Visible = true;
			avatar.NameTagLabelVisible = true;
			triggerCollider.enabled = true;
		}
	}

	public override void AttachBody(MVBody newBody)
	{
		base.AttachBody(newBody);
		newBody.Visible = true;
	}

	public override void OnLeaveVehicle()
	{
		HandleLeaveVehicle();
	}
}
