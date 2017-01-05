using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class CollectTheItemDropOff : MVBlueprintBase, ITriggerBoxEventsHandler
{
	private EditableCubeModelWrapper editableCubeModelWrapper;

	private CollectTheItemDropOffObject triggerObject;

	private CullingSubscriberBase cullingSubscriberBase;

	private CollectTheItem controller;

	private ObscuredIntVector minBounds = new ObscuredIntVector(-5, -4, -6);

	private ObscuredIntVector maxBounds = new ObscuredIntVector(7, 7, 6);

	private ObscuredInt minCubes = 10;

	public Action OnPickupCollected;

	public override bool HasOutputConnector => true;

	public override bool HasInputConnector => false;

	public override Vector3 OutputConnectorOffset => new Vector3(3f, 0f, 0f);

	public CollectTheItemDropOff(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.CollectTheItemDropOffPrefab, worldObjects)
	{
		InteractionFlags |= InteractionFlags.CanEdit;
		InteractionFlags |= InteractionFlags.DirectlySelectable;
		InteractionFlags &= ~InteractionFlags.CanClone;
		triggerObject = (CollectTheItemDropOffObject)component;
	}

	public void InitializeWithController(CollectTheItem controller)
	{
		this.controller = controller;
	}

	public override void Initialize()
	{
		base.Initialize();
		OnPickupCollected = (Action)Delegate.Combine(OnPickupCollected, new Action(triggerObject.Blinker.OnBlinkingActivated));
		triggerObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		outputConnectorObject.transform.localScale = new Vector3(2f, 2f, 2f);
		MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)GetChild("DropOffModel");
		mVCubeModelInstance.Visible = true;
		mVCubeModelInstance.Transform.SetParent(triggerObject.VisualObject.transform);
		editableCubeModelWrapper = new EditableCubeModelWrapper(mVCubeModelInstance, new IntVector(minBounds.x, minBounds.y, minBounds.z), new IntVector(maxBounds.x, maxBounds.y, maxBounds.z), minCubes);
		triggerObject.Blinker.MeshFilters = mVCubeModelInstance.GameObject.GetComponentsInChildren<MeshFilter>();
		triggerObject.Blinker.Visible = true;
		SetupCulling();
	}

	private void SetupCulling()
	{
		cullingSubscriberBase = new CullingSubscriberBase(3.5f, Transform.position, OnStateChanged);
		PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
	}

	private void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = positionChangedEventArgs.NewPos;
	}

	public void OnStateChanged(CullingGroupEvent cullingEvent)
	{
		bool active = CullingApiWrapper.Visible(cullingEvent, cullingSubscriberBase.DistanceBandIndex);
		triggerObject.VisualObject.SetActive(active);
	}

	public void Exit()
	{
		SetLinks(isSet: false);
	}

	public void Enter(int instigatorWOID)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instigatorWOID);
		if (worldObjectClient == null)
		{
			return;
		}
		int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(instigatorWOID);
		if (woIDWithLocalOwnerHighestInHierarchy != -1)
		{
			MVGameControllerBase.OperationRequests.TriggerBoxExit(Id, instigatorWOID);
		}
		else
		{
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate(PrefabPool.Instance.CollectTheItemParticles);
			particleSystem.transform.position = worldObjectClient.Transform.position;
		}
		if (triggerObject.VisualObject.activeInHierarchy)
		{
			MVEquipable mVEquipable = worldObjectClient.GameObject.GetComponent<MVEquipable>();
			if (mVEquipable == null)
			{
				return;
			}
			MVPickupOwner componentInChildren = worldObjectClient.GameObject.GetComponentInChildren<MVPickupOwner>();
			if (componentInChildren == null)
			{
				return;
			}
			PickupItem currentItem = componentInChildren.CurrentItem;
			if (currentItem == null || !(currentItem is PickupItemCollectTheItem))
			{
				return;
			}
			mVEquipable.Unequip();
			PickupItemCollectTheItem pickupItemCollectTheItem = currentItem as PickupItemCollectTheItem;
			ParticleSystem particleSystem2 = UnityEngine.Object.Instantiate(PrefabPool.Instance.CollectTheItemParticles);
			particleSystem2.transform.position = pickupItemCollectTheItem.transform.position;
			if (OnPickupCollected != null)
			{
				OnPickupCollected();
			}
		}
		SetLinks(isSet: true);
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID);
		MVPickupOwner componentInChildren = worldObjectClient.GameObject.GetComponentInChildren<MVPickupOwner>();
		if (componentInChildren == null)
		{
			return;
		}
		PickupItem currentItem = componentInChildren.CurrentItem;
		if (!(currentItem == null) && currentItem is PickupItemCollectTheItem)
		{
			PickupItemCollectTheItem pickupItemCollectTheItem = currentItem as PickupItemCollectTheItem;
			if (controller.GetDoesWoFitDropOff(pickupItemCollectTheItem.GetCubeModelKeyId()))
			{
				MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, e.instigatorWOID);
				pickupItemCollectTheItem.SetShouldSpawnInstanceOnUnequip(shouldSpawnInstance: false);
			}
		}
	}

	private void SetLinks(bool isSet)
	{
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = isSet;
		}
	}

	public override void Destroy()
	{
		OnPickupCollected = null;
		if (cullingSubscriberBase != null)
		{
			PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Remove(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
		base.Destroy();
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		return editableCubeModelWrapper.OnExitObject(e);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		return editableCubeModelWrapper.OnEnterObject(e);
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		return worldObjectClientManager.GetWorldObjectClient(groupId)?.Delete(worldObjectClientManager, ref errorText) ?? false;
	}
}
