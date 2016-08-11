using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVCubeModelBase : MVWorldObjectClient, ICubeModel, ICubeModelCollider
{
	protected RuntimePrototypeCubeModel prototypeCubeModel;

	protected ChunkInstances chunkInstances = new ChunkInstances();

	private bool beingEdited;

	private Queue<CubeModelChangedEventArgs> changedEventArgsQueue = new Queue<CubeModelChangedEventArgs>();

	public Action<CubeModelChangedEventArgs> Changed;

	public Action<HashSet<IntVector>> ChunksChanged;

	public ChunkInstances ChunkInstances => chunkInstances;

	public RuntimePrototypeCubeModel PrototypeCubeModel
	{
		get
		{
			return prototypeCubeModel;
		}
		set
		{
			if (prototypeCubeModel != value)
			{
				RuntimePrototypeCubeModel runtimePrototypeCubeModel = prototypeCubeModel;
				runtimePrototypeCubeModel.DirtyChunksRegenerated = (Action<HashSet<IntVector>>)Delegate.Remove(runtimePrototypeCubeModel.DirtyChunksRegenerated, new Action<HashSet<IntVector>>(DirtyChunksRegeneratedHandler));
				prototypeCubeModel = value;
				RuntimePrototypeCubeModel runtimePrototypeCubeModel2 = prototypeCubeModel;
				runtimePrototypeCubeModel2.DirtyChunksRegenerated = (Action<HashSet<IntVector>>)Delegate.Combine(runtimePrototypeCubeModel2.DirtyChunksRegenerated, new Action<HashSet<IntVector>>(DirtyChunksRegeneratedHandler));
			}
		}
	}

	public int Pid => prototypeCubeModel.PrototypeId;

	public Func<IModelingConstraint> ModelingConstraintBuilder { get; set; }

	public bool BeingEdited
	{
		get
		{
			return beingEdited;
		}
		set
		{
			if (beingEdited != value)
			{
				beingEdited = value;
				if (BeingEditedChanged != null)
				{
					BeingEditedChanged(this, new EditStateEventArgs(value));
				}
			}
		}
	}

	public bool ContainsCubes => prototypeCubeModel.ContainsCubes;

	public int CubeCount => prototypeCubeModel.CubeCount;

	public float PrototypeScale => prototypeCubeModel.Scale;

	public MeshFilter[] MeshFilters
	{
		get
		{
			MeshFilter[] array = new MeshFilter[chunkInstances.Count];
			int num = 0;
			foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)chunkInstances)
			{
				array[num] = item.Value.filter;
				num++;
			}
			return array;
		}
	}

	public event EventHandler<EditStateEventArgs> BeingEditedChanged;

	public MVCubeModelBase(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
		: base(data, worldObjects)
	{
		int key = (int)Data["protoTypeID"];
		prototypeCubeModel = prototypes[key];
		prototypeCubeModel.CreateInstance(this);
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = prototypeCubeModel;
		runtimePrototypeCubeModel.DirtyChunksRegenerated = (Action<HashSet<IntVector>>)Delegate.Combine(runtimePrototypeCubeModel.DirtyChunksRegenerated, new Action<HashSet<IntVector>>(DirtyChunksRegeneratedHandler));
		gameObject.transform.localScale = Vector3.one * prototypes[key].Scale;
		Scale = Vector3.one * prototypes[key].Scale;
		ModelingConstraintBuilder = () => new ModelingDynamicBoxConstraint(this, SharedCubeFunctions.CubeConstraint);
		SetName();
	}

	public override void Initialize()
	{
		base.Initialize();
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = false;
		}
	}

	public override string ToString()
	{
		string text = base.ToString();
		if (prototypeCubeModel != null)
		{
			text = text + " authorProfileID " + prototypeCubeModel.AuthorProfileID;
		}
		return text + " can add to inventory " + ((interactionFlags & InteractionFlags.CanAddToInventory) != 0);
	}

	public void HandleDelta()
	{
		prototypeCubeModel.HandleDelta();
	}

	public CubeBase GetCubeBase(IntVector pos)
	{
		return prototypeCubeModel.GetCube(pos);
	}

	public Cube GetCube(IntVector pos)
	{
		return prototypeCubeModel.GetCube(pos);
	}

	public bool ContainsCube(IntVector pos)
	{
		if (prototypeCubeModel.GetCube(pos) != null)
		{
			return true;
		}
		return false;
	}

	private void MakeUnique()
	{
		if (prototypeCubeModel.InstancesCount > 1)
		{
			prototypeCubeModel.RemoveReferenceFromAllChunks();
			MVGameControllerBase.Game.World.WorldInventory.RequestWoMakeUniquePrototype(id);
			prototypeCubeModel.AddReferenceToAllChunks();
		}
	}

	public void RemoveCube(IntVector pos)
	{
		MakeUnique();
		if (prototypeCubeModel.RemoveCube(pos))
		{
			changedEventArgsQueue.Enqueue(new CubeModelChangedEventArgs(CubeAction.Deleted, pos, this));
		}
	}

	public void AddCube(IntVector pos, CubeBase cube)
	{
		MakeUnique();
		if (prototypeCubeModel.AddCube(pos, (Cube)cube))
		{
			changedEventArgsQueue.Enqueue(new CubeModelChangedEventArgs(CubeAction.Added, pos, this));
		}
	}

	public void SetMaterial(IntVector iVector, Face face, byte material)
	{
		MakeUnique();
		prototypeCubeModel.SetMaterial(iVector, face, material);
	}

	public void ReplaceCube(IntVector iVector, byte materialId)
	{
		MakeUnique();
		prototypeCubeModel.ReplaceCube(iVector, materialId);
		changedEventArgsQueue.Enqueue(new CubeModelChangedEventArgs(CubeAction.FaceChanged, iVector, this));
	}

	public void CornersChangedDone(IntVector iVector, Cube cube)
	{
		MakeUnique();
		prototypeCubeModel.CornersChangedDone(iVector, cube);
	}

	public void CornersChanged(IntVector iVector, Cube cube)
	{
		MakeUnique();
		prototypeCubeModel.CornersChanged(iVector, cube);
	}

	public void UnIndentCubeFace(IntVector localPos, Face face, Cube cube)
	{
		MakeUnique();
		prototypeCubeModel.UnIndentCubeFace(localPos, face, cube);
	}

	public virtual void RemoveCubeNetworkUpdate(IntVector pos)
	{
		prototypeCubeModel.RemoveCubeNetworkUpdate(pos, MeshGeneratePriority.Low);
	}

	public virtual void AddCubeNetworkUpdate(IntVector pos, CubeBase cube)
	{
		Cube cube2 = new Cube(cube.ByteCorners, cube.FaceMaterials);
		prototypeCubeModel.AddCubeNetworkUpdate(pos, cube2, MeshGeneratePriority.Low);
	}

	public void CubePosToChunkPos(ref IntVector pos)
	{
		prototypeCubeModel.CubePosToChunkPos(ref pos);
	}

	public Bounds GetWorldBounds()
	{
		Bounds result = default;
		if (chunkInstances.Count == 0)
		{
			return result;
		}
		Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)chunkInstances)
		{
			bool activeSelf = item.Value.gameObject.activeSelf;
			if (!activeSelf)
			{
				item.Value.gameObject.SetActive(value: true);
			}
			Bounds bounds = item.Value.collider.bounds;
			for (int i = 0; i < 3; i++)
			{
				if (bounds.min[i] < min[i])
				{
					min[i] = bounds.min[i];
				}
				if (bounds.max[i] > max[i])
				{
					max[i] = bounds.max[i];
				}
			}
			if (!activeSelf)
			{
				item.Value.gameObject.SetActive(value: false);
			}
		}
		result.SetMinMax(min, max);
		return result;
	}

	public Bounds GetBounds()
	{
		Bounds result = default;
		if (chunkInstances.Count == 0)
		{
			return result;
		}
		Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)chunkInstances)
		{
			BoxCollider boxCollider = item.Value.collider;
			Vector3 vector = boxCollider.size / 2f;
			Vector3 vector2 = boxCollider.center - vector;
			Vector3 vector3 = boxCollider.center + vector;
			for (int i = 0; i < 3; i++)
			{
				if (vector2[i] < min[i])
				{
					min[i] = vector2[i];
				}
				if (vector3[i] > max[i])
				{
					max[i] = vector3[i];
				}
			}
		}
		result.SetMinMax(min, max);
		return result;
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return GetBounds();
	}

	public Vector3 GetWorldCenterPos()
	{
		return transform.TransformPoint(GetBounds().center);
	}

	public void ObjectLinkChanged(bool visible)
	{
		bool active = visible;
		foreach (ObjectLink objectLinkRef in ObjectLinkRefs)
		{
			if (objectLinkRef.isSet)
			{
				active = true;
				break;
			}
		}
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)chunkInstances)
		{
			item.Value.gameObject.SetActive(active);
		}
	}

	protected virtual void DirtyChunksRegeneratedHandler(HashSet<IntVector> chunksChanged)
	{
		while (0 < changedEventArgsQueue.Count)
		{
			CubeModelChangedEventArgs obj = changedEventArgsQueue.Dequeue();
			if (Changed != null)
			{
				Changed(obj);
			}
		}
		if (ChunksChanged != null)
		{
			ChunksChanged(chunksChanged);
		}
	}
}
