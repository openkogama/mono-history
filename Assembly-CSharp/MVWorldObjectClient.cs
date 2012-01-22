using System.Collections;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class MVWorldObjectClient : MVWorldObject
{
	protected bool isCastingShadows;

	protected static int woShadowCastersCount;

	protected static int woMaxShadowCasters = 20;

	protected string name;

	protected GameObject gameObject;

	protected MVNetworkObject networkObject;

	protected InteractionFlags interactionFlags;

	protected SelectedConnector selectedConnector;

	protected GameObject inputConnectorObject;

	protected GameObject outputConnectorObject;

	private MVRuntimeDataVariables runtimeDataVariables;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public InteractionFlags InteractionFlags => interactionFlags;

	public GameObject GameObject => gameObject;

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

	public virtual Vector3 SyncPos
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return gameObject.transform.position;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			Position = value;
			gameObject.transform.position = value;
			State = MVWorldObjectState.Dirty;
		}
	}

	public virtual Quaternion SyncRot
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return gameObject.transform.rotation;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			Rotation = value;
			gameObject.transform.rotation = value;
			State = MVWorldObjectState.Dirty;
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

	public MVWorldObjectClient()
	{
		runtimeDataVariables = new MVRuntimeDataVariables(this);
	}

	protected virtual void CreateMVWOC(bool local)
	{
	}

	public virtual void Initialize()
	{
	}

	public virtual void Destroy()
	{
		if ((Object)(object)gameObject != (Object)null)
		{
			Object.Destroy((Object)(object)gameObject);
		}
	}

	public virtual void OnDataUpdate()
	{
	}

	public virtual void OnRunTimeDataUpdate()
	{
	}

	public void RunTimeDataUpdate(Hashtable dataDelta)
	{
		RuntimeDataVariables.Receive(dataDelta);
		OnRunTimeDataUpdate();
	}

	protected void UpdateRunTimeData(Hashtable dataDelta)
	{
		MVGameController.Instance.Game.UpdateWorldObjectRunTimeData(Id, dataDelta);
	}

	public virtual void HandleInput(NetworkInputActionCodes actionCode, NetworkInputKeyCodes keyCode)
	{
	}

	public virtual void DeSelect()
	{
		selectedConnector = SelectedConnector.None;
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

	public void HighlightConnector(bool state)
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
		Ray val = ((Component)MVGameController.Instance.WOCM.WeCamera).camera.ScreenPointToRay(point);
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

	public virtual Bounds GetLocalBounds()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Debug.LogWarning((object)$"GetLocalBounds has not been implemented for {GetType().Name}.");
		return new Bounds(Vector3.zero, Vector3.zero);
	}

	public Vector3[] GetBoundsCornersLocal()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		Bounds localBounds = GetLocalBounds();
		return new Vector3[8]
		{
			new Vector3(localBounds.min.x, localBounds.max.y, localBounds.min.z),
			new Vector3(localBounds.max.x, localBounds.max.y, localBounds.min.z),
			new Vector3(localBounds.max.x, localBounds.max.y, localBounds.max.z),
			new Vector3(localBounds.min.x, localBounds.max.y, localBounds.max.z),
			new Vector3(localBounds.min.x, localBounds.min.y, localBounds.max.z),
			new Vector3(localBounds.max.x, localBounds.min.y, localBounds.max.z),
			new Vector3(localBounds.max.x, localBounds.min.y, localBounds.min.z),
			new Vector3(localBounds.min.x, localBounds.min.y, localBounds.min.z)
		};
	}

	public Vector3[] GetBoundsCornersWorld()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 localToWorld = GameObject.transform.localToWorldMatrix;
		return GetBoundsCornersLocal().Select((Vector3 localCorner) =>
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return localToWorld.MultiplyPoint(localCorner);
		}).ToArray();
	}

	public virtual bool GetTranslateData(out TranslateObjectData tod, float maxDistanceBase, float minDistance, float maxDistance)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		tod = default;
		MeshRenderer componentInChildren = GameObject.GetComponentInChildren<MeshRenderer>();
		if ((Object)(object)componentInChildren == (Object)null)
		{
			return false;
		}
		Bounds bounds = ((Renderer)componentInChildren).bounds;
		Vector3 extents = bounds.extents;
		tod.worldBoundsExtentsMagnitude = extents.magnitude;
		tod.maxDistance = Mathf.Clamp(maxDistanceBase * tod.worldBoundsExtentsMagnitude, minDistance, maxDistance);
		MeshFilter componentInChildren2 = GameObject.GetComponentInChildren<MeshFilter>();
		if ((Object)(object)componentInChildren2 == (Object)null)
		{
			return false;
		}
		tod.localBounds = componentInChildren2.sharedMesh.bounds;
		tod.transform = GameObject.transform;
		tod.gameObject = gameObject;
		tod.worldBounds = SharedCubeFunctions.GetAxisAlignedBoundsRecursively(tod.transform);
		return true;
	}

	public virtual void Select(Color color)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Material[] materials = gameObject.renderer.materials;
		Material[] array = materials;
		foreach (Material val in array)
		{
			val.color = color;
		}
		gameObject.renderer.materials = materials;
		AddSelectionBox();
	}

	public virtual void AddSelectionBox()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected Obj, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		SelectionBox selectionBox = gameObject.GetComponentInChildren<SelectionBox>();
		if ((Object)(object)selectionBox == (Object)null)
		{
			GameObject val = new GameObject("SelectionBox");
			val.transform.parent = gameObject.transform;
			val.transform.localPosition = Vector3.zero;
			val.transform.localRotation = Quaternion.identity;
			val.transform.localScale = Vector3.one * 1.001f;
			selectionBox = val.AddComponent<SelectionBox>();
		}
		selectionBox.FadeIn(0.2f, "Materials/SelectBoxMaterial", GetBoundsCornersLocal());
	}

	public virtual void RemoveSelectionBox()
	{
		SelectionBox componentInChildren = gameObject.GetComponentInChildren<SelectionBox>();
		if ((Object)(object)componentInChildren != (Object)null)
		{
			componentInChildren.FadeOutDestroy(0.8f);
		}
	}

	public virtual bool Delete()
	{
		GameObject.SetActiveRecursively(false);
		return true;
	}

	public virtual void DeleteFailed()
	{
		GameObject.SetActiveRecursively(true);
	}

	public void ClearTransformQueue()
	{
		if (NetworkObject != null && (object)NetworkObject.GetType() == typeof(MVNetworkListener))
		{
			(NetworkObject as MVNetworkListener).ClearTransformQueue();
		}
	}

	public void CreateGameObject(bool local)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected Obj, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected Obj, but got Unknown
		CreateMVWOC(local);
		if (HasInputConnector)
		{
			inputConnectorObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/InputConnectorObject"), InputConnectorOffset, Quaternion.identity);
			inputConnectorObject.transform.parent = gameObject.transform;
		}
		if (HasOutputConnector)
		{
			outputConnectorObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/OutputConnectorObject"), OutputConnectorOffset, Quaternion.identity);
			outputConnectorObject.transform.parent = gameObject.transform;
		}
		SetName();
		gameObject.transform.localPosition = Position;
		gameObject.transform.localRotation = Rotation;
		gameObject.transform.localScale = Scale;
		if (MVGameController.Instance.WOCM.HierarchiesHasBeenCreated && groupId != -1)
		{
			MVGroup mVGroup = (MVGroup)MVGameController.Instance.WOCM.GetWorldObjectClient(groupId);
			mVGroup.AddChild(this);
		}
		if ((object)GetType() != typeof(MVCubeModelPrototypeTerrain) && (object)GetType() != typeof(MVCubeModelFineGrainedTerrain))
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

	private void SetName()
	{
		((Object)gameObject).name = GetType().ToString() + " id " + id + " group id " + groupId;
	}

	public void SetId(int id)
	{
		base.id = id;
	}

	public void SetGroupId(int groupId)
	{
		base.groupId = groupId;
	}

	internal void DestroyGameObject()
	{
		Object.Destroy((Object)(object)gameObject);
	}

	public void SetParent(MVWorldObjectClient parent)
	{
		if (parent != null)
		{
			Debug.Log((object)"setting transform parent!");
			gameObject.transform.parent = parent.gameObject.transform;
		}
		else
		{
			gameObject.transform.parent = null;
		}
	}
}
