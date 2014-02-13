using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVPickupItemBase : MVLogicObject
{
	private bool canPickUp = true;

	private static Dictionary<AvatarItemType, string> pickupPrefabLUT = new Dictionary<AvatarItemType, string>
	{
		{
			AvatarItemType.CenterGun,
			"Prefabs/Pickups/PickupItemCenterGun"
		},
		{
			AvatarItemType.ImpulseGun,
			"Prefabs/Pickups/PickupItemImpulseGun"
		},
		{
			AvatarItemType.Health,
			"Prefabs/Pickups/PickupItemHealthPack"
		},
		{
			AvatarItemType.Bazooka,
			"Prefabs/Pickups/PickupItemBazooka"
		},
		{
			AvatarItemType.RailGun,
			"Prefabs/Pickups/PickupItemRailGun"
		},
		{
			AvatarItemType.Mutant,
			"Prefabs/Pickups/PickupItemMutant"
		},
		{
			AvatarItemType.Sword,
			"Prefabs/Pickups/PickupItemSword"
		},
		{
			AvatarItemType.Shotgun,
			"Prefabs/Pickups/PickupItemShotgun"
		},
		{
			AvatarItemType.Flamethrower,
			"Prefabs/Pickups/PickupItemFlamethrower"
		},
		{
			AvatarItemType.CubeGun,
			"Prefabs/Pickups/PickupItemCubeGun"
		}
	};

	private AvatarItemType pickupItemType;

	private int pickupVariantId;

	private TriggerBoxEvents triggerBoxEvents;

	private GameObject pickupMesh;

	private PickupItemObjectScript pickupItem;

	public AvatarItemType Type => pickupItemType;

	public int VariantID => pickupVariantId;

	public Hashtable ItemData
	{
		get
		{
			if (Data.Contains("itemData"))
			{
				return (Hashtable)Data["itemData"];
			}
			return null;
		}
	}

	public MVPickupItemBase(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, GetPickupPrefabName(data), worldObjects)
	{
		pickupItem = gameObject.GetComponent<PickupItemObjectScript>();
		pickupMesh = pickupItem.pickupObject;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if ((Object)(object)triggerBoxEvents != (Object)null)
		{
			triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
			triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		}
		else
		{
			Debug.LogError((object)("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name));
		}
		OnDataUpdate();
	}

	private static string GetPickupPrefabName(Hashtable data)
	{
		Hashtable hashtable = (Hashtable)data[WorldObjectDataParameters.Data];
		string text = pickupPrefabLUT[(AvatarItemType)(int)hashtable["itemType"]];
		int num = (hashtable.ContainsKey("variantId") ? ((int)hashtable["variantId"]) : 0);
		return text + num;
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		((Component)gameObject.transform.FindChild("Cube")).gameObject.active = false;
	}

	public override void OnDataUpdate()
	{
		if (Data.ContainsKey("variantId"))
		{
			pickupVariantId = (int)Data["variantId"];
			Debug.Log((object)("*** Update variant id " + pickupVariantId));
		}
		if (Data.ContainsKey("itemType"))
		{
			pickupItemType = (AvatarItemType)(int)Data["itemType"];
		}
	}

	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		pickupMesh.transform.Rotate(Vector3.up, 68f * Time.deltaTime);
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (e.instigatorWOID == -1)
		{
			return;
		}
		if (canPickUp)
		{
			MVEquipable component = MVGameController.Instance.WOCM.GetWorldObjectClient(e.instigatorWOID).GameObject.GetComponent<MVEquipable>();
			if ((Object)(object)component != (Object)null)
			{
				component.Equip(Type, ItemData, VariantID);
			}
		}
		canPickUp = false;
		MVGameController.Instance.Game.TriggerBoxEnter(Id, e.instigatorWOID);
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		MVGameController.Instance.Game.TriggerBoxExit(Id, e.instigatorWOID);
	}

	public void HandleStateChange(PickupItemState state)
	{
		switch (state)
		{
		case PickupItemState.Listening:
			canPickUp = true;
			pickupItem.Respawn();
			((Component)triggerBoxEvents).collider.enabled = true;
			break;
		case PickupItemState.Pickup:
			pickupItem.Take();
			if (Object.op_Implicit((Object)(object)gameObject.audio))
			{
				gameObject.audio.Play();
			}
			canPickUp = false;
			((Component)triggerBoxEvents).collider.enabled = false;
			break;
		case PickupItemState.Counting:
			break;
		}
	}
}
