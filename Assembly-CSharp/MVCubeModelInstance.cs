using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class MVCubeModelInstance : MVCubeModelBase
{
	protected CullingSubscriberBase cullingSubscriberBase;

	private bool isVisible;

	private Vector3 positionOffset = Vector3.zero;

	public bool IsVisibleSet
	{
		get
		{
			return isVisible;
		}
		private set
		{
			isVisible = value;
		}
	}

	public MVCubeModelInstance(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
		: base(data, worldObjects, prototypes)
	{
		interactionFlags |= InteractionFlags.Selectable | InteractionFlags.HasCubeModel | InteractionFlags.CanRotateY | InteractionFlags.CanEdit | InteractionFlags.CanClone | InteractionFlags.TranslatbleXZ2D;
		if (MVGameControllerBase.Game.LocalPlayer.ProfileID == PrototypeCubeModel.AuthorProfileID && (interactionFlags & InteractionFlags.CanAddToInventory) != 0)
		{
			interactionFlags |= InteractionFlags.CanAddToInventory;
		}
		else
		{
			interactionFlags &= ~InteractionFlags.CanAddToInventory;
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		if (groupId == MVGameControllerBase.Game.WorldObjectClientManager.RootGroup.Id)
		{
			EnableCulling();
		}
	}

	public void SetCullDistanceBand(int distanceBandIndex)
	{
		cullingSubscriberBase.DistanceBandIndex = distanceBandIndex;
	}

	public void EnableCulling()
	{
		SetupCulling(OnStateChanged);
	}

	public void SetupCulling(UnityAction<CullingGroupEvent> onStateChanged)
	{
		cullingSubscriberBase = new CullingSubscriberBase(onStateChanged);
		SetCullSphereToMeshBounds();
		PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		RotationChanged = (UnityAction<MVWorldObjectClient, RotationChangedEventArgs>)Delegate.Combine(RotationChanged, new UnityAction<MVWorldObjectClient, RotationChangedEventArgs>(OnRotationChanged));
		ChunksChanged = (Action<HashSet<IntVector>>)Delegate.Combine(ChunksChanged, new Action<HashSet<IntVector>>(OnChanged));
	}

	private void OnRotationChanged(MVWorldObjectClient wo, RotationChangedEventArgs rotationChangedEventArgs)
	{
		SetCullSphereToMeshBounds();
	}

	private void OnChanged(HashSet<IntVector> chunks)
	{
		SetCullSphereToMeshBounds();
	}

	private void OnPositionChanged(MVWorldObjectClient wo, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = positionChangedEventArgs.NewPos - positionOffset;
	}

	private void SetCullSphereToMeshBounds()
	{
		Bounds worldBounds = GetWorldBounds();
		cullingSubscriberBase.Setup(worldBounds.extents.magnitude, worldBounds.center);
		positionOffset = WorldPosition - worldBounds.center;
	}

	public override bool CompareWithKoGaMaPackage(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, ref int insertedByProfileId)
	{
		insertedByProfileId = PrototypeCubeModel.AuthorProfileID;
		RuntimePrototypeCubeModel rpcm = koGaMaPackageClient.prototypes[(int)wo.Data["protoTypeID"]];
		return PrototypeCubeModel.CompareGeometry(rpcm);
	}

	public override void Compare(MVWorldObjectClient wo, bool visibleCubesOnly, ref int matchingCubeCount, ref int investigatedCubeCount)
	{
		if (wo.WorldObjectType == WorldObjectType)
		{
			RuntimePrototypeCubeModel rpcm = ((MVCubeModelInstance)wo).PrototypeCubeModel;
			PrototypeCubeModel.CompareGeometryDetailed(rpcm, visibleCubesOnly, ref matchingCubeCount, ref investigatedCubeCount);
		}
	}

	public void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		IsVisibleSet = IsLodVisible(cullingGroupEvent);
		ChangeLODVisible();
	}

	public bool IsLodVisible(CullingGroupEvent cullingGroupEvent)
	{
		return CullingApiWrapper.Visible(cullingGroupEvent, cullingSubscriberBase.DistanceBandIndex);
	}

	public void ChangeLODVisible()
	{
		if (!IsVisibleSet)
		{
			prototypeCubeModel.RemoveReferenceFromAllChunks();
			SetLod(enabled: false);
		}
		else
		{
			prototypeCubeModel.AddReferenceToAllChunks();
			SetLod(enabled: true);
		}
	}

	private void SetLod(bool enabled)
	{
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)chunkInstances)
		{
			item.Value.renderer.enabled = enabled;
		}
	}

	public override void Destroy()
	{
		prototypeCubeModel.RemoveReferenceFromAllChunks();
		prototypeCubeModel.RemoveInstance(id);
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
		base.Destroy();
	}

	public override void Select(Color color)
	{
		AddSelectionBox();
		Selected = true;
	}

	public override void AddSelectionBox()
	{
		SelectionBox selectionBox = base.gameObject.GetComponentInChildren<SelectionBox>();
		if (selectionBox == null)
		{
			GameObject gameObject = CreateBox("SelectionBox", 1.001f);
			selectionBox = gameObject.AddComponent<SelectionBox>();
		}
		Bounds bounds = GetBounds();
		selectionBox.transform.localPosition = bounds.center;
		selectionBox.FadeIn(0.2f, PrefabPool.Instance.SelectBoxMaterial, GetCorners(bounds));
	}

	public override void AddPreviewBox()
	{
		PreviewBox previewBox = base.gameObject.GetComponentInChildren<PreviewBox>();
		if (previewBox == null)
		{
			GameObject gameObject = CreateBox("PreviewBox", 1.005f);
			previewBox = gameObject.AddComponent<PreviewBox>();
		}
		Bounds bounds = GetBounds();
		previewBox.transform.localPosition = bounds.center;
		previewBox.Show(PrefabPool.Instance.PreviewBoxMaterial, GetCorners(bounds));
	}

	private Vector3[] GetCorners(Bounds bounds)
	{
		float num = 0f;
		num = ((MVGameControllerBase.GameMode != MVGameMode.Edit || !MVGameControllerBase.IEditModeUI.IsGridSnap()) ? 0.0625f : 1f);
		Vector3 vector = Vector3.one * 0.5f;
		Vector3 vector2 = bounds.min + vector;
		Vector3 vector3 = bounds.max + vector;
		float y = gameObject.transform.localScale.y;
		float num2 = num / y;
		Vector3 vector4 = MathFunctions.TruncateVector(vector2 / num2, 5);
		Vector3 vector5 = MathFunctions.TruncateVector(vector3 / num2, 5);
		vector4 = MathFunctions.FloorVector(vector4);
		vector5 = MathFunctions.CeilVector(vector5);
		Vector3 min = vector4 * num2 - vector - bounds.center;
		Vector3 max = vector5 * num2 - vector - bounds.center;
		return SharedCubeFunctions.GetCorners(min, max);
	}

	public override void DeSelect()
	{
		RemoveSelectionBox();
		Selected = false;
	}

	public ChunkInstances.ChunkInstanceVariables GetChunkInstance(IntVector chunkPos)
	{
		return chunkInstances.GetChunk(chunkPos);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		e.Event = EditorEvent.EditCubes;
		return true;
	}
}
