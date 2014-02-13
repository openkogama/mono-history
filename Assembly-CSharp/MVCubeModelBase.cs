using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVCubeModelBase : MVWorldObjectClient, ICubeModel
{
	protected RuntimePrototypeCubeModel prototypeCubeModel;

	public Dictionary<IntVector, GameObject> chunkInstances = new Dictionary<IntVector, GameObject>();

	private bool beingEdited;

	private Queue<CubeModelChangedEventArgs> changedEventArgsQueue = new Queue<CubeModelChangedEventArgs>();

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
				prototypeCubeModel.DirtyChunksRegenerated -= DirtyChunksRegeneratedHandler;
				prototypeCubeModel = value;
				prototypeCubeModel.DirtyChunksRegenerated += DirtyChunksRegeneratedHandler;
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

	public List<GameObject> Chunks => new List<GameObject>(chunkInstances.Values);

	public MeshFilter[] MeshFilters
	{
		get
		{
			List<GameObject> chunks = Chunks;
			MeshFilter[] array = new MeshFilter[chunkInstances.Values.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = chunks[i].GetComponent<MeshFilter>();
			}
			return array;
		}
	}

	public event EventHandler<CubeModelChangedEventArgs> Changed;

	public event EventHandler<EditStateEventArgs> BeingEditedChanged;

	public MVCubeModelBase(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
		: base(data, worldObjects)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		int key = (int)Data["protoTypeID"];
		prototypeCubeModel = prototypes[key];
		prototypeCubeModel.CreateInstance(this);
		prototypeCubeModel.DirtyChunksRegenerated += DirtyChunksRegeneratedHandler;
		gameObject.transform.localScale = Vector3.one * prototypes[key].Scale;
		Scale = Vector3.one * prototypes[key].Scale;
		ModelingConstraintBuilder = () => new ModelingDynamicBoxConstraint(this, SharedCubeFunctions.CubeConstraint);
		SetName();
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
			MVGameController.Instance.Game.World.WorldInventory.RequestWoMakeUniquePrototype(id);
		}
	}

	public void RemoveCube(IntVector pos)
	{
		MakeUnique();
		if (prototypeCubeModel.RemoveCube(pos))
		{
			changedEventArgsQueue.Enqueue(new CubeModelChangedEventArgs(CubeAction.Deleted, pos));
		}
	}

	public void AddCube(IntVector pos, CubeBase cube)
	{
		MakeUnique();
		if (prototypeCubeModel.AddCube(pos, (Cube)cube))
		{
			changedEventArgsQueue.Enqueue(new CubeModelChangedEventArgs(CubeAction.Added, pos));
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
		changedEventArgsQueue.Enqueue(new CubeModelChangedEventArgs(CubeAction.FaceChanged, iVector));
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

	public void RemoveCubeNetworkUpdate(IntVector pos)
	{
		prototypeCubeModel.RemoveCubeNetworkUpdate(pos);
	}

	public void AddCubeNetworkUpdate(IntVector pos, CubeBase cube)
	{
		Cube cube2 = new Cube(cube.ByteCorners, cube.FaceMaterials);
		prototypeCubeModel.AddCubeNetworkUpdate(pos, cube2);
	}

	public void CubePosToChunkPos(ref IntVector pos)
	{
		prototypeCubeModel.CubePosToChunkPos(ref pos);
	}

	public Bounds GetMeshBounds()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default;
		if (Chunks.Count == 0)
		{
			return result;
		}
		result = Chunks[0].GetComponent<MeshFilter>().sharedMesh.bounds;
		for (int i = 1; i < Chunks.Count; i++)
		{
			MeshFilter component = Chunks[i].GetComponent<MeshFilter>();
			for (int j = 0; j < 3; j++)
			{
				Bounds bounds = component.sharedMesh.bounds;
				Vector3 min = bounds.min;
				float num = min[j];
				Vector3 min2 = result.min;
				if (num < min2[j])
				{
					Vector3 min3 = result.min;
					int num2 = j;
					Bounds bounds2 = component.sharedMesh.bounds;
					Vector3 min4 = bounds2.min;
					min3[num2] = min4[j];
					result.SetMinMax(min3, result.max);
				}
				Bounds bounds3 = component.sharedMesh.bounds;
				Vector3 max = bounds3.max;
				float num3 = max[j];
				Vector3 max2 = result.max;
				if (num3 > max2[j])
				{
					Vector3 max3 = result.max;
					int num4 = j;
					Bounds bounds4 = component.sharedMesh.bounds;
					Vector3 max4 = bounds4.max;
					max3[num4] = max4[j];
					result.SetMinMax(result.min, max3);
				}
			}
		}
		return result;
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetMeshBounds();
	}

	public Vector3 GetWorldCenterPos()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Transform val = transform;
		Bounds meshBounds = GetMeshBounds();
		return val.TransformPoint(meshBounds.center);
	}

	public void Enable(bool active)
	{
		List<GameObject> chunks = Chunks;
		foreach (GameObject item in chunks)
		{
			item.active = active;
		}
	}

	private void DirtyChunksRegeneratedHandler(object sender, EventArgs args)
	{
		while (0 < changedEventArgsQueue.Count)
		{
			CubeModelChangedEventArgs e = changedEventArgsQueue.Dequeue();
			if (Changed != null)
			{
				Changed(this, e);
			}
		}
	}
}
