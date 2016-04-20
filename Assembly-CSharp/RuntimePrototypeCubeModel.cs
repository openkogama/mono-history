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

	private int chunkSize = 32;

	private PrototypeState prototypeState = PrototypeState.Pending;

	private List<byte> pendingDeltaCubes = new List<byte>();

	private int authorProfileID;

	private float scale;

	protected int prototypeId = -1;

	private DeltaCubes deltaCubes = new DeltaCubes();

	private Dictionary<IntVector, CubeModelChunk> chunks = new Dictionary<IntVector, CubeModelChunk>();

	private HashSet<int> instances = new HashSet<int>();

	public MeshGeneratePriority MeshGeneratePriority => meshGeneratePriority;

	public int ChunkSize => chunkSize;

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
						MVGameControllerBase.OperationRequests.UpdatePrototype(prototypeId, pendingDeltaCubes.ToArray());
						pendingDeltaCubes.Clear();
					}
				}
				else
				{
					Debug.LogError("prototypeId is -1 which means that is it no yet assigned");
				}
				break;
			}
			prototypeState = value;
		}
	}

	public int AuthorProfileID => authorProfileID;

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

	public int InstancesCount => instances.Count;

	public int DeltaCubesCount => deltaCubes.Count;

	public DeltaCubes DeltaCubes => deltaCubes;

	public Dictionary<IntVector, CubeModelChunk> Chunks => chunks;

	public HashSet<int> Instances => new HashSet<int>(instances);

	public int CubeCount
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
			{
				num += chunk.Value.CubeCount;
			}
			return num;
		}
	}

	public bool ContainsCubes => CubeCount != 0;

	public event EventHandler DirtyChunksRegenerated = delegate
	{
	};

	private RuntimePrototypeCubeModel()
	{
	}

	public RuntimePrototypeCubeModel(int id, int authorProfileId, float scale, byte[] data)
	{
		Create(id, authorProfileId, scale, data);
	}

	public RuntimePrototypeCubeModel(int id, int authorProfileId, float scale, byte[] data, int chunkSize)
	{
		FineGrainedTerrainOverrideChunkSize(chunkSize);
		Create(id, authorProfileId, scale, data);
	}

	private void FineGrainedTerrainOverrideChunkSize(int size)
	{
		Debug.Log("ChunkSize overwritten");
		if (!Mathf.IsPowerOfTwo(size))
		{
			Debug.LogError("Not power of 2");
		}
		else if (CubeCount > 0)
		{
			Debug.LogError("Can not override chunk size if cubes already present");
		}
		else
		{
			chunkSize = size;
		}
	}

	private void Create(int id, int authorProfileId, float scale, byte[] data)
	{
		prototypeId = id;
		this.scale = scale;
		authorProfileID = authorProfileId;
		BytePacker bp = new BytePacker(data);
		CreateFromBytePackage(bp);
		SetVisibility();
		RebuildPrototypeMesh();
		PrototypeState = PrototypeState.Registered;
	}

	public RuntimePrototypeCubeModel CloneGeometry(bool withDeltaCubes = false)
	{
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = new RuntimePrototypeCubeModel();
		runtimePrototypeCubeModel.scale = scale;
		runtimePrototypeCubeModel.authorProfileID = authorProfileID;
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

	public void RemoveAllCubesLocal()
	{
		List<IntVector> list = chunks.Keys.ToList();
		foreach (IntVector item in list)
		{
			chunks[item].Destroy();
			chunks.Remove(item);
			RemoveChunk(item);
		}
	}

	public bool MeshGenerateDirtyChunksAll(ref int meshUpdates)
	{
		foreach (IntVector dirtyChunk in dirtyChunks)
		{
			RebuildChunk(dirtyChunk, scale * Vector3.one);
			meshUpdates--;
		}
		dirtyChunks.Clear();
		return MeshGenerateStatus();
	}

	public bool MeshGenerateDirtyChunks(ref int meshUpdates)
	{
		HashSet<IntVector> hashSet = new HashSet<IntVector>();
		foreach (IntVector dirtyChunk in dirtyChunks)
		{
			RebuildChunk(dirtyChunk, scale * Vector3.one);
			hashSet.Add(dirtyChunk);
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
		return MeshGenerateStatus();
	}

	private bool MeshGenerateStatus()
	{
		DirtyChunksRegenerated(this, EventArgs.Empty);
		if (dirtyChunks.Count == 0)
		{
			meshGeneratePriority = MeshGeneratePriority.None;
			return true;
		}
		return false;
	}

	public GameObject GetMesh()
	{
		GameObject gameObject = new GameObject();
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			GameObject gameObject2 = new GameObject();
			MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = gameObject2.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = chunk.Value.GetMeshData().mesh;
			meshRenderer.sharedMaterial = chunk.Value.GetMeshData().material;
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.position = Vector3.zero;
			gameObject2.transform.rotation = Quaternion.identity;
		}
		return gameObject;
	}

	private void SetVisibility()
	{
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			chunk.Value.SetCubeVisibility();
		}
	}

	public Vector3 GetRandomCubePos(GameObject go)
	{
		List<IntVector> list = chunks.Keys.ToList();
		return SharedCubeFunctions.LocalToWorld(go, chunks[list[UnityEngine.Random.Range(0, list.Count)]].GetFirstSolidCubePos());
	}

	public Cube GetCube(IntVector cubePos)
	{
		return GetChunkFromCubePos(cubePos)?.GetCube(cubePos);
	}

	public bool AddCube(IntVector pos, Cube cube)
	{
		IntVector key = SharedCubeFunctions.CubePosToChunk(pos, chunkSize);
		if (chunks.ContainsKey(key) && chunks[key].ContainsCube(pos))
		{
			return false;
		}
		AddToChunk(pos, cube, MeshGeneratePriority.HighGenerateAllDirty);
		deltaCubes.Enqueue(pos, CubeAction.Added);
		return true;
	}

	public void UnIndentCubeFace(IntVector localPos, Face face, Cube cube)
	{
		Debug.Log("UnIndentCubeFace");
		if (cube != null)
		{
			Cube.UnIndentFace(cube, face);
			AddToChunk(localPos, cube, MeshGeneratePriority.HighGenerateAllDirty);
			deltaCubes.Enqueue(localPos, CubeAction.CornersChangedDone);
		}
	}

	public void SetMaterial(IntVector iVector, Face face, byte materialId)
	{
		Cube cube = GetCube(iVector);
		if (!(cube == null))
		{
			Cube.SetMaterial(cube, face, materialId);
			AddToChunk(iVector, cube, MeshGeneratePriority.HighGenerateAllDirty);
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
		AddToChunk(iVector, cube, MeshGeneratePriority.HighGenerateAllDirty);
		deltaCubes.Enqueue(iVector, CubeAction.FaceChanged);
	}

	public void CornersChangedDone(IntVector iVector, Cube cube)
	{
		AddToChunk(iVector, cube, MeshGeneratePriority.HighGenerateAllDirty);
		deltaCubes.Enqueue(iVector, CubeAction.CornersChangedDone);
	}

	public void CornersChanged(IntVector iVector, Cube cube)
	{
		AddToChunk(iVector, cube, MeshGeneratePriority.HighGenerateAllDirty);
		deltaCubes.Enqueue(iVector, CubeAction.CornersChanged);
	}

	public bool RemoveCube(IntVector iVector)
	{
		IntVector key = SharedCubeFunctions.CubePosToChunk(iVector, chunkSize);
		if (!chunks.ContainsKey(key))
		{
			return false;
		}
		if (!chunks[key].ContainsCube(iVector))
		{
			return false;
		}
		RemoveFromChunk(iVector, MeshGeneratePriority.HighGenerateAllDirty);
		deltaCubes.Enqueue(iVector, CubeAction.Deleted);
		return true;
	}

	public void CreateInstance(MVCubeModelBase cm)
	{
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)cm.ChunkInstances)
		{
			UnityEngine.Object.Destroy(item.Value.gameObject);
		}
		cm.ChunkInstances.Clear();
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
			cm.GetChunkInstance(chunk.Key).renderer.sharedMaterial = chunk.Value.GetMeshData().material;
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
				rpcm.RemoveCubeNetworkUpdate(iVector, MeshGeneratePriority.Low);
				rpcm.AddCubeNetworkUpdate(iVector, new Cube(bp, bp.ReadByte()), MeshGeneratePriority.Low);
				break;
			case CubeAction.Deleted:
				rpcm.RemoveCubeNetworkUpdate(iVector, MeshGeneratePriority.Low);
				break;
			}
		}
	}

	public void UpdatePrototype(BytePacker bp)
	{
		DecodeBytePacker(bp, this);
		foreach (int instance in instances)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instance);
			if (worldObjectClient.OwnerActorNr != 0 && worldObjectClient.OwnerActorNr != MVGameControllerBase.Game.LocalPlayerActorNumber)
			{
				worldObjectClient.Select(Color.blue);
			}
		}
	}

	public void UpdatePrototypeScale(float scale)
	{
		Debug.LogError("This must be reimplemented!");
	}

	public void AddCubeNetworkUpdate(IntVector iVector, Cube cube, MeshGeneratePriority priority)
	{
		AddToChunk(iVector, cube, priority);
	}

	public void RemoveCubeNetworkUpdate(IntVector iVector, MeshGeneratePriority priority)
	{
		RemoveFromChunk(iVector, priority);
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
					MVGameControllerBase.OperationRequests.UpdatePrototype(prototypeId, array);
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
		if (chunks.ContainsKey(chunkPos))
		{
			CubeModelChunk cubeModelChunk = chunks[chunkPos];
			cubeModelChunk.RebuildChunk(scale);
		}
	}

	private void RebuildPrototypeMesh()
	{
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			chunk.Value.RebuildChunk(Vector3.one * scale);
		}
	}

	private void AddChunk(IntVector chunkPos)
	{
		foreach (int instance in instances)
		{
			SetInstanceDataRef(chunkPos, (MVCubeModelBase)MVGameControllerBase.WOCM.GetWorldObjectClient(instance));
		}
	}

	private void RemoveChunk(IntVector chunkPos)
	{
		foreach (int instance in instances)
		{
			MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)MVGameControllerBase.WOCM.GetWorldObjectClient(instance);
			UnityEngine.Object.Destroy(mVCubeModelBase.ChunkInstances.GetChunk(chunkPos).gameObject);
			mVCubeModelBase.ChunkInstances.Remove(chunkPos);
		}
	}

	private void SetInstanceDataRef(IntVector chunkPos, MVCubeModelBase cubeInstance)
	{
		chunks[chunkPos].SetInstanceDataRef(chunkPos, cubeInstance);
	}

	private void CreateFromBytePackage(BytePacker bp)
	{
		int num = bp.ReadInt32();
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
		IntVector intVector = SharedCubeFunctions.CubePosToChunk(iVector, chunkSize);
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
		IntVector key = SharedCubeFunctions.CubePosToChunk(cubePos, chunkSize);
		if (chunks.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	private void RemoveFromChunk(IntVector iVector, MeshGeneratePriority meshGeneratePriority)
	{
		IntVector intVector = SharedCubeFunctions.CubePosToChunk(iVector, chunkSize);
		if (chunks.ContainsKey(intVector))
		{
			chunks[intVector].RemoveFromChunk(iVector);
			if (chunks[intVector].CubeCount == 0)
			{
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
				MVGameControllerBase.Game.World.WorldInventory.AddRuntimePrototypeToDirty(this);
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
		IntVector intVector = SharedCubeFunctions.CubePosToChunk(cubePos, chunkSize);
		cubePos.x -= (short)(chunkSize * intVector.x);
		cubePos.y -= (short)(chunkSize * intVector.y);
		cubePos.z -= (short)(chunkSize * intVector.z);
	}

	public void AddRefenceToChunk(ref IntVector chunkPosition)
	{
		chunks[chunkPosition].ActiveInstances++;
	}

	public void RemoveRefenceFromChunk(ref IntVector chunkPosition)
	{
		chunks[chunkPosition].ActiveInstances--;
	}

	public int GetRefenceCountFromChunk(ref IntVector chunkPosition)
	{
		return chunks[chunkPosition].ActiveInstances;
	}

	public void AddReferenceToAllChunks()
	{
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			chunk.Value.ActiveInstances++;
		}
	}

	public void RemoveReferenceFromAllChunks()
	{
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			chunk.Value.ActiveInstances--;
		}
	}

	public bool CompareGeometry(RuntimePrototypeCubeModel rpcm)
	{
		if (CubeCount != rpcm.CubeCount)
		{
			return false;
		}
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			if (!rpcm.chunks.ContainsKey(chunk.Key))
			{
				return false;
			}
			if (!chunk.Value.CompareGeometry(rpcm.chunks[chunk.Key]))
			{
				return false;
			}
		}
		return true;
	}

	public void CompareGeometryDetailed(RuntimePrototypeCubeModel rpcm, bool visibleCubesOnly, ref int matchingCubeCount, ref int investigatedCubeCount)
	{
		foreach (KeyValuePair<IntVector, CubeModelChunk> chunk in chunks)
		{
			CubeModelChunk value = null;
			rpcm.chunks.TryGetValue(chunk.Key, out value);
			chunk.Value.CompareGeometry(value, ref matchingCubeCount, ref investigatedCubeCount, visibleCubesOnly);
		}
	}
}
