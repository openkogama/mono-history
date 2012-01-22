using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class RuntimePrototypeCubeModel
{
	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(RuntimePrototypeCubeModel));

	private HashSet<IntVector> dirtyChunks = new HashSet<IntVector>();

	private MeshGeneratePriority meshGeneratePriority;

	private bool useMeshGeneratePrioritySystem = true;

	private PrototypeState prototypeState = PrototypeState.Pending;

	private List<byte> pendingDeltaCubes = new List<byte>();

	private float scale;

	protected int prototypeId = -1;

	private int mvItemId = -1;

	protected string name;

	private DeltaCubes deltaCubes = new DeltaCubes();

	private Dictionary<IntVector, CubeModelChunk> chunks = new Dictionary<IntVector, CubeModelChunk>();

	private HashSet<int> instances = new HashSet<int>();

	private HashSet<IntVector> chunkPositionsBookkeeping = new HashSet<IntVector>();

	public MeshGeneratePriority MeshGeneratePriority => meshGeneratePriority;

	public PrototypeState PrototypeState
	{
		get
		{
			return prototypeState;
		}
		set
		{
			switch (value)
			{
			case PrototypeState.Pending:
				prototypeId = -1;
				break;
			case PrototypeState.Registered:
				if (prototypeId != -1)
				{
					logger.Log("pendingDeltaCubes.Count " + pendingDeltaCubes.Count);
					if (pendingDeltaCubes.Count > 0)
					{
						MVGameController.Instance.Game.UpdatePrototype(prototypeId, pendingDeltaCubes.ToArray());
						pendingDeltaCubes.Clear();
					}
				}
				else
				{
					Debug.LogError((object)"prototypeId is -1 which means that is it no yet assigned");
				}
				break;
			}
			prototypeState = value;
		}
	}

	public float Scale => scale;

	public int PrototypeId
	{
		get
		{
			return prototypeId;
		}
		set
		{
			prototypeId = value;
		}
	}

	public int MVItemId => mvItemId;

	public string Name => name;

	public int InstancesCount => instances.Count;

	public int DeltaCubesCount => deltaCubes.Count;

	public DeltaCubes DeltaCubes => deltaCubes;

	public Dictionary<IntVector, CubeModelChunk> Chunks => chunks;

	private RuntimePrototypeCubeModel()
	{
	}

	public RuntimePrototypeCubeModel(MVItem item)
	{
		if (MVGameController.Instance.WOCM.PlayerRepository.ItemTypes[item.itemTypeID] != "CubeModel")
		{
			Debug.LogError((object)"Item is not a cubeModel");
		}
		MVPrototype mVPrototype = new MVPrototype();
		mVPrototype.ItemID = item.itemID;
		mVPrototype.Scale = 1f;
		mVPrototype.Name = item.name;
		mVPrototype.ID = -1;
		mVPrototype.Data = new Hashtable();
		mVPrototype.Data.Add((byte)121, item.data);
		CreateFromPrototype(mVPrototype);
	}

	public RuntimePrototypeCubeModel(MVPrototype prototype)
	{
		CreateFromPrototype(prototype);
		PrototypeState = PrototypeState.Registered;
		CreateMipMapMeshes();
	}

	public RuntimePrototypeCubeModel CloneGeometry(bool withDeltaCubes = false)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = new RuntimePrototypeCubeModel();
		runtimePrototypeCubeModel.mvItemId = mvItemId;
		runtimePrototypeCubeModel.scale = scale;
		runtimePrototypeCubeModel.name = name;
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			runtimePrototypeCubeModel.chunks.Add(chunk.Key, chunk.Value.CloneGeometry(Vector3.one * scale));
		}
		if (withDeltaCubes)
		{
			runtimePrototypeCubeModel.deltaCubes = new DeltaCubes(deltaCubes.CubeChange);
		}
		return runtimePrototypeCubeModel;
	}

	private void CreateFromPrototype(MVPrototype prototype)
	{
		mvItemId = prototype.ItemID;
		scale = prototype.Scale;
		prototypeId = prototype.ID;
		name = prototype.Name;
		BytePacker bp = new BytePacker((byte[])prototype.Data[(byte)121]);
		CreateFromBytePackage(bp);
		SetVisibility();
		RebuildPrototypeMesh();
	}

	public bool MeshGenerateDirtyChunks(ref int meshUpdates)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (meshGeneratePriority == MeshGeneratePriority.High)
		{
			foreach (IntVector dirtyChunk in dirtyChunks)
			{
				RebuildChunk(dirtyChunk, scale * Vector3.one);
				meshUpdates--;
			}
			dirtyChunks.Clear();
		}
		else
		{
			HashSet<IntVector> hashSet = new HashSet<IntVector>();
			foreach (IntVector dirtyChunk2 in dirtyChunks)
			{
				RebuildChunk(dirtyChunk2, scale * Vector3.one);
				hashSet.Add(dirtyChunk2);
				meshUpdates--;
				if (meshUpdates <= 0)
				{
					break;
				}
			}
			foreach (IntVector item in hashSet)
			{
				dirtyChunks.Remove(item);
			}
		}
		if (dirtyChunks.Count == 0)
		{
			meshGeneratePriority = MeshGeneratePriority.None;
			return true;
		}
		return false;
	}

	public GameObject GetMesh()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected Obj, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject();
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			GameObject val2 = new GameObject();
			MeshRenderer val3 = val2.AddComponent<MeshRenderer>();
			MeshFilter val4 = val2.AddComponent<MeshFilter>();
			val4.sharedMesh = chunk.Value.GetMeshData(MeshSetting.OriginalMesh).mesh;
			((Renderer)val3).sharedMaterials = chunk.Value.GetMeshData(MeshSetting.OriginalMesh).materials;
			val2.transform.parent = val.transform;
			val2.transform.position = Vector3.zero;
			val2.transform.rotation = Quaternion.identity;
		}
		return val;
	}

	public void CreateMipMapMeshes()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			chunk.Value.RebuildMipMapMesh(Vector3.one * scale);
		}
	}

	private void SetVisibility()
	{
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			chunk.Value.SetCubeVisibility();
		}
	}

	public int GetCubeCount()
	{
		int num = 0;
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			num += chunk.Value.CubeCount;
		}
		return num;
	}

	public bool CubesLeft()
	{
		if (chunks.Count == 0)
		{
			return false;
		}
		return true;
	}

	public Vector3 GetRandomCubePos(GameObject go)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		List<IntVector> list = chunks.Keys.ToList();
		return SharedCubeFunctions.LocalToWorld(go, chunks[list[Random.Range(0, list.Count)]].GetFirstSolidCubePos());
	}

	public Cube GetCube(IntVector cubePos)
	{
		return GetChunkFromCubePos(cubePos)?.GetCube(cubePos);
	}

	public void AddCube(IntVector pos, Cube cube)
	{
		IntVector key = SharedCubeFunctions.CubePosToChunk(pos, CubeModelChunk.ChunkSize);
		if (!chunks.ContainsKey(key) || !chunks[key].ContainsCube(pos))
		{
			AddToChunk(pos, cube, MeshGeneratePriority.High);
			deltaCubes.Enqueue(pos, CubeAction.Added);
		}
	}

	public void UnIndentCubeFace(IntVector localPos, Face face, Cube cube)
	{
		if (cube != null)
		{
			Cube.UnIndentFace(cube, face);
			AddToChunk(localPos, cube, MeshGeneratePriority.High);
			deltaCubes.Enqueue(localPos, CubeAction.CornersChangedDone);
		}
	}

	public void SetMaterial(IntVector iVector, Face face, byte materialId)
	{
		Cube cube = GetCube(iVector);
		if (!(cube == null))
		{
			Cube.SetMaterial(cube, face, materialId);
			AddToChunk(iVector, cube, MeshGeneratePriority.High);
			deltaCubes.Enqueue(iVector, CubeAction.FaceChanged);
		}
	}

	public void ReplaceCube(IntVector iVector, byte materialId)
	{
		Cube cube = GetCube(iVector);
		if (cube == null)
		{
			return;
		}
		foreach (int value in Enum.GetValues(typeof(Face)))
		{
			Cube.SetMaterial(cube, (Face)value, materialId);
		}
		AddToChunk(iVector, cube, MeshGeneratePriority.High);
		deltaCubes.Enqueue(iVector, CubeAction.FaceChanged);
	}

	public void CornersChangedDone(IntVector iVector, Cube cube)
	{
		AddToChunk(iVector, cube, MeshGeneratePriority.High);
		deltaCubes.Enqueue(iVector, CubeAction.CornersChangedDone);
	}

	public void CornersChanged(IntVector iVector, Cube cube)
	{
		AddToChunk(iVector, cube, MeshGeneratePriority.High);
		deltaCubes.Enqueue(iVector, CubeAction.CornersChanged);
	}

	public void RemoveCube(IntVector iVector)
	{
		IntVector key = SharedCubeFunctions.CubePosToChunk(iVector, CubeModelChunk.ChunkSize);
		if (chunks.ContainsKey(key) && chunks[key].ContainsCube(iVector))
		{
			RemoveFromChunk(iVector, MeshGeneratePriority.High);
			deltaCubes.Enqueue(iVector, CubeAction.Deleted);
		}
	}

	public void CreateInstance(MVCubeModelBase cm)
	{
		foreach (KeyValuePair<IntVector, GameObject> chunkInstance in cm.chunkInstances)
		{
			Object.Destroy((Object)(object)chunkInstance.Value);
		}
		cm.chunkInstances.Clear();
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			SetInstanceDataRef(chunk.Key, cm);
		}
		instances.Add(cm.Id);
	}

	public void RemoveInstance(int id)
	{
		instances.Remove(id);
	}

	public void ResetSharedMaterials(MVCubeModelInstance cm)
	{
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			cm.GetChunkInstance(chunk.Key).renderer.sharedMaterials = chunk.Value.GetMeshData(MeshSetting.OriginalMesh).materials;
		}
	}

	private static void DecodeBytePacker(BytePacker bp, RuntimePrototypeCubeModel rpcm)
	{
		while (bp.Position < bp.Length)
		{
			CubeAction cubeAction = (CubeAction)bp.ReadByte();
			IntVector iVector = new IntVector(bp.ReadInt16(), bp.ReadInt16(), bp.ReadInt16());
			switch (cubeAction)
			{
			case CubeAction.Added:
			case CubeAction.FaceChanged:
			case CubeAction.CornersChangedDone:
				rpcm.RemoveCubeNetworkUpdate(iVector);
				rpcm.AddCubeNetworkUpdate(iVector, new Cube(bp, bp.ReadByte()));
				break;
			case CubeAction.Deleted:
				rpcm.RemoveCubeNetworkUpdate(iVector);
				break;
			}
		}
	}

	public void UpdatePrototype(BytePacker bp)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		DecodeBytePacker(bp, this);
		foreach (int instance in instances)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(instance);
			if (worldObjectClient.OwnerActorNr != 0 && worldObjectClient.OwnerActorNr != MVGameController.Instance.WOCM.LocalPlayerActorNumber)
			{
				worldObjectClient.Select(Color.blue);
			}
		}
	}

	public void UpdatePrototypeScale(float scale)
	{
		Debug.LogError((object)"This must be reimplemented!");
	}

	public void AddCubeNetworkUpdate(IntVector iVector, Cube cube)
	{
		AddToChunk(iVector, cube, MeshGeneratePriority.Low);
	}

	public void RemoveCubeNetworkUpdate(IntVector iVector)
	{
		RemoveFromChunk(iVector, MeshGeneratePriority.Low);
	}

	public void HandleDelta()
	{
		while (deltaCubes.Count > 0)
		{
			byte[] array = deltaCubes.Dequeue(this);
			if (array != null)
			{
				if (prototypeState == PrototypeState.Registered)
				{
					MVGameController.Instance.Game.UpdatePrototype(prototypeId, array);
				}
				else if (prototypeState == PrototypeState.Pending)
				{
					pendingDeltaCubes.AddRange(array);
				}
			}
		}
	}

	private void RebuildChunk(IntVector chunkPos, Vector3 scale)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (chunks.ContainsKey(chunkPos))
		{
			CubeModelChunk cubeModelChunk = chunks[chunkPos];
			cubeModelChunk.RebuildChunk(scale);
		}
	}

	private void RebuildPrototypeMesh()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			chunk.Value.RebuildChunk(Vector3.one * scale);
		}
	}

	private void AddChunk(IntVector chunkPos)
	{
		foreach (int instance in instances)
		{
			SetInstanceDataRef(chunkPos, (MVCubeModelBase)MVGameController.Instance.WOCM.WorldObjects[instance]);
		}
	}

	private void RemoveChunk(IntVector chunkPos)
	{
		foreach (int instance in instances)
		{
			MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)MVGameController.Instance.WOCM.WorldObjects[instance];
			Object.Destroy((Object)(object)mVCubeModelBase.chunkInstances[chunkPos]);
			mVCubeModelBase.chunkInstances.Remove(chunkPos);
		}
	}

	private void SetInstanceDataRef(IntVector chunkPos, MVCubeModelBase cubeInstance)
	{
		chunks[chunkPos].SetInstanceDataRef(chunkPos, cubeInstance);
	}

	private void CreateFromBytePackage(BytePacker bp)
	{
		int num = bp.ReadInt32();
		logger.Log("Cube Count " + num);
		for (int i = 0; i < num; i++)
		{
			IntVector intVector = new IntVector(bp.ReadInt16(), bp.ReadInt16(), bp.ReadInt16());
			byte b = bp.ReadByte();
			Cube cube = new Cube(bp, b);
			AddToChunk(intVector, cube, MeshGeneratePriority.None, setVisibility: false);
			int cubesInRow = CubeDataPacker.GetCubesInRow(b);
			for (int j = 1; j < cubesInRow; j++)
			{
				IntVector iVector = intVector;
				iVector.x += (short)j;
				AddToChunk(iVector, Cube.Clone(cube), MeshGeneratePriority.None, setVisibility: false);
			}
		}
	}

	private void AddToChunk(IntVector iVector, Cube cube, MeshGeneratePriority meshGeneratePriority, bool setVisibility = true)
	{
		IntVector intVector = SharedCubeFunctions.CubePosToChunk(iVector, CubeModelChunk.ChunkSize);
		if (!chunks.ContainsKey(intVector))
		{
			CubeModelChunk value = new CubeModelChunk(intVector);
			chunks.Add(intVector, value);
			AddChunk(intVector);
		}
		chunks[intVector].AddToChunk(iVector, cube, setVisibility);
		AddToDirtyChunks(intVector, meshGeneratePriority);
	}

	private CubeModelChunk GetChunkFromCubePos(IntVector cubePos)
	{
		IntVector key = SharedCubeFunctions.CubePosToChunk(cubePos, CubeModelChunk.ChunkSize);
		if (chunks.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	private void RemoveFromChunk(IntVector iVector, MeshGeneratePriority meshGeneratePriority)
	{
		IntVector intVector = SharedCubeFunctions.CubePosToChunk(iVector, CubeModelChunk.ChunkSize);
		if (chunks.ContainsKey(intVector))
		{
			chunks[intVector].RemoveFromChunk(iVector);
			if (chunks[intVector].CubeCount == 0)
			{
				Debug.Log((object)"cube count is 0");
				chunks[intVector].Destroy();
				chunks.Remove(intVector);
				RemoveChunk(intVector);
			}
			else
			{
				AddToDirtyChunks(intVector, meshGeneratePriority);
			}
		}
	}

	private void AddToDirtyChunks(IntVector chunkPos, MeshGeneratePriority meshGeneratePriority)
	{
		if (useMeshGeneratePrioritySystem && meshGeneratePriority != MeshGeneratePriority.None)
		{
			dirtyChunks.Add(chunkPos);
			if (this.meshGeneratePriority == MeshGeneratePriority.None)
			{
				MVGameController.Instance.WOCM.WorldInventory.AddRuntimePrototypeToDirty(this);
			}
			if (meshGeneratePriority > this.meshGeneratePriority)
			{
				this.meshGeneratePriority = meshGeneratePriority;
			}
		}
	}

	public static BytePacker GetBytePackerFromCubeDict(Dictionary<IntVector, Cube> cubesDict, bool addCount)
	{
		BytePacker bytePacker = new BytePacker();
		if (addCount)
		{
			bytePacker.Write(cubesDict.Count);
		}
		foreach (KeyValuePair<IntVector, Cube> item in cubesDict)
		{
			CubeDataPacker.WriteCompressedCube(bytePacker, item.Key.x, item.Key.y, item.Key.z, item.Value.ByteCorners, item.Value.FaceMaterials);
		}
		return bytePacker;
	}

	public void CubePosToChunkPos(ref IntVector cubePos)
	{
		IntVector intVector = SharedCubeFunctions.CubePosToChunk(cubePos, CubeModelChunk.ChunkSize);
		cubePos.x -= (short)(CubeModelChunk.ChunkSize * intVector.x);
		cubePos.y -= (short)(CubeModelChunk.ChunkSize * intVector.y);
		cubePos.z -= (short)(CubeModelChunk.ChunkSize * intVector.z);
	}

	public bool CompareGeometry(RuntimePrototypeCubeModel rpcm)
	{
		if (GetCubeCount() != rpcm.GetCubeCount())
		{
			return false;
		}
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			if (!chunk.Value.CompareGeometry(rpcm.chunks[chunk.Key]))
			{
				return false;
			}
		}
		return true;
	}
}
