using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVPickupItemBase : MVLogicObject, IPickupStateHandler, IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberBase
{
	private const UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private MVWorldObjectDocumentationType documentationType;

	private AvatarItemType pickupItemType;

	private int pickupVariantId;

	private bool canPickUp = true;

	private UseInteractor useInteractor;

	private MVPickupItemBaseObject baseObject;

	private static readonly Dictionary<AvatarItemType, MVWorldObjectDocumentationType> avatarItemToToinventoryItemDescrip = new Dictionary<AvatarItemType, MVWorldObjectDocumentationType>
	{
		{
			AvatarItemType.Health,
			MVWorldObjectDocumentationType.HealthPack
		},
		{
			AvatarItemType.CenterGun,
			MVWorldObjectDocumentationType.Centergun
		},
		{
			AvatarItemType.ImpulseGun,
			MVWorldObjectDocumentationType.ImpulseGun
		},
		{
			AvatarItemType.Bazooka,
			MVWorldObjectDocumentationType.Bazooka
		},
		{
			AvatarItemType.RailGun,
			MVWorldObjectDocumentationType.Railgun
		},
		{
			AvatarItemType.Sword,
			MVWorldObjectDocumentationType.Sword
		},
		{
			AvatarItemType.Mutant,
			MVWorldObjectDocumentationType.Mutant
		},
		{
			AvatarItemType.Flamethrower,
			MVWorldObjectDocumentationType.Flamethrower
		},
		{
			AvatarItemType.Shotgun,
			MVWorldObjectDocumentationType.Shotgun
		},
		{
			AvatarItemType.GrowthPack,
			MVWorldObjectDocumentationType.GrowthPill
		},
		{
			AvatarItemType.MousePack,
			MVWorldObjectDocumentationType.MousePill
		},
		{
			AvatarItemType.MouseGun,
			MVWorldObjectDocumentationType.MouseGun
		},
		{
			AvatarItemType.ThrowingStar,
			MVWorldObjectDocumentationType.ThrowingStar
		},
		{
			AvatarItemType.MultiThrowingStar,
			MVWorldObjectDocumentationType.MultiThrowingStar
		},
		{
			AvatarItemType.CubeGun,
			MVWorldObjectDocumentationType.CubeGun
		},
		{
			AvatarItemType.DoubleSixShooter,
			MVWorldObjectDocumentationType.DoubleSixShooter
		},
		{
			AvatarItemType.GrowthGun,
			MVWorldObjectDocumentationType.GrowthGun
		},
		{
			AvatarItemType.SixShooter,
			MVWorldObjectDocumentationType.SixShooter
		},
		{
			AvatarItemType.NinjaRun,
			MVWorldObjectDocumentationType.NinjaRun
		},
		{
			AvatarItemType.HealRay,
			MVWorldObjectDocumentationType.HealRay
		}
	};

	private List<int> instigatorsInTrigger = new List<int>();

	protected override bool HasVisualsInPlaymode => true;

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

	public override MVWorldObjectDocumentationType DocumentationType => documentationType;

	public MVPickupItemBase(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, GetPickupPrefabName(data), worldObjects)
	{
		baseObject = (MVPickupItemBaseObject)component;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		interactionFlags |= InteractionFlags.CanUseGameRank;
		Dictionary<object, object> dictionary = (Dictionary<object, object>)data[WorldObjectDataParameters.Data];
		AvatarItemType key = (AvatarItemType)dictionary["itemType"];
		documentationType = (avatarItemToToinventoryItemDescrip.ContainsKey(key) ? avatarItemToToinventoryItemDescrip[key] : MVWorldObjectDocumentationType.Missing);
	}

	private static ObjectPrefab GetPickupPrefabName(Dictionary<object, object> data)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)data[WorldObjectDataParameters.Data];
		return PrefabPool.PickupPrefabLUT[(AvatarItemType)dictionary["itemType"]].prefabObject;
	}

	private void SetupUseInteractor()
	{
		useInteractor = new UseInteractor(this, baseObject.useInteractionRotator, reset: false, baseObject.TriggerBoxEvents.Collider, DoPickup, CheckCanUse);
		GameCoinLogic useRequirement = new GameCoinLogic(baseObject.useInteractionRotator);
		useInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(baseObject.useInteractionRotator);
		useInteractor.AddRequirement(useRequirement2);
		GameRankRequirement useRequirement3 = new GameRankRequirement(baseObject.useInteractionRotator, this, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement3);
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
		baseObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		baseObject.TriggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		if (PrefabPool.PickupPrefabLUT[Type].equipableType == AvatarEquipableType.Weapon)
		{
			MVGameControllerBase.Game.LocalPlayer.BoostController.AllowBoost(BoostType.AmmoIntMultiplier, allowed: true);
		}
	}

	public override void Destroy()
	{
		baseObject.TriggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		baseObject.TriggerBoxEvents.TriggerExit -= triggerBoxEvents_TriggerExit;
		if (useInteractor != null)
		{
			baseObject.TriggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
			baseObject.TriggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
			useInteractor.OnDestroy(Data);
			useInteractor = null;
		}
		UpdateController.RemoveUpdateObject(this);
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
		gameObject.transform.Find("Cube").gameObject.SetActive(value: false);
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
			pickupItemType = (AvatarItemType)Data["itemType"];
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

	private bool CheckCanUse(int woId, MVInteractableBase avatarInteractable)
	{
		if (avatarInteractable.HasModifierEffect(AvatarModifierEffect.DisablePickups) || avatarInteractable.HasModifierEffect(AvatarModifierEffect.DisableWeapons))
		{
			return false;
		}
		if (!canPickUp)
		{
			return false;
		}
		woId = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(woId);
		if (ShouldDoAutoPickup(woId))
		{
			return false;
		}
		return true;
	}

	private bool DoPickup(int instigatorWOID)
	{
		if (MVGameControllerBase.Game.PlayerController.IsEnteringVehicle)
		{
			return false;
		}
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instigatorWOID);
		if (worldObjectClient.GameObject.GetComponent<MVInteractableBase>().HasModifierEffect(AvatarModifierEffect.DisablePickups))
		{
			return false;
		}
		MVEquipable mVEquipable = worldObjectClient.GameObject.GetComponent<MVEquipable>();
		if (mVEquipable != null && mVEquipable.Equip(Type, PrefabPool.PickupPrefabLUT[Type].equipableType, ItemData, VariantID))
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
		}
	}

	private bool ShouldDoAutoPickup(int instigator)
	{
		bool flag = true;
		bool flag2 = false;
		if (PrefabPool.PickupPrefabLUT[Type].equipableType == AvatarEquipableType.Weapon)
		{
			int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(instigator);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDWithLocalOwnerHighestInHierarchy);
			if (worldObjectClient != null)
			{
				MVPickupOwner mVPickupOwner = worldObjectClient.GameObject.GetComponent<MVPickupOwner>();
				if (mVPickupOwner != null && !(mVPickupOwner is VehiclePickupOwner) && mVPickupOwner.CurrentItem != null)
				{
					flag2 = mVPickupOwner.CurrentItem.Type == Type;
					if (!flag2)
					{
						flag = mVPickupOwner.CurrentItem.Type == AvatarItemType.Hand;
					}
				}
			}
		}
		if ((useInteractor.EvaluateRequirementsUsability() & (UseGUIResult.CanAfford | UseGUIResult.CannotAfford)) == 0 && flag)
		{
			return true;
		}
		if ((useInteractor.GetGUIShowOptions() & ShowUseOption.UsingGameCoins) == 0 && (useInteractor.EvaluateRequirementsUsability() & UseGUIResult.CannotAfford) == 0 && flag2 && flag)
		{
			return true;
		}
		return false;
	}

	void IUpdatecontrollerSubscriberUpdate.UpdateControllerUpdate()
	{
		if (!canPickUp)
		{
			return;
		}
		for (int num = instigatorsInTrigger.Count - 1; num >= 0; num--)
		{
			if (MVGameControllerBase.WOCM.GetWorldObjectClient(instigatorsInTrigger[num]) == null)
			{
				instigatorsInTrigger.RemoveAt(num);
			}
			else if (ShouldDoAutoPickup(instigatorsInTrigger[num]))
			{
				DoPickup(instigatorsInTrigger[num]);
			}
		}
	}
}
