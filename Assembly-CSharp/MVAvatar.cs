using System;
using MV.WorldObject;
using UnityEngine;

public class MVAvatar : MVWorldObjectClient
{
	private bool isLocal;

	private Avatar avatar;

	public MVRuntimeDataVariableClampedFloat Health;

	public MVRuntimeDataVariable Animation;

	public MVRuntimeDataVariable CenterSlot;

	public MVRuntimeDataVariable RightSlot;

	public MVRuntimeDataVariable LeftSlot;

	public MVRuntimeDataVariable AboveSlot;

	public MVRuntimeDataVariable IsFiring;

	private AvatarController avatarController;

	public Avatar Avatar => avatar;

	public AvatarController AvatarController => avatarController;

	protected override void CreateMVWOC(bool isLocal)
	{
		this.isLocal = isLocal;
		interactionFlags = InteractionFlags.None;
		Health = RuntimeDataVariables.NewClampedFloat("health", 0.2f, writeThrough: false, 0f, 100f);
		Animation = RuntimeDataVariables.New("animation", 0f, writeThrough: false);
		CenterSlot = RuntimeDataVariables.New("centerSlot", 0f, writeThrough: true);
		RightSlot = RuntimeDataVariables.New("rightSlot", 0f, writeThrough: true);
		LeftSlot = RuntimeDataVariables.New("leftSlot", 0f, writeThrough: true);
		AboveSlot = RuntimeDataVariables.New("aboveSlot", 0f, writeThrough: true);
		IsFiring = RuntimeDataVariables.New("isFiring", 0f, writeThrough: false);
		Object val = Object.Instantiate(Resources.Load("Prefabs/Avatar/Avatar"));
		gameObject = (GameObject)(object)((val is GameObject) ? val : null);
		gameObject.layer = LayerMask.NameToLayer("Player");
		((Object)gameObject).name = GetType().ToString();
		avatar = gameObject.GetComponent<Avatar>();
		avatar.Initialize(this, isLocal);
		avatar.NameTag = ((!isLocal) ? MVGameController.Instance.WOCM.Players[OwnerActorNr].Username : string.Empty);
		InitializeAnimation();
		InitializeHealth();
		InitializeAvatarItemSlots();
		if (isLocal)
		{
			MVGameController.Instance.WOCM.AvatarId = id;
			avatarController = gameObject.AddComponent<AvatarController>();
			avatarController.Initialize(this, avatar);
		}
		else
		{
			Object.Destroy((Object)(object)gameObject.GetComponent<MvCharacterController>());
			MVGameController.Instance.WOCM.Players[OwnerActorNr].Avatar = this;
		}
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

	private void InitializeAnimation()
	{
		MVRuntimeDataVariable animation = Animation;
		animation.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(animation.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			avatar.SetAnimation((string)obj);
		}));
		avatar.SetAnimation((string)Animation.Value);
	}

	private void InitializeAvatarItemSlots()
	{
		MVRuntimeDataVariable centerSlot = CenterSlot;
		centerSlot.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(centerSlot.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object prefabName) =>
		{
			avatar.Equip(AvatarItemSlotName.Center, (string)prefabName);
		}));
		avatar.Equip(AvatarItemSlotName.Center, (string)CenterSlot.Value);
		MVRuntimeDataVariable rightSlot = RightSlot;
		rightSlot.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(rightSlot.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object prefabName) =>
		{
			avatar.Equip(AvatarItemSlotName.Right, (string)prefabName);
		}));
		avatar.Equip(AvatarItemSlotName.Right, (string)RightSlot.Value);
		MVRuntimeDataVariable leftSlot = LeftSlot;
		leftSlot.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(leftSlot.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object prefabName) =>
		{
			avatar.Equip(AvatarItemSlotName.Left, (string)prefabName);
		}));
		avatar.Equip(AvatarItemSlotName.Left, (string)LeftSlot.Value);
		MVRuntimeDataVariable aboveSlot = AboveSlot;
		aboveSlot.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(aboveSlot.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object prefabName) =>
		{
			avatar.Equip(AvatarItemSlotName.Above, (string)prefabName);
		}));
		avatar.Equip(AvatarItemSlotName.Above, (string)AboveSlot.Value);
		MVRuntimeDataVariable isFiring = IsFiring;
		isFiring.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isFiring.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object value) =>
		{
			avatar.HandleFiring((bool)value);
		}));
	}

	public void HandlePickupItem(WorldObjectType type, int instigatorActorNr)
	{
		Debug.Log((object)"AVATAR: HandlePickupItem");
		switch (type)
		{
		case WorldObjectType.PickupItemHealthPack:
			Health.Value += 50f;
			break;
		case WorldObjectType.PickupItemCenterGun:
			CenterSlot.Value = string.Empty;
			CenterSlot.Value = "Prefabs/AvatarItemCenterGun";
			break;
		}
	}

	public void Unequip(AvatarItemSlotName slotName)
	{
		switch (slotName)
		{
		case AvatarItemSlotName.Center:
			CenterSlot.Value = string.Empty;
			break;
		case AvatarItemSlotName.Right:
			RightSlot.Value = string.Empty;
			break;
		case AvatarItemSlotName.Left:
			LeftSlot.Value = string.Empty;
			break;
		case AvatarItemSlotName.Above:
			AboveSlot.Value = string.Empty;
			break;
		}
	}

	public void UnequipAll()
	{
		CenterSlot.Value = string.Empty;
		RightSlot.Value = string.Empty;
		LeftSlot.Value = string.Empty;
		AboveSlot.Value = string.Empty;
	}
}
