using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public abstract class MVAvatar : MVGroup, IHealRayAttachementObject
{
	protected const float healParticleSpawnCooldownTime = 1f;

	protected Avatar avatar;

	public MVRuntimeDataVariableClampedFloat Health;

	private MVRuntimeDataVariableClampedFloat shield;

	public MVRuntimeDataVariable Modifiers;

	public MVRuntimeDataVariable CurrentItem;

	public MVRuntimeDataVariable IsFiring;

	public MVRuntimeDataVariable Animation;

	public MVRuntimeDataVariable avatarModeTypeFlags;

	private readonly Vector3 characterControllerCenterOffset = new Vector3(0f, 0.95f, 0f);

	protected float healParticleSpawnTime;

	private Ray lineOfFire;

	private bool isLocal;

	private MVBody body;

	private GameObject healRayAttachmentObject;

	protected AvatarlateUpdateManager avatarlateUpdateManager;

	protected AvatarPickupOwner avatarPickupOwner;

	protected AvatarLimbManager limbManager;

	public MVRuntimeDataVariableClampedFloat Shield
	{
		get
		{
			return shield;
		}
		set
		{
			shield = value;
		}
	}

	public int AvatarModeTypeFlags
	{
		get
		{
			return (int)avatarModeTypeFlags.Value;
		}
		set
		{
			avatarModeTypeFlags.Value = value;
		}
	}

	public Vector3 CharacterControllerCenterOffset => characterControllerCenterOffset;

	public PickupItem CurrentPickup => avatarPickupOwner.CurrentItem;

	public AvatarLimbManager LimbManager => limbManager;

	public float SetTransparency
	{
		set
		{
			Avatar.AvatarFader.SetTransparency(value);
		}
	}

	public MVBody Body => body;

	public Avatar Avatar => avatar;

	public abstract Vector3 VelocityRelative { get; }

	public abstract Vector3 VelocityAbsolute { get; }

	public MVAvatar(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVAvatarPrefab, worldObjects)
	{
		isLocal = OwnerActorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr;
		interactionFlags = InteractionFlags.None;
		PlayInteractionType = PlayInteractionType.HandlesHits;
		Health = RuntimeDataVariables.NewClampedFloat("health", 0.2f, writeThrough: false, 0f, 100f);
		shield = RuntimeDataVariables.NewClampedFloat("shield", 0.2f, writeThrough: false, 0f, 100f);
		IsFiring = RuntimeDataVariables.New("isFiring", 0f, writeThrough: false);
		Modifiers = RuntimeDataVariables.New("modifiers", 1f, writeThrough: false);
		CurrentItem = RuntimeDataVariables.New("currentItem", 0f, writeThrough: true);
		avatarModeTypeFlags = RuntimeDataVariables.New("avatarModeTypes", 0f, writeThrough: true);
		Animation = RuntimeDataVariables.New("animation", 0f, writeThrough: false);
		gameObject.layer = LayerMask.NameToLayer("Player");
		avatar = gameObject.GetComponent<Avatar>();
	}

	public bool IsInMode(AvatarModeTypes t)
	{
		return (int)((uint)AvatarModeTypeFlags & (uint)t) > 0;
	}

	public virtual void BeforeVehicleEntered()
	{
	}

	public virtual void OnEnterVehicle()
	{
	}

	public virtual void OnLeaveVehicle()
	{
		Debug.Log("Do the thing with the legs");
	}

	protected void HandleLeaveVehicle()
	{
		if (!(Group is MVVehicleBase))
		{
			Debug.LogError("Trying to leave vehicle but Group is not vehicleBase " + GetType());
			return;
		}
		VehicleSeatManager vehicleSeatManager = Group.GameObject.GetComponent<VehicleSeatManager>();
		if (vehicleSeatManager == null)
		{
			Debug.LogError("Did not find seatmanager. Cannot detach");
			return;
		}
		vehicleSeatManager.DetachFromSeat(this);
		AvatarPickupOwner avatarPickupOwner = gameObject.GetComponent<AvatarPickupOwner>();
		if (avatarPickupOwner == null)
		{
			Debug.LogError("Could not find AvatarPickupOwner");
		}
		else
		{
			avatarPickupOwner.AdditionalIgnoreWOIDS = null;
		}
	}

	public void SetTeam()
	{
		avatar.UpdateNameTag();
		if (!avatar.IsLocal)
		{
			avatar.SetHealthBarColor(MVGameControllerBase.Game.TeamManager.IsOnSameTeam(this, MVGameControllerBase.Game.LocalPlayer.Avatar));
			return;
		}
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			value.Avatar.avatar.UpdateNameTag();
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		body.Attach(this, isLocal);
		avatarPickupOwner = gameObject.AddComponent<AvatarPickupOwner>();
		avatarPickupOwner.IsLocal = isLocal;
		avatarPickupOwner.Init(CurrentItem, IsFiring, this);
		avatar.Initialize(this, isLocal);
		avatar.InteractionDataHandlerBase.FindWorldObjectParent();
		InitializeModifiers();
		avatarlateUpdateManager = transform.gameObject.AddComponent<AvatarlateUpdateManager>();
		healParticleSpawnTime = Time.time;
		MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(OwnerActorNr)?.SetAvatar(Id);
		BodyData.PartIndex part = BodyData.PartIndex.Head;
		healRayAttachmentObject = Body.BodyData.GetPartBone(part).gameObject;
		MVRuntimeDataVariable mVRuntimeDataVariable = avatarModeTypeFlags;
		mVRuntimeDataVariable.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(mVRuntimeDataVariable.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChangedHandler));
	}

	protected virtual void AvatarStateChangedHandler(object a)
	{
		AvatarModeTypes avatarModeTypes = (AvatarModeTypes)(int)a;
		if ((avatarModeTypes & AvatarModeTypes.Hidden) > AvatarModeTypes.None)
		{
			avatar.Collider.enabled = false;
			avatar.InteractionDataHandlerBase.enabled = false;
		}
		else
		{
			avatar.Collider.enabled = true;
			avatar.InteractionDataHandlerBase.enabled = true;
		}
	}

	private void InitializeModifiers()
	{
		MVRuntimeDataVariable modifiers = Modifiers;
		modifiers.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(modifiers.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			avatar.UpdateModifiers((Dictionary<object, object>)obj);
		}));
		avatar.UpdateModifiers((Dictionary<object, object>)Modifiers.Value);
	}

	public override void AddChild(MVWorldObjectClient child)
	{
		base.AddChild(child);
		if (body != null)
		{
			body.Detach();
		}
		if (child is MVBody mVBody)
		{
			body = mVBody;
		}
	}

	public override void TransferChild(int id)
	{
		base.TransferChild(id);
		Debug.Log("Transfer child");
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(id);
		if (worldObjectClient is MVBody newBody)
		{
			AttachBody(newBody);
		}
	}

	public virtual void AttachBody(MVBody newBody)
	{
		if (body != null)
		{
			body.Detach();
		}
		if (newBody == null)
		{
			Debug.Log("newBody == null");
			return;
		}
		Debug.Log("Created avatar fader");
		newBody.Position = new Vector3(0f, 0.03f, 0f);
		newBody.Rotation = Quaternion.identity;
		newBody.Attach(this, isLocal);
		body = newBody;
	}

	protected void TrySpawningHealParticles(float previousHealth, float currentHealth)
	{
		float num = Time.time - healParticleSpawnTime;
		if (currentHealth > previousHealth && num > 1f)
		{
			healParticleSpawnTime = Time.time;
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate(PrefabPool.Instance.HealingParticles);
			Vector3 vector = gameObject.transform.position;
			vector.y += 0.6f;
			particleSystem.transform.position = vector;
			particleSystem.transform.SetParent(transform);
			Avatar.StartBlinking(BlinkType.Healing, 1.5f);
		}
	}

	public GameObject GetHealRayAttachmentObject()
	{
		return healRayAttachmentObject;
	}
}
