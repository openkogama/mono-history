using System;
using System.Collections.Generic;
using MV.Common;
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

	public MVRuntimeDataVariable AvatarRuntimeDataState;

	private readonly Vector3 characterControllerCenterOffset = new Vector3(0f, 0.95f, 0f);

	private Ray lineOfFire;

	private bool isLocal;

	private MVBody body;

	protected AvatarPickupOwner avatarPickupOwner;

	protected AvatarFader avatarFader;

	public AvatarRuntimeState AvatarRuntimeState
	{
		get
		{
			return (AvatarRuntimeState)(byte)AvatarRuntimeDataState.Value;
		}
		protected set
		{
			AvatarRuntimeDataState.Value = (byte)value;
		}
	}

	public Vector3 CharacterControllerCenterOffset => characterControllerCenterOffset;

	public float SetTransparency
	{
		set
		{
			if (avatarFader != null)
			{
				avatarFader.SetTransparency(value);
			}
		}
	}

	public MVBody Body => body;

	public Avatar Avatar => avatar;

	public virtual Vector3 Velocity
	{
		get
		{
			throw new Exception("not implemented");
		}
	}

	public MVAvatar(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVAvatarPrefab, worldObjects)
	{
		isLocal = OwnerActorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr;
		interactionFlags = InteractionFlags.None;
		PlayInteractionType = PlayInteractionType.HandlesHits;
		Health = RuntimeDataVariables.NewClampedFloat("health", 0.2f, writeThrough: false, 0f, 100f);
		IsFiring = RuntimeDataVariables.New("isFiring", 0f, writeThrough: false);
		Modifiers = RuntimeDataVariables.New("modifiers", 1f, writeThrough: false);
		CurrentItem = RuntimeDataVariables.New("currentItem", 0f, writeThrough: true);
		Invulnerable = RuntimeDataVariables.New("invulnerable", 0.2f, writeThrough: true);
		AvatarRuntimeDataState = RuntimeDataVariables.New("avatarRuntimeState", 0f, writeThrough: true);
		Animation = RuntimeDataVariables.New("animation", 0f, writeThrough: false);
		gameObject.layer = LayerMask.NameToLayer("Player");
		avatar = gameObject.GetComponent<Avatar>();
	}

	public virtual void BeforeVehicleEntered()
	{
	}

	public virtual void VehicleEntered()
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
		VehicleSeatManager component = Group.GameObject.GetComponent<VehicleSeatManager>();
		if (component == null)
		{
			Debug.LogError("Did not find seatmanager. Cannot detach");
			return;
		}
		component.DetachFromSeat(this);
		AvatarPickupOwner component2 = gameObject.GetComponent<AvatarPickupOwner>();
		if (component2 == null)
		{
			Debug.LogError("Could not find AvatarPickupOwner");
		}
		else
		{
			component2.AdditionalIgnoreWOIDS = null;
		}
	}

	public void SetTeam()
	{
		avatar.UpdateNameTag();
	}

	public override void Initialize()
	{
		base.Initialize();
		avatarFader = new AvatarFader(Body.Transform);
		gameObject.AddComponent<InteractionDataHandler>();
		avatarPickupOwner = gameObject.AddComponent<AvatarPickupOwner>();
		avatarPickupOwner.Init(CurrentItem, IsFiring, this, body);
		avatarPickupOwner.IsLocal = isLocal;
		avatar.Initialize(this, isLocal);
		InitializeModifiers();
		MVGameControllerBase.Game.Players.TryGetValue(OwnerActorNr, out var value);
		if (value != null)
		{
			value.Avatar = this;
		}
		MVRuntimeDataVariable avatarRuntimeDataState = AvatarRuntimeDataState;
		avatarRuntimeDataState.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarRuntimeDataState.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChangedHandler));
	}

	protected virtual void AvatarStateChangedHandler(object a)
	{
		if ((byte)a == 0)
		{
			gameObject.GetComponent<Collider>().enabled = false;
			gameObject.GetComponent<InteractionDataHandlerBase>().enabled = false;
		}
		else
		{
			gameObject.GetComponent<Collider>().enabled = true;
			gameObject.GetComponent<InteractionDataHandlerBase>().enabled = true;
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
		if (child is MVBody mVBody)
		{
			body = mVBody;
			body.Attach(this, isLocal);
		}
	}

	public override void TransferChild(int id)
	{
		base.TransferChild(id);
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
		if (newBody != null)
		{
			newBody.Position = new Vector3(0f, 0.03f, 0f);
			newBody.Rotation = Quaternion.identity;
			newBody.Attach(this, isLocal);
			body = newBody;
		}
	}
}
