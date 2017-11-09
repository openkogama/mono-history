using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class CollectTheItemDropOff : MVBlueprintBase, ILogicWorldObject
{
	private const string isActiveKey = "isActive";

	private const string doOnce = "doOnce";

	private EditableCubeModelWrapper editableCubeModelWrapper;

	private CollectTheItemDropOffObject triggerObject;

	private CullingSubscriberBase cullingSubscriberBase;

	private CollectTheItem controller;

	private ObscuredIntVector minBounds = new ObscuredIntVector(-5, -4, -6);

	private ObscuredIntVector maxBounds = new ObscuredIntVector(7, 8, 6);

	private ObscuredInt minCubes = 10;

	private OutputSignalTransmitter outputSignalTransmitter;

	public Action<bool> OnPickupCollected;

	private bool sendSignal;

	public override bool HasOutputConnector => true;

	public override bool HasInputConnector => false;

	public override Vector3 OutputConnectorOffset => new Vector3(3.5f, 0f, 0f);

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public bool IsActive => (ObscuredBool)RunTimeData.GetObscuredType("isActive");

	public bool DoOnce => (bool)blueprintData["doOnce"];

	public CollectTheItemDropOff(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.CollectTheItemDropOffPrefab, worldObjects)
	{
		InteractionFlags |= InteractionFlags.CanEdit;
		InteractionFlags |= InteractionFlags.DirectlySelectable;
		InteractionFlags |= InteractionFlags.HasSettings;
		InteractionFlags |= InteractionFlags.CanResetLogic;
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
		OnPickupCollected = (Action<bool>)Delegate.Combine(OnPickupCollected, new Action<bool>(OnCollected));
		triggerObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		outputConnectorObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)GetChild("DropOffModel");
		mVCubeModelInstance.Visible = true;
		mVCubeModelInstance.Transform.SetParent(triggerObject.CullingObject.transform);
		editableCubeModelWrapper = new EditableCubeModelWrapper(mVCubeModelInstance, new IntVector(minBounds.x, minBounds.y, minBounds.z), new IntVector(maxBounds.x, maxBounds.y, maxBounds.z), minCubes);
		triggerObject.Blinker.MeshFilters = mVCubeModelInstance.GameObject.GetComponentsInChildren<MeshFilter>();
		triggerObject.Blinker.Visible = true;
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
			triggerObject.GreyOutScript.InitializeOriginalMaterials();
			editableCubeModelWrapper.CubeModel.BeingEditedChanged += OnChunkEditReset;
		}
		if (DoOnce)
		{
			OnPickupCollected(IsActive);
		}
		SetupCulling();
		InputSignalReceiver = LogicClientsideFactory.CreateInputSignalReceiver(this, defaultInput: false, SignalCallback);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
	}

	private void SignalCallback(bool b, bool wasHot, LogicObjectManager logicObjectManager)
	{
		outputSignalTransmitter.Send(sendSignal);
		sendSignal = false;
	}

	private void OnChunkEditReset(object sender, EditStateEventArgs args)
	{
		ReInitializeVisuals();
	}

	public void ReInitializeVisuals()
	{
		triggerObject.GreyOutScript.InitializeOriginalMaterials();
		triggerObject.Blinker.MeshFilters = editableCubeModelWrapper.CubeModel.GameObject.GetComponentsInChildren<MeshFilter>();
	}

	private void OnEditModeChange(EditModeChangeArgs arg)
	{
		triggerObject.EditCollider.enabled = true;
		if (arg.playInEditor)
		{
			triggerObject.EditCollider.enabled = false;
		}
	}

	private void OnCollected(bool shouldbeActiveOnCollect)
	{
		triggerObject.VisualObject.SetActive(shouldbeActiveOnCollect);
		triggerObject.Collider.enabled = shouldbeActiveOnCollect;
		triggerObject.Blinker.OnBlinkingActivated(shouldbeActiveOnCollect, BlinkType.DropOffCollectedItem);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit && !shouldbeActiveOnCollect)
		{
			triggerObject.GreyOutScript.GreyOut();
		}
	}

	public override void Reset()
	{
		base.Reset();
		triggerObject.VisualObject.SetActive(value: true);
		triggerObject.Collider.enabled = true;
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			triggerObject.GreyOutScript.GreyIn();
			if (OnPickupCollected != null)
			{
				OnPickupCollected(obj: true);
			}
			triggerObject.Blinker.DeactivateBlinking();
		}
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
		triggerObject.CullingObject.SetActive(active);
	}

	public override void OnDataUpdate()
	{
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	public void DropWoId(int instigatorWoID)
	{
		sendSignal = true;
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instigatorWoID);
		if (worldObjectClient == null)
		{
			return;
		}
		int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(instigatorWoID);
		if (woIDWithLocalOwnerHighestInHierarchy == -1)
		{
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate(PrefabPool.Instance.CollectTheItemParticles);
			particleSystem.transform.position = worldObjectClient.Transform.position;
			if (OnPickupCollected != null)
			{
				OnPickupCollected(!DoOnce);
			}
		}
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
		if (!(currentItem == null) && currentItem is PickupItemCollectTheItem)
		{
			mVEquipable.Unequip();
			PickupItemCollectTheItem pickupItemCollectTheItem = currentItem as PickupItemCollectTheItem;
			ParticleSystem particleSystem2 = UnityEngine.Object.Instantiate(PrefabPool.Instance.CollectTheItemParticles);
			particleSystem2.transform.position = pickupItemCollectTheItem.transform.position;
			if (OnPickupCollected != null)
			{
				OnPickupCollected(!DoOnce);
			}
		}
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
				pickupItemCollectTheItem.SetShouldSpawnInstanceOnUnequip(shouldSpawnInstance: false);
				MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, e.instigatorWOID);
			}
		}
	}

	public override void Destroy()
	{
		OnPickupCollected = null;
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
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
