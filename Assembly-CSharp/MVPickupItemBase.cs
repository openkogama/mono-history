using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVPickupItemBase : MVLogicObject
{
	private bool canPickUp = true;

	private UseInteractor useInteractor;

	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private static Dictionary<AvatarItemType, EquipableData> pickupPrefabLUT = new Dictionary<AvatarItemType, EquipableData>
	{
		{
			AvatarItemType.CenterGun,
			new EquipableData("Prefabs/Pickups/PickupItemCenterGun", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.ImpulseGun,
			new EquipableData("Prefabs/Pickups/PickupItemImpulseGun", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.Health,
			new EquipableData("Prefabs/Pickups/PickupItemHealthPack", AvatarEquipableType.Modifier)
		},
		{
			AvatarItemType.Bazooka,
			new EquipableData("Prefabs/Pickups/PickupItemBazooka", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.RailGun,
			new EquipableData("Prefabs/Pickups/PickupItemRailGun", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.Mutant,
			new EquipableData("Prefabs/Pickups/PickupItemMutant", AvatarEquipableType.Modifier)
		},
		{
			AvatarItemType.Sword,
			new EquipableData("Prefabs/Pickups/PickupItemSword", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.Shotgun,
			new EquipableData("Prefabs/Pickups/PickupItemShotgun", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.Flamethrower,
			new EquipableData("Prefabs/Pickups/PickupItemFlamethrower", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.CubeGun,
			new EquipableData("Prefabs/Pickups/PickupItemCubeGun", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.NinjaRun,
			new EquipableData("Prefabs/Pickups/PickupItemNinjaRun", AvatarEquipableType.Modifier)
		},
		{
			AvatarItemType.SixShooter,
			new EquipableData("Prefabs/Pickups/PickupItemSixShooter", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.DoubleSixShooter,
			new EquipableData("Prefabs/Pickups/PickupItemDoubleSixShooter", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.ThrowingStar,
			new EquipableData("Prefabs/Pickups/PickupItemThrowingStar", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.MultiThrowingStar,
			new EquipableData("Prefabs/Pickups/PickupItemMultiThrowingStar", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.MouseGun,
			new EquipableData("Prefabs/Pickups/PickupItemMouseGun", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.GrowthGun,
			new EquipableData("Prefabs/Pickups/PickupItemGrowthGun", AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.MousePack,
			new EquipableData("Prefabs/Pickups/PickupItemMousePack", AvatarEquipableType.Modifier)
		},
		{
			AvatarItemType.GrowthPack,
			new EquipableData("Prefabs/Pickups/PickupItemGrowthPack", AvatarEquipableType.Modifier)
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
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		pickupItem = gameObject.GetComponent<GreyOutObjectScript>();
		pickupMesh = pickupItem.pickupObject;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		useInteractor = new UseInteractor(Id, gameObject, reset: false, triggerBoxEvents.GetComponent<Collider>(), DoPickup, CheckCanUse);
		GameCoinLogic useRequirement = new GameCoinLogic(gameObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(gameObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement2);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		OnDataUpdate();
	}

	private static string GetPickupPrefabName(Dictionary<object, object> data)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)data[WorldObjectDataParameters.Data];
		string prefabPath = pickupPrefabLUT[(AvatarItemType)(int)dictionary["itemType"]].prefabPath;
		int num = (dictionary.ContainsKey("variantId") ? ((int)dictionary["variantId"]) : 0);
		return prefabPath + num;
	}

	public override void Initialize()
	{
		useInteractor.UpdateData(Data);
		base.Initialize();
	}

	public override void Destroy()
	{
		if (useInteractor != null)
		{
			useInteractor.OnDestroy(Data);
		}
		base.Destroy();
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
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
		useInteractor.UpdateData(Data);
	}

	protected override void OnUpdate()
	{
		pickupMesh.transform.Rotate(Vector3.up, 68f * Time.deltaTime);
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (e.instigatorWOID != -1 && canPickUp && (useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0)
		{
			DoPickup(e.instigatorWOID);
		}
	}

	private bool CheckCanUse(MVInteractableBase avatarInteractable)
	{
		if (avatarInteractable.HasModifierEffect(AvatarModifierEffect.DisablePickups))
		{
			return false;
		}
		if (!canPickUp)
		{
			return false;
		}
		return true;
	}

	private bool DoPickup(int instigatorWOID)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instigatorWOID);
		if (worldObjectClient.GameObject.GetComponent<MVInteractableBase>().HasModifierEffect(AvatarModifierEffect.DisablePickups))
		{
			return false;
		}
		MVEquipable component = worldObjectClient.GameObject.GetComponent<MVEquipable>();
		if (component != null && component.Equip(Type, pickupPrefabLUT[Type].equipableType, ItemData, VariantID))
		{
			MVGameControllerBase.Game.TriggerBoxEnter(Id, instigatorWOID);
			canPickUp = false;
			return true;
		}
		return false;
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		MVGameControllerBase.Game.TriggerBoxExit(Id, e.instigatorWOID);
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
