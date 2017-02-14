using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class CollectTheItemCollectableInstance : MVBlueprintBase, ITriggerBoxEventsHandler, IPickupStateHandler
{
	private CollectTheItemObject collectTheItemObject;

	private CullingSubscriberBase cullingSubscriberBase;

	private UseInteractor useInteractor;

	private bool isTaken;

	private readonly float timeCreated = Time.time;

	private PickupItemState currentState;

	private bool IsOriginalInstance => !RunTimeData.ContainsObscuredKey("OriginalId");

	private int OriginalInstanceID
	{
		get
		{
			if (IsOriginalInstance)
			{
				return Id;
			}
			return (ObscuredInt)RunTimeData.GetObscuredType("OriginalId");
		}
	}

	public bool HasArrowIndicator => (bool)blueprintData["hasIndicator"];

	private int CollectTheItemCollectableID => MVGameControllerBase.WOCM.GetWorldObjectClient(OriginalInstanceID).GroupId;

	public CollectTheItemCollectableInstance(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.CollectTheItemCollectablePrefab, worldObjects)
	{
		InteractionFlags &= ~InteractionFlags.CanClone;
		collectTheItemObject = (CollectTheItemObject)component;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupInstance();
		if (!IsOriginalInstance)
		{
			InteractionFlags &= ~InteractionFlags.Selectable;
			SetBlinker();
			collectTheItemObject.Blinker.Visible = true;
		}
		else
		{
			if (MVGameControllerBase.GameMode == MVGameMode.Edit)
			{
				IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
				iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
			}
			collectTheItemObject.GreyOutScriptEditMode.InitializeOriginalMaterials();
		}
		SetupUseInteractor();
	}

	public override void Reset()
	{
		base.Reset();
		collectTheItemObject.VisualObject.SetActive(value: true);
		collectTheItemObject.Collider.enabled = true;
		if (IsOriginalInstance && MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			collectTheItemObject.GreyOutScriptEditMode.GreyIn();
		}
	}

	public void SetupGreyoutScript(object sender, EditStateEventArgs args)
	{
		SetBlinker();
		Reset();
		if (MVGameControllerBase.WOCM.GetWorldObjectClient(CollectTheItemCollectableID) is CollectTheItemCollectable collectTheItemCollectable && MVGameControllerBase.WOCM.GetWorldObjectClient(collectTheItemCollectable.DropOffId) is CollectTheItemDropOff collectTheItemDropOff)
		{
			collectTheItemDropOff.ReInitializeVisuals();
		}
	}

	private void InitializeInstanceWithData()
	{
		List<MVWorldObjectClient> list = Children;
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Transform.SetParent(collectTheItemObject.CullingObject.transform);
			list[i].Transform.rotation = Quaternion.identity;
		}
		collectTheItemObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		collectTheItemObject.InitializeGreyOutScript();
		SetBlinker();
		collectTheItemObject.Blinker.Visible = true;
	}

	private void SetupCulling()
	{
		cullingSubscriberBase = new CullingSubscriberBase(2f, Transform.position, OnStateChanged);
		PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(CollectTheItemCollectableID);
			worldObjectClient.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(worldObjectClient.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		}
	}

	private void SetupUseInteractor()
	{
		useInteractor = new UseInteractor(this, collectTheItemObject.gameObject, reset: false, collectTheItemObject.TriggerBoxEvents.Collider, SendEnterEvent, CheckCanUse);
		collectTheItemObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		collectTheItemObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
	}

	private void OnEditModeChange(EditModeChangeArgs arg)
	{
		collectTheItemObject.EditCollider.enabled = true;
		if (arg.playInEditor)
		{
			collectTheItemObject.EditCollider.enabled = false;
		}
	}

	private void SetupInstance()
	{
		InitializeInstanceWithData();
		SetupCulling();
		HashSet<int> hashSet = new HashSet<int>(WorldIDsRecursive);
		foreach (int item in hashSet)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(item);
			if (worldObjectClient is MVCubeModelInstance mVCubeModelInstance)
			{
				mVCubeModelInstance.Visible = true;
				mVCubeModelInstance.GameObject.SetLayerRecursively(LayerMask.NameToLayer("Player"));
			}
		}
		if (MVGameControllerBase.WOCM.GetWorldObjectClient(CollectTheItemCollectableID) is CollectTheItemCollectable collectTheItemCollectable && MVGameControllerBase.WOCM.GetWorldObjectClient(collectTheItemCollectable.DropOffId) is CollectTheItemDropOff collectTheItemDropOff)
		{
			collectTheItemObject.EnableFading = !IsOriginalInstance;
			collectTheItemDropOff.OnPickupCollected = (Action<bool>)Delegate.Combine(collectTheItemDropOff.OnPickupCollected, new Action<bool>(OnCollected));
		}
	}

	private void OnCollected(bool shouldBeActiveOnCollect)
	{
		collectTheItemObject.Collider.enabled = shouldBeActiveOnCollect;
		collectTheItemObject.VisualObject.SetActive(shouldBeActiveOnCollect);
		if (IsOriginalInstance && MVGameControllerBase.GameMode == MVGameMode.Edit && !shouldBeActiveOnCollect)
		{
			collectTheItemObject.GreyOutScriptEditMode.GreyOut();
		}
		else if (!IsOriginalInstance && !shouldBeActiveOnCollect)
		{
			collectTheItemObject.TriggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
			isTaken = true;
		}
	}

	private void SetBlinker()
	{
		collectTheItemObject.Blinker.MeshFilters = collectTheItemObject.VisualObject.GetComponentsInChildren<MeshFilter>();
	}

	private bool CheckCanUse(MVInteractableBase interactable)
	{
		if (interactable.HasModifierEffect(AvatarModifierEffect.DisableWeapons) || interactable.HasModifierEffect(AvatarModifierEffect.DisablePickups))
		{
			return false;
		}
		if (isTaken)
		{
			return false;
		}
		if (CanPickupWithoutUse(MVGameControllerBase.WOCM.AvatarLocal.Id))
		{
			return false;
		}
		return true;
	}

	private bool CanPickupWithoutUse(int instigator)
	{
		int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(instigator);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDWithLocalOwnerHighestInHierarchy);
		if (worldObjectClient != null)
		{
			MVPickupOwner mVPickupOwner = worldObjectClient.GameObject.GetComponent<MVPickupOwner>();
			if (mVPickupOwner != null)
			{
				if (mVPickupOwner.CurrentItem == null || (mVPickupOwner.CurrentItem != null && mVPickupOwner.CurrentItem.Type == AvatarItemType.Hand))
				{
					return true;
				}
			}
			else if (worldObjectClient is MVJetPack)
			{
				MVPickupOwner mVPickupOwner2 = MVGameControllerBase.WOCM.AvatarLocal.GameObject.GetComponent<MVPickupOwner>();
				if (mVPickupOwner2 != null && (mVPickupOwner2.CurrentItem == null || (mVPickupOwner2.CurrentItem != null && mVPickupOwner2.CurrentItem.Type == AvatarItemType.Hand)))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (CanPickupWithoutUse(e.instigatorWOID))
		{
			SendEnterEvent(e.instigatorWOID);
		}
	}

	private bool SendEnterEvent(int instigator)
	{
		if (!IsOriginalInstance && Time.time - timeCreated < 0.1f)
		{
			return false;
		}
		if (currentState != PickupItemState.Listening)
		{
			return false;
		}
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instigator);
		if (worldObjectClient == null)
		{
			return false;
		}
		MVInteractableBase mVInteractableBase = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (mVInteractableBase == null)
		{
			return false;
		}
		if (mVInteractableBase.HasModifierEffect(AvatarModifierEffect.DisablePickups) || mVInteractableBase.HasModifierEffect(AvatarModifierEffect.DisableWeapons))
		{
			return false;
		}
		if (!IsOriginalInstance && isTaken)
		{
			return false;
		}
		MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, instigator);
		return true;
	}

	private void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = positionChangedEventArgs.NewPos;
	}

	public void OnStateChanged(CullingGroupEvent cullingEvent)
	{
		bool active = CullingApiWrapper.Visible(cullingEvent, cullingSubscriberBase.DistanceBandIndex);
		collectTheItemObject.CullingObject.SetActive(active);
	}

	private bool DoPickup(int instigatorWOID)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instigatorWOID);
		if (worldObjectClient == null)
		{
			return false;
		}
		MVEquipable mVEquipable = worldObjectClient.GameObject.GetComponent<MVEquipable>();
		if (mVEquipable == null)
		{
			return false;
		}
		mVEquipable.Unequip();
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		if (MVGameControllerBase.WOCM.GetWorldObjectClient(OriginalInstanceID) == null)
		{
			return false;
		}
		dictionary.Add("CollectTheItemCollectableId", CollectTheItemCollectableID);
		if (mVEquipable.Equip(AvatarItemType.CollectTheItemCollectable, AvatarEquipableType.Weapon, dictionary))
		{
			return true;
		}
		return false;
	}

	public void Enter(int instigatorWoID)
	{
		if (!MVGameControllerBase.IsPlaying)
		{
			return;
		}
		if (!IsOriginalInstance && isTaken)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instigatorWoID);
			if (worldObjectClient == null)
			{
				return;
			}
			MVEquipable mVEquipable = worldObjectClient.GameObject.GetComponent<MVEquipable>();
			if (mVEquipable == null)
			{
				return;
			}
			mVEquipable.Unequip();
		}
		if (DoPickup(instigatorWoID))
		{
			isTaken = true;
		}
	}

	public void Exit()
	{
	}

	public void SetRotationEnabled(bool enableRotation)
	{
		collectTheItemObject.RotateLocal.enabled = enableRotation;
		collectTheItemObject.RotateLocal.transform.rotation = Quaternion.identity;
	}

	public override void Destroy()
	{
		base.Destroy();
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		if (PositionChanged != null)
		{
			PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Remove(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		}
		if (cullingSubscriberBase != null)
		{
			MVGroup mVGroup = (MVGroup)MVGameControllerBase.WOCM.GetWorldObjectClient(MVGameControllerBase.WOCM.RootGroup.Id);
			mVGroup.RemoveChild(Id);
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
		if (MVGameControllerBase.WOCM.GetWorldObjectClient(OriginalInstanceID) != null && MVGameControllerBase.WOCM.GetWorldObjectClient(CollectTheItemCollectableID) is CollectTheItemCollectable collectTheItemCollectable)
		{
			if (collectTheItemCollectable.PositionChanged != null)
			{
				collectTheItemCollectable.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Remove(collectTheItemCollectable.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
			}
			if (MVGameControllerBase.WOCM.GetWorldObjectClient(collectTheItemCollectable.DropOffId) is CollectTheItemDropOff collectTheItemDropOff)
			{
				collectTheItemDropOff.OnPickupCollected = (Action<bool>)Delegate.Remove(collectTheItemDropOff.OnPickupCollected, new Action<bool>(OnCollected));
			}
		}
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		if (!IsOriginalInstance)
		{
			return true;
		}
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(groupId);
		collectTheItemObject.TriggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
		collectTheItemObject.TriggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
		return worldObjectClient?.Delete(worldObjectClientManager, ref errorText) ?? false;
	}

	public void HandleStateChange(PickupItemState state)
	{
		currentState = state;
		switch (state)
		{
		case PickupItemState.Listening:
			isTaken = false;
			collectTheItemObject.GreyOutObject.GreyIn();
			break;
		case PickupItemState.Pickup:
			collectTheItemObject.GreyOutObject.GreyOut();
			isTaken = true;
			break;
		case PickupItemState.Counting:
			break;
		}
	}
}
