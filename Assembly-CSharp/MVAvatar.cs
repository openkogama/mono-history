using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public abstract class MVAvatar : MVGroup, IHealRayAttachementObject, IUpdatecontrollerSubscriberLateUpdate, IUpdatecontrollerSubscriberBase
{
	protected Avatar avatar;

	public MVRuntimeDataVariable<float> Health;

	public MVRuntimeDataVariable<int> MaxHealth;

	private MVRuntimeDataVariableClampedFloat shield;

	public MVRuntimeDataVariable Modifiers;

	public MVRuntimeDataVariable CurrentItem;

	public MVRuntimeDataVariable IsFiring;

	public MVRuntimeDataVariable Animation;

	public MVRuntimeDataVariable SpawnRoleModeTypes;

	public LimbRotationRuntimeData LimbRotationRuntimeData = new LimbRotationRuntimeData();

	private readonly Vector3 characterControllerCenterOffset = new Vector3(0f, 0.95f, 0f);

	protected const float healParticleSpawnCooldownTime = 1f;

	protected float healParticleSpawnTime;

	private Ray lineOfFire;

	private bool isLocal;

	private MVBody body;

	private GameObject healRayAttachmentObject;

	protected AvatarPickupOwner avatarPickupOwner;

	protected AvatarLimbManager limbManager;

	protected WorldObjectSkillDataManager skillDataManager;

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

	public Vector3 CharacterControllerCenterOffset => characterControllerCenterOffset;

	public float HealParticleSpawnTime
	{
		set
		{
			healParticleSpawnTime = value;
		}
	}

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

	public bool IsSeated
	{
		get
		{
			if (!RunTimeData.ContainsObscuredKey("seat"))
			{
				Debug.LogError("MVAvatar does not contain key seat");
				return false;
			}
			return (int)(ObscuredInt)RunTimeData.GetObscuredType("seat") != -1;
		}
	}

	public int SeatID
	{
		get
		{
			if (!RunTimeData.ContainsObscuredKey("seat"))
			{
				Debug.LogError("MVAvatar does not contain key seat");
				return -1;
			}
			return (ObscuredInt)RunTimeData.GetObscuredType("seat");
		}
		set
		{
			bool isSeated = IsSeated;
			RunTimeData.SetObscuredType("seat", (ObscuredInt)value);
			if (IsSeated != isSeated)
			{
				OnSeatedChanged(IsSeated);
			}
		}
	}

	public Avatar Avatar => avatar;

	public abstract Vector3 VelocityRelative { get; }

	public abstract Vector3 VelocityAbsolute { get; }

	public MVWorldObjectClient WorldObjectClient => this;

	public MVAvatar(Dictionary<object, object> data, GameObject avatarPrefab, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, avatarPrefab, worldObjects)
	{
		isLocal = OwnerActorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr;
		interactionFlags = InteractionFlags.None;
		PlayInteractionType = PlayInteractionType.HandlesHits;
		Health = RuntimeDataVariables.New<float>("health", 0.2f, writeThrough: false);
		MaxHealth = RuntimeDataVariables.New<int>("maxHealth", 0f, writeThrough: true);
		shield = RuntimeDataVariables.NewClampedFloat("shield", 0.2f, writeThrough: false, 0f, 100f);
		IsFiring = RuntimeDataVariables.New("isFiring", 0f, writeThrough: false);
		Modifiers = RuntimeDataVariables.New("modifiers", 1f, writeThrough: false);
		CurrentItem = RuntimeDataVariables.New("currentItem", 0f, writeThrough: true);
		SpawnRoleModeTypes = RuntimeDataVariables.New("spawnRoleModeType", 0f, writeThrough: true);
		Animation = RuntimeDataVariables.New("animation", 0f, writeThrough: false);
		LimbRotationRuntimeData.HeadRotationYaw = RuntimeDataVariables.New("headRotationYaw", 0.8f, writeThrough: false);
		LimbRotationRuntimeData.HeadRotationPitch = RuntimeDataVariables.New("headRotationPitch", 0.8f, writeThrough: false);
		LimbRotationRuntimeData.PointRotationYaw = RuntimeDataVariables.New("pointRotationYaw", 0.8f, writeThrough: false);
		LimbRotationRuntimeData.PointRotationPitch = RuntimeDataVariables.New("pointRotationPitch", 0.8f, writeThrough: false);
		LimbRotationRuntimeData.Emote = RuntimeDataVariables.New("emote", 0.5f, writeThrough: false);
		gameObject.layer = LayerMask.NameToLayer("Player");
		avatar = gameObject.GetComponent<Avatar>();
	}

	protected virtual void OnSeatedChanged(bool isSeated)
	{
	}

	public bool IsInMode(SpawnRoleModeType t)
	{
		return (int)((uint)(int)SpawnRoleModeTypes.Value & (uint)t) > 0;
	}

	public virtual void BeforeVehicleEntered()
	{
	}

	public virtual void OnEnterVehicle()
	{
		avatar.OnEnterVehicle();
	}

	public virtual void OnLeaveVehicle()
	{
		avatar.OnExitVehicle();
	}

	public override void Initialize()
	{
		base.Initialize();
		if (OwnerActorNr != -1)
		{
			body.Attach(this, isLocal);
			avatarPickupOwner = gameObject.AddComponent<AvatarPickupOwner>();
			avatarPickupOwner.IsLocal = isLocal;
			avatarPickupOwner.Init(CurrentItem, IsFiring, this, skillDataManager);
			avatar.Initialize(this, isLocal);
			avatar.InteractionDataHandlerBase.FindWorldObjectParent();
			InitializeModifiers();
			healParticleSpawnTime = Time.time;
			BodyData.PartIndex part = BodyData.PartIndex.Head;
			healRayAttachmentObject = Body.BodyData.GetPartBone(part).gameObject;
			MVRuntimeDataVariable spawnRoleModeTypes = SpawnRoleModeTypes;
			spawnRoleModeTypes.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(spawnRoleModeTypes.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChangedHandler));
			MVRuntimeDataVariable animation = Animation;
			animation.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(animation.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnAnimationChange));
			MVRuntimeDataVariable<float> health = Health;
			health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnHealthChange));
			MVRuntimeDataVariableClampedFloat mVRuntimeDataVariableClampedFloat = Shield;
			mVRuntimeDataVariableClampedFloat.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(mVRuntimeDataVariableClampedFloat.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnShieldChange));
			MVRuntimeDataVariable currentItem = CurrentItem;
			currentItem.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(currentItem.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnCurrentPickupChange));
			UpdateController.AddLateUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		avatar.AvatarUIHandler.ForceDestroy();
		UpdateController.RemoveLateUpdateObject(this);
	}

	public void SetTeam()
	{
		if (!avatar.IsLocal)
		{
			((AvatarUIHandlerRemote)avatar.AvatarUIHandler).UpdateNameTag();
			((AvatarUIHandlerRemote)avatar.AvatarUIHandler).SetHealthBarColor(MVGameControllerBase.Game.LocalPlayer.IsOnSameTeam(this));
			return;
		}
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			if (value.ActorNr != MVGameControllerBase.Game.LocalPlayer.ActorNr)
			{
				Debug.LogWarning("Reimplement with callback function. Spawn role interface should support this.");
			}
		}
	}

	public void UpdateControllerLateUpdate()
	{
		limbManager.UpdateLimbRotations(avatarPickupOwner.LookDirection);
		body.UpdateBlinking();
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

	protected virtual void AvatarStateChangedHandler(object a)
	{
		SpawnRoleModeType spawnRoleModeType = (SpawnRoleModeType)a;
		if ((spawnRoleModeType & SpawnRoleModeType.Hidden) > SpawnRoleModeType.None)
		{
			avatar.Collider.enabled = false;
			avatar.InteractionDataHandlerBase.enabled = false;
			OnStateChangeToHidden();
			avatar.AvatarUIHandler.SetShouldShowUI(shouldShow: false);
		}
		else
		{
			avatar.Collider.enabled = true;
			avatar.InteractionDataHandlerBase.enabled = true;
			avatar.AvatarUIHandler.SetShouldShowUI(shouldShow: true);
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

	protected virtual void AttachBody(MVBody newBody)
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

	protected void OnStateChangeToHidden()
	{
		avatar.ChatBubbleAnchor.HideChatBubble();
	}

	protected virtual void OnAnimationChange(object newAnimationData)
	{
		body.OnAnimationUpdate(newAnimationData);
	}

	protected virtual void OnHealthChange(object newHealthData)
	{
		body.OnHealthUpdate(newHealthData);
	}

	protected virtual void OnShieldChange(object newShieldData)
	{
		body.OnShieldUpdate(newShieldData);
	}

	protected virtual void OnCurrentPickupChange(object newPickupDataData)
	{
	}
}
