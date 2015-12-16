using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class MVWorldObjectClient : MVWorldObject
{
	public new delegate void CallBackDelegate(MVWorldObjectClient woc);

	protected bool isCastingShadows;

	protected static int woShadowCastersCount;

	protected static int woMaxShadowCasters = 20;

	private int goId;

	protected string name;

	protected GameObject gameObject;

	protected Transform transform;

	protected MVNetworkObject networkObject;

	private bool reactsToLODChanges = true;

	private MVGroup group;

	private bool selected;

	protected SelectedConnector selectedConnector;

	protected GameObject inputConnectorObject;

	protected GameObject outputConnectorObject;

	protected GameObject objectConnectorObject;

	protected InteractionFlags interactionFlags;

	protected LayerFlags previewLayerMask = LayerFlags.Default;

	private MVRuntimeDataVariables runtimeDataVariables;

	public override Vector3 Position
	{
		get
		{
			return transform.localPosition;
		}
		set
		{
			Vector3 vector = transform.position;
			transform.localPosition = value;
			Vector3 vector2 = transform.position;
			if (vector != vector2 && PositionChanged != null)
			{
				PositionChanged(this, new PositionChangedEventArgs(vector, vector2));
			}
		}
	}

	public override Quaternion Rotation
	{
		get
		{
			return transform.localRotation;
		}
		set
		{
			Quaternion quaternion = transform.rotation;
			transform.localRotation = value;
			Quaternion quaternion2 = transform.rotation;
			if (quaternion != quaternion2 && RotationChanged != null)
			{
				RotationChanged(this, new RotationChangedEventArgs(quaternion, quaternion2));
			}
		}
	}

	public Vector3 EulerAngles
	{
		get
		{
			return transform.localEulerAngles;
		}
		set
		{
			Quaternion quaternion = transform.rotation;
			transform.localEulerAngles = value;
			Quaternion quaternion2 = transform.rotation;
			if (quaternion != quaternion2 && RotationChanged != null)
			{
				RotationChanged(this, new RotationChangedEventArgs(quaternion, quaternion2));
			}
		}
	}

	public override Vector3 Scale
	{
		get
		{
			return transform.localScale;
		}
		set
		{
			Vector3 localScale = transform.localScale;
			transform.localScale = value;
			if (localScale != value && ScaleChanged != null)
			{
				ScaleChanged(this, new ScaleChangedEventArgs(localScale, value));
			}
		}
	}

	public new Vector3 WorldPosition
	{
		get
		{
			return transform.position;
		}
		set
		{
			Vector3 vector = transform.position;
			transform.position = value;
			if (vector != value && PositionChanged != null)
			{
				PositionChanged(this, new PositionChangedEventArgs(vector, value));
			}
		}
	}

	public new Quaternion WorldRotation
	{
		get
		{
			return gameObject.transform.rotation;
		}
		set
		{
			Quaternion quaternion = transform.rotation;
			transform.rotation = value;
			if (quaternion != value && RotationChanged != null)
			{
				RotationChanged(this, new RotationChangedEventArgs(quaternion, value));
			}
		}
	}

	public Vector3 WorldEulerAngles
	{
		get
		{
			return transform.eulerAngles;
		}
		set
		{
			Quaternion quaternion = transform.rotation;
			transform.eulerAngles = value;
			Quaternion quaternion2 = transform.rotation;
			if (quaternion != quaternion2 && RotationChanged != null)
			{
				RotationChanged(this, new RotationChangedEventArgs(quaternion, quaternion2));
			}
		}
	}

	public virtual Vector3 SyncPos
	{
		get
		{
			return WorldPosition;
		}
		set
		{
			WorldPosition = value;
		}
	}

	public virtual Quaternion SyncRot
	{
		get
		{
			return WorldRotation;
		}
		set
		{
			WorldRotation = value;
		}
	}

	public bool ReactsToLODChanges
	{
		get
		{
			return reactsToLODChanges;
		}
		set
		{
			reactsToLODChanges = value;
		}
	}

	public MVGroup Group
	{
		get
		{
			return group;
		}
		set
		{
			group = value;
		}
	}

	public bool Selected
	{
		get
		{
			return selected;
		}
		protected set
		{
			if (selected != value)
			{
				selected = value;
				OnSelectedChanged(value);
			}
		}
	}

	public HashSet<int> WorldIDsRecursive
	{
		get
		{
			HashSet<int> childIDs = new HashSet<int>();
			CallBackDelegate callBack = (MVWorldObjectClient wo) =>
			{
				childIDs.Add(wo.Id);
			};
			TraverseRecursiveTail(callBack);
			return childIDs;
		}
	}

	public virtual Vector3 WorldPivot => SharedCubeFunctions.GetWorldCenter(transform);

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public override bool HasObjectConnector => false;

	public InteractionFlags InteractionFlags
	{
		get
		{
			return interactionFlags;
		}
		set
		{
			interactionFlags = value;
		}
	}

	public LayerFlags PreviewLayerMask => previewLayerMask;

	public PlayInteractionType PlayInteractionType { get; set; }

	public int GameObjectID => goId;

	public GameObject GameObject => gameObject;

	public Transform Transform => transform;

	public MVNetworkObject NetworkObject
	{
		get
		{
			return networkObject;
		}
		set
		{
			networkObject = value;
		}
	}

	public SelectedConnector SelectedConnector => selectedConnector;

	public virtual Vector3 InputConnectorOffset => new Vector3(-1f, 0f, 0f);

	public virtual Vector3 OutputConnectorOffset => new Vector3(1f, 0f, 0f);

	public virtual Vector3 ObjectConnectorOffset => new Vector3(0f, 0f, -1f);

	public virtual Quaternion ObjectConnectorRotation => Quaternion.identity;

	public MVRuntimeDataVariables RuntimeDataVariables => runtimeDataVariables;

	public override Dictionary<object, object> RunTimeData
	{
		get
		{
			return base.RunTimeData;
		}
		set
		{
			base.RunTimeData = (Dictionary<object, object>)ObscuredTypesConverter.CreateObscuredValue(value);
		}
	}

	public virtual bool Visible
	{
		get
		{
			MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer.enabled)
				{
					return true;
				}
			}
			return false;
		}
		set
		{
			MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
			if (componentsInChildren.Length > 0)
			{
				MeshRenderer[] array = componentsInChildren;
				foreach (MeshRenderer meshRenderer in array)
				{
					meshRenderer.enabled = value;
				}
			}
			else
			{
				Debug.LogWarning("MeshRenderer(s) not found on attempt to set visibility");
			}
		}
	}

	public event EventHandler<PositionChangedEventArgs> PositionChanged;

	public event EventHandler<RotationChangedEventArgs> RotationChanged;

	public event EventHandler<ScaleChangedEventArgs> ScaleChanged;

	public event EventHandler<SelectedEventArgs> SelectedChanged;

	public event EventHandler ObjectDestroyed;

	public MVWorldObjectClient(Dictionary<object, object> data, GameObject prefabObject, Dictionary<int, MVWorldObjectClient> worldObjects)
	{
		gameObject = InstantiatePrefab(prefabObject);
		transform = gameObject.transform;
		CreateWorldObject(data, worldObjects);
	}

	public MVWorldObjectClient(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
	{
		gameObject = new GameObject();
		goId = gameObject.GetInstanceID();
		transform = gameObject.transform;
		CreateWorldObject(data, worldObjects);
	}

	protected GameObject InstantiatePrefab(GameObject prefabObject)
	{
		if (prefabObject == null)
		{
			Debug.LogError("Prefab object is null.");
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(prefabObject);
		goId = gameObject.GetInstanceID();
		return gameObject;
	}

	public bool HasInteractionFlag(InteractionFlags flag)
	{
		return (interactionFlags & flag) == flag;
	}

	private void SetupBusinessLogic()
	{
		if (MVGameControllerBase.Game.ItemBusinessLogic.CanAddItemToInventory(itemId))
		{
			interactionFlags |= InteractionFlags.CanAddToInventory;
		}
	}

	private void ApplyData(Dictionary<object, object> data)
	{
		id = (int)data[WorldObjectDataParameters.Id];
		groupId = (int)data[WorldObjectDataParameters.GroudId];
		itemId = (int)data[WorldObjectDataParameters.ItemId];
		WorldObjectType = (WorldObjectType)(int)data[WorldObjectDataParameters.WorldObjectType];
		Vector3 vector = (Vector3)data[WorldObjectDataParameters.Position];
		if (MathFunctions.VectorIsNan(vector))
		{
			Debug.LogError("Nan position detected");
			vector = Vector3.zero;
		}
		Position = vector;
		Quaternion quaternion = (Quaternion)data[WorldObjectDataParameters.Rotation];
		if (MathFunctions.QuaternionIsNan(quaternion))
		{
			Debug.LogError("Nan rotation detected");
			quaternion = Quaternion.identity;
		}
		Rotation = quaternion;
		Vector3 vector2 = (Vector3)data[WorldObjectDataParameters.Scale];
		if (MathFunctions.VectorIsNan(vector2))
		{
			Debug.LogError("Nan scale detected");
			vector2 = Vector3.one;
		}
		Scale = vector2;
		Data = (Dictionary<object, object>)data[WorldObjectDataParameters.Data];
		if (data.ContainsKey(WorldObjectDataParameters.RuntimeData))
		{
			RunTimeData = (Dictionary<object, object>)data[WorldObjectDataParameters.RuntimeData];
		}
		else
		{
			RunTimeData = RuntimeVariablesRepository.GetRuntimeVariables(WorldObjectType);
		}
		if (data.ContainsKey(WorldObjectDataParameters.OwnerActorNumber))
		{
			OwnerActorNr = (int)data[WorldObjectDataParameters.OwnerActorNumber];
		}
		if (data.ContainsKey(WorldObjectDataParameters.PreviewOwnerProfileId))
		{
			PreviewOwnerProfileId = (int)data[WorldObjectDataParameters.PreviewOwnerProfileId];
		}
	}

	private void CreateWorldObject(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
	{
		CreateConnectors();
		ApplyData(data);
		SetName();
		runtimeDataVariables = new MVRuntimeDataVariables(this);
		if (groupId != -1)
		{
			group = (MVGroup)worldObjects[groupId];
			group.AddChild(this);
		}
		SetupBusinessLogic();
	}

	public MVWorldObjectClient GetHitInteractionHandlingWO()
	{
		switch (PlayInteractionType)
		{
		case PlayInteractionType.Solid:
		case PlayInteractionType.ExcludeFromInteraction:
			return null;
		case PlayInteractionType.HandlesHits:
			return this;
		case PlayInteractionType.ParentHandlesHits:
			if (GroupId != -1)
			{
				MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(GroupId);
				return worldObjectClient.GetHitInteractionHandlingWO();
			}
			Debug.LogWarning("WorldObject has ParentHandlesHits, but no parent group!", gameObject);
			return null;
		default:
			return null;
		}
	}

	public virtual void TraverseRecursiveTail(CallBackDelegate callBack)
	{
		callBack(this);
	}

	public virtual bool CompareWithKoGaMaPackage(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, ref int insertedByProfileId)
	{
		return false;
	}

	public virtual void Compare(MVWorldObjectClient wo, bool visibleCubesOnly, ref int matchingCubeCount, ref int investigatedCubeCount)
	{
	}

	public virtual MVWorldObjectClient Clone(int ownerActorNumber, int cloneGroupId, CloneBookkeeping cloneBookkeeping, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
	{
		base.RunTimeData = (Dictionary<object, object>)ObscuredTypesConverter.CreateUnObscuredValue(RunTimeData);
		Dictionary<object, object> dictionary = DeepCopyWorldObjectDataParameters();
		RunTimeData = base.RunTimeData;
		dictionary[WorldObjectDataParameters.Id] = cloneBookkeeping.cloneIdIncrement;
		dictionary[WorldObjectDataParameters.GroudId] = cloneGroupId;
		dictionary[WorldObjectDataParameters.OwnerActorNumber] = ownerActorNumber;
		dictionary[WorldObjectDataParameters.ItemId] = itemId;
		dictionary[WorldObjectDataParameters.PreviewOwnerProfileId] = PreviewOwnerProfileId;
		MVWorldObjectClient mVWorldObjectClient = KoGaMaPackageClient.WorldObjectFactory(dictionary, worldObjects, prototypes);
		cloneBookkeeping.worldObjectIdsMaps.Add(id, mVWorldObjectClient.id);
		mVWorldObjectClient.SetNetworkObject(MVGameControllerBase.Game.LocalPlayerActorNumber == ownerActorNumber);
		mVWorldObjectClient.State = MVWorldObjectState.Synced;
		MVGameControllerBase.Game.AddCloneToWorldObjects(mVWorldObjectClient);
		GetLinksForClone(cloneBookkeeping.linkIds);
		GetObjectLinksForClone(cloneBookkeeping.objectLinkIds);
		cloneBookkeeping.cloneIdIncrement++;
		return mVWorldObjectClient;
	}

	public virtual void SetWorldObjectToPurchased()
	{
		PreviewOwnerProfileId = 0;
		interactionFlags &= ~InteractionFlags.IsPreview;
		RemovePreviewBox();
	}

	public void SetNetworkObject(bool local)
	{
		if (GetType() != typeof(MVCubeModelFineGrainedTerrain) && GetType() != typeof(MVCubeModelPrototypeTerrain))
		{
			if (local)
			{
				networkObject = new MVNetworkReporter(this);
			}
			else
			{
				networkObject = new MVNetworkListener(this);
			}
		}
	}

	public virtual void Initialize()
	{
		if (PreviewOwnerProfileId != 0)
		{
			AddPreviewBox();
			interactionFlags |= InteractionFlags.IsPreview;
		}
	}

	public virtual void InitializeInventory()
	{
	}

	public virtual void PlayModeInitialize()
	{
	}

	public virtual void Destroy()
	{
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		if (ObjectDestroyed != null)
		{
			ObjectDestroyed(this, new EventArgs());
		}
	}

	public virtual void OnDataUpdate()
	{
	}

	public virtual void OnRunTimeDataUpdate()
	{
	}

	public virtual bool OnEnterObject(EditorStateMachine e)
	{
		return false;
	}

	public virtual bool OnExitObject(EditorStateMachine e)
	{
		e.Event = EditorEvent.ObjectSelected;
		return false;
	}

	protected virtual void OnSelectedChanged(bool selected)
	{
		if (SelectedChanged != null)
		{
			SelectedChanged(this, new SelectedEventArgs(selected));
		}
	}

	public virtual bool ValidateObjectLinkTarget(MVWorldObjectClient wo)
	{
		return false;
	}

	public void SendPackage(Dictionary<object, object> package)
	{
		MVGameControllerBase.Game.WorldObjectRPC(id, package);
	}

	public virtual void ReceivePackage(MVPlayer p, Dictionary<object, object> package)
	{
		foreach (KeyValuePair<object, object> item in package)
		{
			if ((byte)item.Key == 0)
			{
				ReceiveInteractionPackage(new InteractionData((byte[])item.Value), p);
			}
			else
			{
				Debug.LogError("Unknown package type");
			}
		}
	}

	public virtual void ReceiveInteractionPackage(InteractionData interactionStruct, MVPlayer p)
	{
		AvatarPackages.packages[interactionStruct.InteractionType].ParseAndHandlePackage(this, p, interactionStruct);
	}

	private void CreateConnectors()
	{
		if (HasInputConnector)
		{
			inputConnectorObject = UnityEngine.Object.Instantiate(PrefabPool.Instance.LogicInputConnectorPrefab, gameObject.transform.position + InputConnectorOffset, Quaternion.identity) as GameObject;
			inputConnectorObject.transform.parent = gameObject.transform;
		}
		if (HasOutputConnector)
		{
			outputConnectorObject = UnityEngine.Object.Instantiate(PrefabPool.Instance.LogicOutputConnectorPrefab, gameObject.transform.position + OutputConnectorOffset, Quaternion.identity) as GameObject;
			outputConnectorObject.transform.parent = gameObject.transform;
		}
		if (HasObjectConnector)
		{
			objectConnectorObject = UnityEngine.Object.Instantiate(PrefabPool.Instance.LogicObjectConnectorPrefab, gameObject.transform.position + ObjectConnectorOffset, ObjectConnectorRotation) as GameObject;
			objectConnectorObject.transform.parent = gameObject.transform;
		}
	}

	public void RuntimeDataUpdate(Dictionary<object, object> dataDelta)
	{
		RuntimeDataVariables.Receive(dataDelta);
		OnRunTimeDataUpdate();
	}

	public override void PartialUpdateWOData(Dictionary<object, object> woData)
	{
		base.PartialUpdateWOData(woData);
		OnDataUpdate();
	}

	public override void PartialRemoveFromWOData(Dictionary<object, object> entriesToRemove)
	{
		base.PartialRemoveFromWOData(entriesToRemove);
		OnDataUpdate();
	}

	public virtual void HandleInput(NetworkInputActionCodes actionCode, NetworkInputKeyCodes keyCode)
	{
	}

	public virtual void ChangeLOD(float distance)
	{
	}

	public virtual Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, gameObject.transform.localScale);
	}

	public virtual bool OnClickHandler(EditorStateMachine esm, Collider collider)
	{
		if (HasInputConnector && collider == inputConnectorObject.GetComponentInChildren<Collider>())
		{
			selectedConnector = SelectedConnector.Input;
			esm.PushState(EditorEvent.ESAddLink);
			return true;
		}
		if (HasOutputConnector && collider == outputConnectorObject.GetComponentInChildren<Collider>())
		{
			selectedConnector = SelectedConnector.Output;
			esm.PushState(EditorEvent.ESAddLink);
			return true;
		}
		if (HasObjectConnector && collider == objectConnectorObject.GetComponentInChildren<Collider>())
		{
			selectedConnector = SelectedConnector.Object;
			esm.PushState(EditorEvent.ESAddObjectLink);
			return true;
		}
		return false;
	}

	public Vector3 GetInputConnectorPos()
	{
		if (!HasInputConnector)
		{
			return gameObject.transform.position;
		}
		return inputConnectorObject.transform.position;
	}

	public Vector3 GetOutputConnectorPos()
	{
		if (!HasOutputConnector)
		{
			return gameObject.transform.position;
		}
		return outputConnectorObject.transform.position;
	}

	public Vector3 GetObjectConnectorPos()
	{
		if (!HasObjectConnector)
		{
			return gameObject.transform.position;
		}
		return objectConnectorObject.transform.position;
	}

	public bool IsPointOverInputConnector(Vector3 mousePoint)
	{
		if (!HasInputConnector)
		{
			return false;
		}
		return DoesScreenPointHitCollider(mousePoint, inputConnectorObject.GetComponentInChildren<Collider>());
	}

	public bool IsPointOverOutputConnector(Vector3 mousePoint)
	{
		if (!HasOutputConnector)
		{
			return false;
		}
		return DoesScreenPointHitCollider(mousePoint, outputConnectorObject.GetComponentInChildren<Collider>());
	}

	public virtual void HighlightConnector(bool state)
	{
		if (selectedConnector == SelectedConnector.Input)
		{
			Renderer componentInChildren = inputConnectorObject.GetComponentInChildren<Collider>().GetComponentInChildren<Renderer>();
			if (!(componentInChildren == null))
			{
				componentInChildren.material = (state ? ((Material)Resources.Load("Materials/LogicCubeConnectorRedSelected")) : ((Material)Resources.Load("Materials/LogicCubeConnectorRed")));
			}
		}
		else if (selectedConnector == SelectedConnector.Output)
		{
			Renderer componentInChildren2 = outputConnectorObject.GetComponentInChildren<Collider>().GetComponentInChildren<Renderer>();
			if (!(componentInChildren2 == null))
			{
				componentInChildren2.material = (state ? ((Material)Resources.Load("Materials/LogicCubeConnectorBlueSelected")) : ((Material)Resources.Load("Materials/LogicCubeConnectorBlue")));
			}
		}
	}

	private bool DoesScreenPointHitCollider(Vector3 point, Collider collider)
	{
		Ray ray = MVGameControllerBase.CameraController.MainCamera.ScreenPointToRay(point);
		RaycastHit[] array = Physics.RaycastAll(ray);
		if (array.Length > 0)
		{
			RaycastHit[] array2 = array;
			foreach (RaycastHit raycastHit in array2)
			{
				if (raycastHit.collider == collider)
				{
					return true;
				}
			}
		}
		return false;
	}

	public virtual Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		Debug.LogWarning($"GetLocalBounds has not been implemented for {GetType().Name}.");
		return new Bounds(Vector3.zero, Vector3.zero);
	}

	public Vector3[] GetBoundsCornersLocal(BoundsContext boundsContext)
	{
		return SharedCubeFunctions.GetCorners(GetLocalBounds(boundsContext));
	}

	public Vector3[] GetBoundsCornersWorld(BoundsContext boundsContext)
	{
		Matrix4x4 localToWorld = transform.localToWorldMatrix;
		return (from localCorner in GetBoundsCornersLocal(boundsContext)
			select localToWorld.MultiplyPoint(localCorner)).ToArray();
	}

	public virtual void Select()
	{
		Selected = true;
	}

	public virtual void Select(Color color)
	{
		AddSelectionBox();
		Selected = true;
	}

	public virtual void DeSelect()
	{
		selectedConnector = SelectedConnector.None;
		RemoveSelectionBox();
		Selected = false;
	}

	public virtual void CheckCanInsert(MVGUIInventoryGroup.CanInsertDelegate canInsert)
	{
		canInsert?.Invoke(canInsert: true);
	}

	public virtual void AddPreviewBox()
	{
		PreviewBox previewBox = this.gameObject.GetComponentInChildren<PreviewBox>();
		if (previewBox == null)
		{
			GameObject gameObject = CreateBox("PreviewBox", 1.005f);
			previewBox = gameObject.AddComponent<PreviewBox>();
		}
		previewBox.Show("Materials/PreviewBoxMaterial", GetBoundsCornersLocal(BoundsContext.BoxVisualization));
	}

	public virtual void AddSelectionBox()
	{
		SelectionBox selectionBox = this.gameObject.GetComponentInChildren<SelectionBox>();
		if (selectionBox == null)
		{
			GameObject gameObject = CreateBox("SelectionBox", 1.001f);
			selectionBox = gameObject.AddComponent<SelectionBox>();
		}
		selectionBox.FadeIn(0.2f, "Materials/SelectBoxMaterial", GetBoundsCornersLocal(BoundsContext.BoxVisualization));
	}

	protected GameObject CreateBox(string name, float scale)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.parent = this.gameObject.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one * scale;
		return gameObject;
	}

	public virtual void RemoveSelectionBox()
	{
		if (!(gameObject == null))
		{
			SelectionBox componentInChildren = gameObject.GetComponentInChildren<SelectionBox>();
			if (componentInChildren != null)
			{
				componentInChildren.FadeOutDestroy(0.8f);
			}
		}
	}

	public virtual void RemovePreviewBox()
	{
		if (!(gameObject == null))
		{
			PreviewBox componentInChildren = gameObject.GetComponentInChildren<PreviewBox>();
			if (componentInChildren != null)
			{
				componentInChildren.DestroyBox();
			}
		}
	}

	public virtual bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		gameObject.SetActive(value: false);
		worldObjectClientManager.UnregisterWorldObject(id);
		return true;
	}

	public virtual void DeleteFailed()
	{
		if (gameObject != null)
		{
			gameObject.SetActive(value: true);
		}
	}

	public void ClearTransformQueue()
	{
		if (NetworkObject != null && NetworkObject.GetType() == typeof(MVNetworkListener))
		{
			(NetworkObject as MVNetworkListener).ClearTransformQueue();
		}
	}

	public void SetName()
	{
		gameObject.name = ToString();
	}

	public override string ToString()
	{
		return GetType().ToString() + " id " + id + " group id " + groupId + " item id " + itemId;
	}

	public virtual Vector3 GetTargetPosition()
	{
		Collider componentInChildren = gameObject.GetComponentInChildren<Collider>();
		if (componentInChildren == null)
		{
			Debug.LogWarning("Did not find collider. Using transform.Position for " + this);
			return transform.position;
		}
		return componentInChildren.bounds.center;
	}

	public float ComputeObjectRadius()
	{
		return (GetLocalBounds(BoundsContext.Default).size.Multiply(Scale) * 0.5f).magnitude;
	}

	public float ComputeObjectSqrRadius()
	{
		return (GetLocalBounds(BoundsContext.Default).size.Multiply(Scale) * 0.5f).sqrMagnitude;
	}

	public void RotateAround(Vector3 pivot, Vector3 axis, float angle)
	{
		Quaternion quaternion = transform.rotation;
		transform.RotateAround(pivot, axis, angle);
		Quaternion quaternion2 = transform.rotation;
		if (quaternion != quaternion2 && RotationChanged != null)
		{
			RotationChanged(this, new RotationChangedEventArgs(quaternion, quaternion2));
		}
	}
}
