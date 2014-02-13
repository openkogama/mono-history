using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Localize;
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
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return transform.localPosition;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = transform.position;
			transform.localPosition = value;
			Vector3 val2 = transform.position;
			if (val != val2 && PositionChanged != null)
			{
				PositionChanged(this, new PositionChangedEventArgs(val, val2));
			}
		}
	}

	public override Quaternion Rotation
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return transform.localRotation;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = transform.rotation;
			transform.localRotation = value;
			Quaternion val2 = transform.rotation;
			if (val != val2 && RotationChanged != null)
			{
				RotationChanged(this, new RotationChangedEventArgs(val, val2));
			}
		}
	}

	public Vector3 EulerAngles
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return transform.localEulerAngles;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = transform.rotation;
			transform.localEulerAngles = value;
			Quaternion val2 = transform.rotation;
			if (val != val2 && RotationChanged != null)
			{
				RotationChanged(this, new RotationChangedEventArgs(val, val2));
			}
		}
	}

	public override Vector3 Scale
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return transform.localScale;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return transform.position;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = transform.position;
			transform.position = value;
			if (val != value && PositionChanged != null)
			{
				PositionChanged(this, new PositionChangedEventArgs(val, value));
			}
		}
	}

	public new Quaternion WorldRotation
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return gameObject.transform.rotation;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = transform.rotation;
			transform.rotation = value;
			if (val != value && RotationChanged != null)
			{
				RotationChanged(this, new RotationChangedEventArgs(val, value));
			}
		}
	}

	public Vector3 WorldEulerAngles
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return transform.eulerAngles;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = transform.rotation;
			transform.eulerAngles = value;
			Quaternion val2 = transform.rotation;
			if (val != val2 && RotationChanged != null)
			{
				RotationChanged(this, new RotationChangedEventArgs(val, val2));
			}
		}
	}

	public virtual Vector3 SyncPos
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return WorldPosition;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			WorldPosition = value;
			State = MVWorldObjectState.Dirty;
		}
	}

	public virtual Quaternion SyncRot
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return WorldRotation;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			WorldRotation = value;
			State = MVWorldObjectState.Dirty;
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

	public virtual Vector3 WorldPivot
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return SharedCubeFunctions.GetWorldCenter(transform);
		}
	}

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

	public virtual Vector3 InputConnectorOffset
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(-1f, 0f, 0f);
		}
	}

	public virtual Vector3 OutputConnectorOffset
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(1f, 0f, 0f);
		}
	}

	public virtual Vector3 ObjectConnectorOffset
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(0f, 0f, -1f);
		}
	}

	public virtual Quaternion ObjectConnectorRotation
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Quaternion.identity;
		}
	}

	public MVRuntimeDataVariables RuntimeDataVariables => runtimeDataVariables;

	public bool Visible
	{
		get
		{
			MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer val in array)
			{
				if (((Renderer)val).enabled)
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
				foreach (MeshRenderer val in array)
				{
					((Renderer)val).enabled = value;
				}
			}
			else
			{
				Debug.LogWarning((object)"MeshRenderer(s) not found on attempt to set visibility");
			}
		}
	}

	public event EventHandler<PositionChangedEventArgs> PositionChanged;

	public event EventHandler<RotationChangedEventArgs> RotationChanged;

	public event EventHandler<ScaleChangedEventArgs> ScaleChanged;

	public event EventHandler<SelectedEventArgs> SelectedChanged;

	public event EventHandler ObjectDestroyed;

	public MVWorldObjectClient(Hashtable data, string prefabPath, Dictionary<int, MVWorldObjectClient> worldObjects)
	{
		gameObject = LoadPrefab(prefabPath);
		transform = gameObject.transform;
		CreateWorldObject(data, worldObjects);
	}

	public MVWorldObjectClient(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected Obj, but got Unknown
		gameObject = new GameObject();
		goId = ((Object)gameObject).GetInstanceID();
		transform = gameObject.transform;
		CreateWorldObject(data, worldObjects);
	}

	protected GameObject LoadPrefab(string prefabPath)
	{
		Object val = Resources.Load(prefabPath);
		if (val == (Object)null)
		{
			Debug.LogError((object)("Could not find prefab: " + prefabPath));
		}
		Object val2 = Object.Instantiate(val);
		GameObject val3 = (GameObject)(object)((val2 is GameObject) ? val2 : null);
		goId = ((Object)val3).GetInstanceID();
		return val3;
	}

	public bool HasInteractionFlag(InteractionFlags flag)
	{
		return (interactionFlags & flag) == flag;
	}

	private void SetupBusinessLogic()
	{
		if (MVGameController.Instance.Game.ItemBusinessLogic.CanAddItemToInventory(itemId))
		{
			interactionFlags |= InteractionFlags.CanAddToInventory;
		}
	}

	private void ApplyData(Hashtable data)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		id = (int)data[WorldObjectDataParameters.Id];
		groupId = (int)data[WorldObjectDataParameters.GroudId];
		itemId = (int)data[WorldObjectDataParameters.ItemId];
		WorldObjectType = (WorldObjectType)(int)data[WorldObjectDataParameters.WorldObjectType];
		Position = (Vector3)data[WorldObjectDataParameters.Position];
		Rotation = (Quaternion)data[WorldObjectDataParameters.Rotation];
		Scale = (Vector3)data[WorldObjectDataParameters.Scale];
		Data = (Hashtable)data[WorldObjectDataParameters.Data];
		if (data.ContainsKey(WorldObjectDataParameters.RuntimeData))
		{
			RunTimeData = (Hashtable)data[WorldObjectDataParameters.RuntimeData];
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

	private void CreateWorldObject(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
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
				MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(GroupId);
				return worldObjectClient.GetHitInteractionHandlingWO();
			}
			Debug.LogWarning((object)"WorldObject has ParentHandlesHits, but no parent group!", (Object)(object)gameObject);
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
		Hashtable hashtable = DeepCopyWorldObjectDataParameters();
		hashtable[WorldObjectDataParameters.Id] = cloneBookkeeping.cloneIdIncrement;
		hashtable[WorldObjectDataParameters.GroudId] = cloneGroupId;
		hashtable[WorldObjectDataParameters.OwnerActorNumber] = ownerActorNumber;
		hashtable[WorldObjectDataParameters.ItemId] = itemId;
		hashtable[WorldObjectDataParameters.PreviewOwnerProfileId] = PreviewOwnerProfileId;
		MVWorldObjectClient mVWorldObjectClient = KoGaMaPackageClient.WorldObjectFactory(hashtable, worldObjects, prototypes);
		cloneBookkeeping.worldObjectIdsMaps.Add(id, mVWorldObjectClient.id);
		mVWorldObjectClient.SetNetworkObject(MVGameController.Instance.Game.LocalPlayerActorNumber == ownerActorNumber);
		mVWorldObjectClient.State = MVWorldObjectState.Synced;
		MVGameController.Instance.Game.AddCloneToWorldObjects(mVWorldObjectClient);
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
		if ((object)GetType() != typeof(MVCubeModelFineGrainedTerrain) && (object)GetType() != typeof(MVCubeModelPrototypeTerrain))
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
		if ((Object)(object)gameObject != (Object)null)
		{
			Object.Destroy((Object)(object)gameObject);
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

	public void SendPackage(Hashtable package)
	{
		MVGameController.Instance.Game.WorldObjectRPC(id, package);
	}

	public virtual void ReceivePackage(MVPlayer p, Hashtable package)
	{
		foreach (DictionaryEntry item in package)
		{
			if ((byte)item.Key == 0)
			{
				ReceiveInteractionPackage(new InteractionData((byte[])item.Value, withSharedValues: true), p);
			}
			else
			{
				Debug.LogError((object)"Unknown package type");
			}
		}
	}

	public virtual void ReceiveInteractionPackage(InteractionData interactionStruct, MVPlayer p)
	{
		AvatarPackages.packages[interactionStruct.InteractionType].ParseAndHandlePackage(this, p, interactionStruct);
	}

	private void CreateConnectors()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected Obj, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected Obj, but got Unknown
		if (HasInputConnector)
		{
			inputConnectorObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/InputConnectorObject"), gameObject.transform.position + InputConnectorOffset, Quaternion.identity);
			inputConnectorObject.transform.parent = gameObject.transform;
		}
		if (HasOutputConnector)
		{
			outputConnectorObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/OutputConnectorObject"), gameObject.transform.position + OutputConnectorOffset, Quaternion.identity);
			outputConnectorObject.transform.parent = gameObject.transform;
		}
		if (HasObjectConnector)
		{
			objectConnectorObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ObjectConnectorObject"), gameObject.transform.position + ObjectConnectorOffset, ObjectConnectorRotation);
			objectConnectorObject.transform.parent = gameObject.transform;
		}
	}

	public void RuntimeDataUpdate(Hashtable dataDelta)
	{
		RuntimeDataVariables.Receive(dataDelta);
		OnRunTimeDataUpdate();
	}

	public override void PartialUpdateWOData(Hashtable woData)
	{
		base.PartialUpdateWOData(woData);
		OnDataUpdate();
	}

	public override void PartialRemoveFromWOData(Hashtable entriesToRemove)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, gameObject.transform.localScale);
	}

	public virtual bool OnClickHandler(EditorStateMachine esm, Collider collider)
	{
		if (HasInputConnector && (Object)(object)collider == (Object)(object)inputConnectorObject.GetComponentInChildren<Collider>())
		{
			selectedConnector = SelectedConnector.Input;
			esm.PushState(EditorEvent.ESAddLink);
			return true;
		}
		if (HasOutputConnector && (Object)(object)collider == (Object)(object)outputConnectorObject.GetComponentInChildren<Collider>())
		{
			selectedConnector = SelectedConnector.Output;
			esm.PushState(EditorEvent.ESAddLink);
			return true;
		}
		if (HasObjectConnector && (Object)(object)collider == (Object)(object)objectConnectorObject.GetComponentInChildren<Collider>())
		{
			selectedConnector = SelectedConnector.Object;
			esm.PushState(EditorEvent.ESAddObjectLink);
			return true;
		}
		return false;
	}

	public Vector3 GetInputConnectorPos()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!HasInputConnector)
		{
			return gameObject.transform.position;
		}
		return inputConnectorObject.transform.position;
	}

	public Vector3 GetOutputConnectorPos()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!HasOutputConnector)
		{
			return gameObject.transform.position;
		}
		return outputConnectorObject.transform.position;
	}

	public Vector3 GetObjectConnectorPos()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!HasObjectConnector)
		{
			return gameObject.transform.position;
		}
		return objectConnectorObject.transform.position;
	}

	public bool IsPointOverInputConnector(Vector3 mousePoint)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!HasInputConnector)
		{
			return false;
		}
		return DoesScreenPointHitCollider(mousePoint, inputConnectorObject.GetComponentInChildren<Collider>());
	}

	public bool IsPointOverOutputConnector(Vector3 mousePoint)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!HasOutputConnector)
		{
			return false;
		}
		return DoesScreenPointHitCollider(mousePoint, outputConnectorObject.GetComponentInChildren<Collider>());
	}

	public virtual void HighlightConnector(bool state)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected Obj, but got Unknown
		//IL_00b7: Expected Obj, but got Unknown
		if (selectedConnector == SelectedConnector.Input)
		{
			Renderer componentInChildren = ((Component)inputConnectorObject.GetComponentInChildren<Collider>()).GetComponentInChildren<Renderer>();
			if (!((Object)(object)componentInChildren == (Object)null))
			{
				componentInChildren.material = (state ? ((Material)Resources.Load("Materials/LogicCubeConnectorRedSelected")) : ((Material)Resources.Load("Materials/LogicCubeConnectorRed")));
			}
		}
		else if (selectedConnector == SelectedConnector.Output)
		{
			Renderer componentInChildren2 = ((Component)outputConnectorObject.GetComponentInChildren<Collider>()).GetComponentInChildren<Renderer>();
			if (!((Object)(object)componentInChildren2 == (Object)null))
			{
				componentInChildren2.material = (state ? ((Material)Resources.Load("Materials/LogicCubeConnectorBlueSelected")) : ((Material)Resources.Load("Materials/LogicCubeConnectorBlue")));
			}
		}
	}

	private bool DoesScreenPointHitCollider(Vector3 point, Collider collider)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Ray val = ((Component)MVGameController.Instance.Game.CameraController).camera.ScreenPointToRay(point);
		RaycastHit[] array = Physics.RaycastAll(val);
		if (array.Length > 0)
		{
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit val2 = array2[i];
				if ((Object)(object)val2.collider == (Object)(object)collider)
				{
					return true;
				}
			}
		}
		return false;
	}

	public virtual Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Debug.LogWarning((object)$"GetLocalBounds has not been implemented for {GetType().Name}.");
		return new Bounds(Vector3.zero, Vector3.zero);
	}

	public Vector3[] GetBoundsCornersLocal(BoundsContext boundsContext)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return SharedCubeFunctions.GetCorners(GetLocalBounds(boundsContext));
	}

	public Vector3[] GetBoundsCornersWorld(BoundsContext boundsContext)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 localToWorld = transform.localToWorldMatrix;
		return GetBoundsCornersLocal(boundsContext).Select((Vector3 localCorner) =>
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return localToWorld.MultiplyPoint(localCorner);
		}).ToArray();
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
		PreviewBox previewBox = gameObject.GetComponentInChildren<PreviewBox>();
		if ((Object)(object)previewBox == (Object)null)
		{
			GameObject val = CreateBox("PreviewBox", 1.005f);
			previewBox = val.AddComponent<PreviewBox>();
		}
		previewBox.Show("Materials/PreviewBoxMaterial", GetBoundsCornersLocal(BoundsContext.BoxVisualization));
	}

	public virtual void AddSelectionBox()
	{
		SelectionBox selectionBox = gameObject.GetComponentInChildren<SelectionBox>();
		if ((Object)(object)selectionBox == (Object)null)
		{
			GameObject val = CreateBox("SelectionBox", 1.001f);
			selectionBox = val.AddComponent<SelectionBox>();
		}
		selectionBox.FadeIn(0.2f, "Materials/SelectBoxMaterial", GetBoundsCornersLocal(BoundsContext.BoxVisualization));
	}

	protected GameObject CreateBox(string name, float scale)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		val.transform.parent = gameObject.transform;
		val.transform.localPosition = Vector3.zero;
		val.transform.localRotation = Quaternion.identity;
		val.transform.localScale = Vector3.one * scale;
		return val;
	}

	public virtual void RemoveSelectionBox()
	{
		if (!((Object)(object)gameObject == (Object)null))
		{
			SelectionBox componentInChildren = gameObject.GetComponentInChildren<SelectionBox>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				componentInChildren.FadeOutDestroy(0.8f);
			}
		}
	}

	public virtual void RemovePreviewBox()
	{
		if (!((Object)(object)gameObject == (Object)null))
		{
			PreviewBox componentInChildren = gameObject.GetComponentInChildren<PreviewBox>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				componentInChildren.DestroyBox();
			}
		}
	}

	public virtual bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref TextSlotIndex errorTextIndex)
	{
		gameObject.SetActiveRecursively(false);
		worldObjectClientManager.UnregisterWorldObject(id);
		return true;
	}

	public virtual void DeleteFailed()
	{
		if ((Object)(object)gameObject != (Object)null)
		{
			gameObject.SetActiveRecursively(true);
		}
	}

	public void ClearTransformQueue()
	{
		if (NetworkObject != null && (object)NetworkObject.GetType() == typeof(MVNetworkListener))
		{
			(NetworkObject as MVNetworkListener).ClearTransformQueue();
		}
	}

	public void SetName()
	{
		((Object)gameObject).name = ToString();
	}

	public override string ToString()
	{
		return GetType().ToString() + " id " + id + " group id " + groupId + " item id " + itemId;
	}

	public virtual Vector3 GetTargetPosition()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Collider componentInChildren = gameObject.GetComponentInChildren<Collider>();
		if ((Object)(object)componentInChildren == (Object)null)
		{
			Debug.LogWarning((object)("Did not find collider. Using transform.Position for " + this));
			return transform.position;
		}
		Bounds bounds = componentInChildren.bounds;
		return bounds.center;
	}

	public float ComputeObjectRadius()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Bounds localBounds = GetLocalBounds(BoundsContext.Default);
		Vector3 val = localBounds.size.Multiply(Scale) * 0.5f;
		return val.magnitude;
	}

	public void RotateAround(Vector3 pivot, Vector3 axis, float angle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = transform.rotation;
		transform.RotateAround(pivot, axis, angle);
		Quaternion val2 = transform.rotation;
		if (val != val2 && RotationChanged != null)
		{
			RotationChanged(this, new RotationChangedEventArgs(val, val2));
		}
	}
}
