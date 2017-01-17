using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class CollectTheItemCollectable : MVBlueprintBase
{
	private CullingSubscriberBase cullingSubscriberBase;

	private MVCubeModelInstance collectableModel;

	private CollectTheItemCollectableInstance collectableInstance;

	private EditableCubeModelWrapper editableCubeModelWrapper;

	private CollectTheItem controller;

	public Action OnCollectTheItemDestroyed;

	private ObscuredIntVector minBounds = new ObscuredIntVector(-3, 0, -3);

	private ObscuredIntVector maxBounds = new ObscuredIntVector(3, 6, 3);

	private ObscuredInt minCubes = 5;

	public int CollectableModelId
	{
		get
		{
			if (collectableModel == null)
			{
				controller = (CollectTheItem)MVGameControllerBase.WOCM.GetWorldObjectClient(GroupId);
				controller.SetupReferences();
			}
			return collectableModel.Id;
		}
	}

	public int DropOffId
	{
		get
		{
			if (controller == null)
			{
				controller = (CollectTheItem)MVGameControllerBase.WOCM.GetWorldObjectClient(GroupId);
				controller.SetupReferences();
			}
			return controller.DropOffId;
		}
	}

	public bool HasArrowIndicator => collectableInstance.HasArrowIndicator;

	public CollectTheItemCollectable(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		InteractionFlags |= InteractionFlags.CanEdit;
		InteractionFlags |= InteractionFlags.DirectlySelectable;
		interactionFlags |= InteractionFlags.HasSettings;
		InteractionFlags &= ~InteractionFlags.CanClone;
	}

	public void InitializeWithController(CollectTheItem controller)
	{
		this.controller = controller;
		SetupCollectableModel();
		controller.WoKeyInstance = collectableModel.Id;
		cullingSubscriberBase = new CullingSubscriberBase(2f, Transform.position, OnStateChanged);
		PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
	}

	private void SetupCollectableModel()
	{
		collectableInstance = (CollectTheItemCollectableInstance)GetChild("CollectableInstance");
		collectableModel = (MVCubeModelInstance)collectableInstance.GetChild("CollectableModel");
		collectableModel.Visible = true;
		collectableModel.GameObject.SetLayerRecursively(LayerMask.NameToLayer("Player"));
		collectableModel.Transform.rotation = Quaternion.identity;
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			CollectTheItemCollectableInstance collectTheItemCollectableInstance = collectableInstance;
			collectTheItemCollectableInstance.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(collectTheItemCollectableInstance.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
			editableCubeModelWrapper = new EditableCubeModelWrapper(collectableModel, new IntVector(minBounds.x, minBounds.y, minBounds.z), new IntVector(maxBounds.x, maxBounds.y, maxBounds.z), minCubes);
			editableCubeModelWrapper.CubeModel.BeingEditedChanged += collectableInstance.SetupGreyoutScript;
		}
	}

	private void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = positionChangedEventArgs.NewPos;
	}

	public Dictionary<string, object> GetItemData()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["cubeModelId"] = collectableModel.Id;
		dictionary["dropOffId"] = controller.DropOffId;
		dictionary["spawnerId"] = Id;
		dictionary["isOriginal"] = true;
		return dictionary;
	}

	public void CreateCollectableInstance(Vector3 position, Quaternion rotation)
	{
		MVGameControllerBase.OperationRequests.CloneTempWorldObjectWithOriginalReference(collectableInstance, position, rotation);
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		collectableInstance.SetRotationEnabled(enableRotation: true);
		return editableCubeModelWrapper.OnExitObject(e);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		collectableInstance.SetRotationEnabled(enableRotation: false);
		collectableModel.Transform.rotation = Quaternion.identity;
		return editableCubeModelWrapper.OnEnterObject(e);
	}

	public void OnStateChanged(CullingGroupEvent cullingEvent)
	{
		bool active = CullingApiWrapper.Visible(cullingEvent, cullingSubscriberBase.DistanceBandIndex);
		GameObject.SetActive(active);
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		return worldObjectClientManager.GetWorldObjectClient(groupId)?.Delete(worldObjectClientManager, ref errorText) ?? false;
	}

	public override void Destroy()
	{
		if (cullingSubscriberBase != null)
		{
			PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Remove(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
		if (OnCollectTheItemDestroyed != null)
		{
			OnCollectTheItemDestroyed();
			OnCollectTheItemDestroyed = null;
		}
		base.Destroy();
	}
}
