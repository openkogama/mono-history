using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVAvatar : MVGroup
{
	protected Avatar avatar;

	public MVRuntimeDataVariableClampedFloat Health;

	public MVRuntimeDataVariable Modifiers;

	public MVRuntimeDataVariable CurrentItem;

	public MVRuntimeDataVariable IsFiring;

	public MVRuntimeDataVariable Invulnerable;

	public MVRuntimeDataVariable Animation;

	public MVRuntimeDataVariable CollectibleCount;

	private readonly Vector3 characterControllerCenterOffset = new Vector3(0f, 0.95f, 0f);

	private Ray lineOfFire;

	private bool isLocal;

	private static string prefabPath = "Prefabs/Avatar/Avatar";

	private MVBody body;

	protected AvatarPickupOwner avatarPickupOwner;

	public Vector3 CharacterControllerCenterOffset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return characterControllerCenterOffset;
		}
	}

	public MVBody Body => body;

	public MVAvatar(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabPath, worldObjects)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		isLocal = (int)Data["actorNr"] == MVGameController.Instance.Game.LocalPlayer.ActorNr;
		interactionFlags = InteractionFlags.None;
		PlayInteractionType = PlayInteractionType.HandlesHits;
		Health = RuntimeDataVariables.NewClampedFloat("health", 0.2f, writeThrough: false, 0f, 100f);
		IsFiring = RuntimeDataVariables.New("isFiring", 0f, writeThrough: false);
		Modifiers = RuntimeDataVariables.New("modifiers", 1f, writeThrough: false);
		CurrentItem = RuntimeDataVariables.New("currentItem", 0f, writeThrough: true);
		Invulnerable = RuntimeDataVariables.New("invulnerable", 0.2f, writeThrough: true);
		Animation = RuntimeDataVariables.New("animation", 0f, writeThrough: false);
		CollectibleCount = RuntimeDataVariables.New("collectibleCount", 0f, writeThrough: false);
		gameObject.layer = LayerMask.NameToLayer("Player");
		avatar = gameObject.GetComponent<Avatar>();
	}

	public virtual void VehicleEntered()
	{
	}

	public virtual void OnLeaveVehicle()
	{
		Debug.Log((object)"Do the thing with the legs");
	}

	protected void HandleLeaveVehicle()
	{
		if (!(Group is MVVehicleBase))
		{
			Debug.LogError((object)("Trying to leave vehicle but Group is not vehicleBase " + GetType()));
			return;
		}
		VehicleSeatManager component = Group.GameObject.GetComponent<VehicleSeatManager>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogError((object)"Did not find seatmanager. Cannot detach");
			return;
		}
		component.DetachFromSeat(this);
		AvatarPickupOwner component2 = gameObject.GetComponent<AvatarPickupOwner>();
		if ((Object)(object)component2 == (Object)null)
		{
			Debug.LogError((object)"Could not find AvatarPickupOwner");
		}
		else
		{
			component2.AdditionalIgnoreWOIDS = null;
		}
	}

	public void SetTeam()
	{
		avatar.NameTag = (string)Data["ownerUserName"];
	}

	public override void Initialize()
	{
		base.Initialize();
		gameObject.AddComponent<InteractionDataHandler>();
		avatarPickupOwner = gameObject.AddComponent<AvatarPickupOwner>();
		avatarPickupOwner.Init(CurrentItem, IsFiring, this, body);
		avatarPickupOwner.IsLocal = isLocal;
		avatar.Initialize(this, isLocal);
		InitializeHealth();
		InitializeModifiers();
		MVGameController.Instance.Game.Players.TryGetValue(OwnerActorNr, out var value);
		if (value != null)
		{
			value.Avatar = this;
		}
	}

	private void InitializeModifiers()
	{
		MVRuntimeDataVariable modifiers = Modifiers;
		modifiers.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(modifiers.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			avatar.UpdateModifiers((Hashtable)obj);
		}));
		avatar.UpdateModifiers((Hashtable)Modifiers.Value);
	}

	private void InitializeHealth()
	{
		MVRuntimeDataVariableClampedFloat health = Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			avatar.Health = (float)obj;
		}));
		avatar.Health = Health.Value;
	}

	public override void AddChild(MVWorldObjectClient child)
	{
		base.AddChild(child);
		if (child is MVBody mVBody)
		{
			body = mVBody;
			body.Attach(this, isLocal);
		}
	}

	public override void TransferChild(int id)
	{
		base.TransferChild(id);
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(id);
		if (worldObjectClient is MVBody newBody)
		{
			AttachBody(newBody);
		}
	}

	protected virtual void AttachBody(MVBody newBody)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (body != null)
		{
			body.Detach();
		}
		if (newBody != null)
		{
			newBody.Position = new Vector3(0f, 0.03f, 0f);
			newBody.Rotation = Quaternion.identity;
			newBody.Attach(this, isLocal);
			body = newBody;
		}
	}
}
