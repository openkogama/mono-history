using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVPickupItemBase : MVLogicObject
{
	private bool canPickUp = true;

	private GameCoinLogic gameCoinLogic;

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
		},
		{
			AvatarItemType.NinjaRun,
			"Prefabs/Pickups/PickupItemNinjaRun"
		},
		{
			AvatarItemType.SixShooter,
			"Prefabs/Pickups/PickupItemSixShooter"
		},
		{
			AvatarItemType.DoubleSixShooter,
			"Prefabs/Pickups/PickupItemDoubleSixShooter"
		},
		{
			AvatarItemType.ThrowingStar,
			"Prefabs/Pickups/PickupItemThrowingStar"
		},
		{
			AvatarItemType.MultiThrowingStar,
			"Prefabs/Pickups/PickupItemMultiThrowingStar"
		}
	};

	private AvatarItemType pickupItemType;

	private int pickupVariantId;

	private TriggerBoxEvents triggerBoxEvents;

	private GameObject pickupMesh;

	private GreyOutObjectScript pickupItem;

	public AvatarItemType Type => pickupItemType;

	public int VariantID => pickupVariantId;

	public Dictionary<object, object> ItemData
	{
		get
		{
			if (Data.ContainsKey("itemData"))
			{
				return (Dictionary<object, object>)Data["itemData"];
			}
			return null;
		}
	}

	public MVPickupItemBase(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, GetPickupPrefabName(data), worldObjects)
	{
		pickupItem = gameObject.GetComponent<GreyOutObjectScript>();
		pickupMesh = pickupItem.pickupObject;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if (triggerBoxEvents != null)
		{
			triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
			triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		gameCoinLogic = new GameCoinLogic(gameObject, Data);
		OnDataUpdate();
	}

	private static string GetPickupPrefabName(Dictionary<object, object> data)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)data[WorldObjectDataParameters.Data];
		string text = pickupPrefabLUT[(AvatarItemType)(int)dictionary["itemType"]];
		int num = (dictionary.ContainsKey("variantId") ? ((int)dictionary["variantId"]) : 0);
		return text + num;
	}

	public override void Destroy()
	{
		base.Destroy();
		if (gameCoinLogic != null)
		{
			gameCoinLogic.OnDestroy(Data);
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		gameObject.transform.FindChild("Cube").gameObject.SetActive(value: false);
	}

	public override void OnDataUpdate()
	{
		if (Data.ContainsKey("variantId"))
		{
			pickupVariantId = (int)Data["variantId"];
			Debug.Log("*** Update variant id " + pickupVariantId);
		}
		if (Data.ContainsKey("itemType"))
		{
			pickupItemType = (AvatarItemType)(int)Data["itemType"];
		}
		gameCoinLogic.OnDataUpdate(Data);
	}

	protected override void OnUpdate()
	{
		pickupMesh.transform.Rotate(Vector3.up, 68f * Time.deltaTime);
		if (gameCoinLogic.PurchaseAmount > 0 && triggerBoxEvents.IsInTrigger && gameCoinLogic.ShowUseGUI())
		{
			DoPickup(MVGameController.WOCM.AvatarLocal.Id);
		}
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (e.instigatorWOID != -1 && canPickUp && gameCoinLogic.PurchaseAmount <= 0)
		{
			DoPickup(e.instigatorWOID);
		}
	}

	private void DoPickup(int instigatorWOID)
	{
		MVEquipable component = MVGameController.WOCM.GetWorldObjectClient(instigatorWOID).GameObject.GetComponent<MVEquipable>();
		if (component != null)
		{
			component.Equip(Type, ItemData, VariantID);
		}
		MVGameController.Game.TriggerBoxEnter(Id, instigatorWOID);
		canPickUp = false;
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		MVGameController.Game.TriggerBoxExit(Id, e.instigatorWOID);
	}

	public void HandleStateChange(PickupItemState state)
	{
		switch (state)
		{
		case PickupItemState.Listening:
			canPickUp = true;
			pickupItem.GreyIn();
			triggerBoxEvents.GetComponent<Collider>().enabled = true;
			break;
		case PickupItemState.Pickup:
			pickupItem.GreyOut();
			if ((bool)gameObject.GetComponent<AudioSource>())
			{
				gameObject.GetComponent<AudioSource>().Play();
			}
			canPickUp = false;
			triggerBoxEvents.GetComponent<Collider>().enabled = false;
			break;
		case PickupItemState.Counting:
			break;
		}
	}
}
