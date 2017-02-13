using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVPickupItemBase : MVLogicObject, IUpdatecontrollerSubscriber, IPickupStateHandler
{
	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private static Dictionary<AvatarItemType, EquipableData> pickupPrefabLUT = new Dictionary<AvatarItemType, EquipableData>
	{
		{
			AvatarItemType.CenterGun,
			new EquipableData(PrefabPool.Instance.AvatarCenterGunPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.ImpulseGun,
			new EquipableData(PrefabPool.Instance.AvatarImpulseGunPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.Health,
			new EquipableData(PrefabPool.Instance.AvatarHealthPrefab, AvatarEquipableType.Modifier)
		},
		{
			AvatarItemType.Bazooka,
			new EquipableData(PrefabPool.Instance.AvatarBazookaPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.RailGun,
			new EquipableData(PrefabPool.Instance.AvatarRailGunPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.Mutant,
			new EquipableData(PrefabPool.Instance.AvatarMutantPrefab, AvatarEquipableType.Modifier)
		},
		{
			AvatarItemType.Sword,
			new EquipableData(PrefabPool.Instance.AvatarSwordPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.Shotgun,
			new EquipableData(PrefabPool.Instance.AvatarShotgunPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.Flamethrower,
			new EquipableData(PrefabPool.Instance.AvatarFlamethrowerPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.CubeGun,
			new EquipableData(PrefabPool.Instance.AvatarCubeGunPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.NinjaRun,
			new EquipableData(PrefabPool.Instance.AvatarNinjaRunPrefab, AvatarEquipableType.Modifier)
		},
		{
			AvatarItemType.SixShooter,
			new EquipableData(PrefabPool.Instance.AvatarSixShooterPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.DoubleSixShooter,
			new EquipableData(PrefabPool.Instance.AvatarDoubleSixShooterPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.ThrowingStar,
			new EquipableData(PrefabPool.Instance.AvatarThrowingStarPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.MultiThrowingStar,
			new EquipableData(PrefabPool.Instance.AvatarMultiThrowingStarPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.MouseGun,
			new EquipableData(PrefabPool.Instance.AvatarMouseGunPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.GrowthGun,
			new EquipableData(PrefabPool.Instance.AvatarGrowthGunPrefab, AvatarEquipableType.Weapon)
		},
		{
			AvatarItemType.MousePack,
			new EquipableData(PrefabPool.Instance.AvatarMousePackPrefab, AvatarEquipableType.Modifier)
		},
		{
			AvatarItemType.GrowthPack,
			new EquipableData(PrefabPool.Instance.AvatarGrowthPackPrefab, AvatarEquipableType.Modifier)
		}
	};

	private AvatarItemType pickupItemType;

	private int pickupVariantId;

	private bool canPickUp = true;

	private UseInteractor useInteractor;

	private MVPickupItemBaseObject baseObject;

	private List<int> instigatorsInTrigger = new List<int>();

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
		baseObject = (MVPickupItemBaseObject)component;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		baseObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		baseObject.TriggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
	}

	void IUpdatecontrollerSubscriber.UpdateControllerUpdate()
	{
		for (int i = 0; i < instigatorsInTrigger.Count; i++)
		{
			if (!canPickUp)
			{
				continue;
			}
			bool flag = true;
			if (pickupPrefabLUT[Type].equipableType == AvatarEquipableType.Weapon)
			{
				int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(instigatorsInTrigger[i]);
				MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDWithLocalOwnerHighestInHierarchy);
				if (worldObjectClient != null)
				{
					MVPickupOwner mVPickupOwner = worldObjectClient.GameObject.GetComponent<MVPickupOwner>();
					if (mVPickupOwner != null && !(mVPickupOwner is VehiclePickupOwner) && mVPickupOwner.CurrentItem != null && mVPickupOwner.CurrentItem.Type != Type)
					{
						flag = mVPickupOwner.CurrentItem.Type == AvatarItemType.Hand;
					}
				}
			}
			if ((useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0 && flag)
			{
				DoPickup(instigatorsInTrigger[i]);
			}
		}
	}

	void IUpdatecontrollerSubscriber.UpdateControllerFixedUpdate()
	{
	}

	private static ObjectPrefab GetPickupPrefabName(Dictionary<object, object> data)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)data[WorldObjectDataParameters.Data];
		return pickupPrefabLUT[(AvatarItemType)(int)dictionary["itemType"]].prefabObject;
	}

	private void SetupUseInteractor()
	{
		useInteractor = new UseInteractor(this, baseObject.useInteractionRotator, reset: false, baseObject.TriggerBoxEvents.Collider, DoPickup, CheckCanUse);
		GameCoinLogic useRequirement = new GameCoinLogic(baseObject.useInteractionRotator);
		useInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(baseObject.useInteractionRotator);
		useInteractor.AddRequirement(useRequirement2);
		baseObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		baseObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
	}

	public override void Initialize()
	{
		SetupUseInteractor();
		OnDataUpdate();
		useInteractor.UpdateData(Data);
		base.Initialize();
		baseObject.PickupItem.pickupObject.AddComponent<RotateLocal>().rotationSpeed = 68f;
		SetupCulling(baseObject.PickupItem.pickupObject);
		cullingSubscriberBase.DistanceBandIndex = 2;
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public override void Destroy()
	{
		if (useInteractor != null)
		{
			baseObject.TriggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
			baseObject.TriggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
			useInteractor.OnDestroy(Data);
			useInteractor = null;
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

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (e.instigatorWOID != -1)
		{
			instigatorsInTrigger.Add(e.instigatorWOID);
		}
	}

	private bool CheckCanUse(MVInteractableBase avatarInteractable)
	{
		if (avatarInteractable.HasModifierEffect(AvatarModifierEffect.DisablePickups) || avatarInteractable.HasModifierEffect(AvatarModifierEffect.DisableWeapons))
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
		MVEquipable mVEquipable = worldObjectClient.GameObject.GetComponent<MVEquipable>();
		if (mVEquipable != null && mVEquipable.Equip(Type, pickupPrefabLUT[Type].equipableType, ItemData, VariantID))
		{
			MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, instigatorWOID);
			canPickUp = false;
			return true;
		}
		return false;
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		MVGameControllerBase.OperationRequests.TriggerBoxExit(Id, e.instigatorWOID);
		instigatorsInTrigger.Remove(e.instigatorWOID);
	}

	public void HandleStateChange(PickupItemState state)
	{
		switch (state)
		{
		case PickupItemState.Listening:
			canPickUp = true;
			baseObject.PickupItem.GreyIn();
			baseObject.TriggerBoxEvents.Collider.enabled = true;
			break;
		case PickupItemState.Pickup:
			baseObject.PickupItem.GreyOut();
			if ((bool)baseObject.AudioSource)
			{
				baseObject.AudioSource.Play();
			}
			canPickUp = false;
			baseObject.TriggerBoxEvents.Collider.enabled = false;
			break;
		case PickupItemState.Counting:
			break;
		}
	}
}
