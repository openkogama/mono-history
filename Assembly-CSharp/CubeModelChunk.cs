using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class CubeModelChunk
{
	public class FaceData
	{
		public Vector3[] faceVertices = new Vector3[4];

		public Color[] colors = new Color[4];

		public Face face;
	}

	public static readonly bool UseAOShadows = true;

	private IntVector chunkPos;

	private List<ChunkInstances.ChunkInstanceVariables> instances = new List<ChunkInstances.ChunkInstanceVariables>();

	private SharedMeshData sharedMeshData = default;

	private Bounds meshBounds = default;

	private string name;

	private int cubeCount;

	private int triangleCount;

	private int activeInstances;

	private Dictionary<IntVector, Cell> cells = new Dictionary<IntVector, Cell>();

	private static FaceData[] faceData = new FaceData[6]
	{
		new FaceData(),
		new FaceData(),
		new FaceData(),
		new FaceData(),
		new FaceData(),
		new FaceData()
	};

	private static Vector2[] uvs = new Vector2[4];

	private static Vector2 uvOffsetVector = new Vector2(0f, 0f);

	private static Vector2 uvOffsetVector0 = Vector2.one * 0.5f;

	private static Vector2 uvOffsetVector1 = new Vector2(-0.5f, 0.5f);

	public int TriangleCount => triangleCount;

	public int ActiveInstances
	{
		get
		{
			return activeInstances;
		}
		set
		{
			activeInstances = value;
		}
	}

	public int CubeCount => cubeCount;

	public CubeModelChunk(IntVector iVector)
	{
		name = "chunk" + iVector.x + "." + iVector.y + "." + iVector.z;
		chunkPos = iVector;
		sharedMeshData = new SharedMeshData(new Mesh());
	}

	public CubeModelChunk CloneGeometry(Vector3 scale)
	{
		CubeModelChunk cubeModelChunk = new CubeModelChunk(chunkPos);
		foreach (KeyValuePair<IntVector, Cell> cell in cells)
		{
			cubeModelChunk.cells.Add(cell.Key, cell.Value.Clone());
		}
		cubeModelChunk.cubeCount = cubeCount;
		cubeModelChunk.RebuildChunk(scale);
		return cubeModelChunk;
	}

	public bool CompareGeometry(CubeModelChunk chunk)
	{
		if (cells.Count != chunk.cells.Count)
		{
			return false;
		}
		foreach (KeyValuePair<IntVector, Cell> cell in cells)
		{
			if (chunk.cells.TryGetValue(cell.Key, out var value))
			{
				if (cell.Value.cube != value.cube)
				{
					return false;
				}
				continue;
			}
			return false;
		}
		return true;
	}

	public bool CompareGeometry(CubeModelChunk chunk, ref int matchingCubeCount, ref int investigatedCubeCount, bool visibleCubesOnly)
	{
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<IntVector, Cell> cell in cells)
		{
			if (visibleCubesOnly && cell.Value.cube.HiddenSides == 63)
			{
				continue;
			}
			if (chunk != null && chunk.cells.TryGetValue(cell.Key, out var value))
			{
				bool flag = true;
				for (int i = 0; i < 8; i++)
				{
					if (cell.Value.cube.ByteCorners[i] != value.cube.ByteCorners[i])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					num++;
				}
			}
			num2++;
		}
		matchingCubeCount += num;
		investigatedCubeCount += num2;
		return num == num2;
	}

	public Cube GetCube(IntVector iVector)
	{
		if (cells.TryGetValue(iVector, out var value))
		{
			return value.cube;
		}
		return null;
	}

	public bool ContainsCube(IntVector iVector)
	{
		return cells.ContainsKey(iVector);
	}

	public void AddToChunk(IntVector iVector, Cube cube, bool setVisibility = true)
	{
		if (!cells.ContainsKey(iVector))
		{
			cubeCount++;
		}
		cells[iVector] = new Cell(cube);
		if (setVisibility)
		{
			SetCubeVisibilityWithNeighbors(iVector);
		}
	}

	public IntVector GetFirstSolidCubePos()
	{
		if (cells.Count > 0)
		{
			return cells.GetEnumerator().Current.Key;
		}
		Debug.LogError("No cube found in chunk. This is a problem");
		return IntVector.One;
	}

	public void RemoveFromChunk(IntVector iVector)
	{
		if (cells.ContainsKey(iVector))
		{
			cells.Remove(iVector);
			SetCubeVisibilityWithNeighbors(iVector);
			cubeCount--;
		}
	}

	public void Destroy()
	{
		foreach (ChunkInstances.ChunkInstanceVariables instance in instances)
		{
			Object.Destroy(instance.gameObject);
		}
		sharedMeshData.Destroy();
	}

	public void RebuildChunk(Vector3 scale)
	{
		MeshData meshData = new MeshData();
		triangleCount = RebuildMesh(cells, scale);
		meshData.SetToMesh(ref sharedMeshData.mesh, ref sharedMeshData.material);
		GetMeshBounds(ref meshBounds, cells, scale);
		UpdateInstances();
	}

	public SharedMeshData GetMeshData()
	{
		return sharedMeshData;
	}

	private void EvaluateReferenceCount(int oldReferenceCount, int newReferenceCount)
	{
		if (oldReferenceCount == 0 && newReferenceCount > 0 && MeshPool.Instance.GotFreeMesh)
		{
			RebuildChunk(Vector3.one * 4f);
			RestoreSharedMeshOnInstances();
			Debug.Log("Loading mesh");
		}
		else if (oldReferenceCount > 0 && newReferenceCount == 0)
		{
			RevokeSharedMeshOnInstances();
			Debug.Log("Unloading mesh");
		}
	}

	private void RevokeSharedMeshOnInstances()
	{
		foreach (ChunkInstances.ChunkInstanceVariables instance in instances)
		{
			instance.filter.sharedMesh = null;
			instance.renderer.sharedMaterial = null;
			instance.renderer.enabled = false;
		}
	}

	private void RestoreSharedMeshOnInstances()
	{
		foreach (ChunkInstances.ChunkInstanceVariables instance in instances)
		{
			instance.filter.sharedMesh = sharedMeshData.mesh;
			instance.renderer.sharedMaterial = sharedMeshData.material;
		}
	}

	private void UpdateInstances()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < instances.Count; i++)
		{
			if (instances[i].gameObject != null)
			{
				instances[i].renderer.sharedMaterial = sharedMeshData.material;
				MVGameControllerBase.WOCM.UpdateWorldBounds(meshBounds);
			}
			else
			{
				list.Add(i);
			}
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			instances.RemoveAt(list[num]);
		}
		for (int j = 0; j < instances.Count; j++)
		{
			ChunkInstances.ChunkInstanceVariables value = instances[j];
			value.collider.size = meshBounds.size;
			value.collider.center = meshBounds.center;
			instances[j] = value;
		}
	}

	public void SetInstanceDataRef(IntVector chunkPos, MVCubeModelBase cubeInstance)
	{
		CubeModelChunkPrefab cubeModelChunkPrefab = Object.Instantiate(PrefabPool.Instance.CubeModelChunkPrefab);
		cubeModelChunkPrefab.gameObject.name = name;
		cubeModelChunkPrefab.MeshFilter.sharedMesh = sharedMeshData.mesh;
		cubeModelChunkPrefab.MeshRenderer.sharedMaterial = sharedMeshData.material;
		cubeModelChunkPrefab.transform.parent = cubeInstance.Transform;
		cubeModelChunkPrefab.transform.localPosition = Vector3.zero;
		cubeModelChunkPrefab.transform.localRotation = Quaternion.identity;
		cubeModelChunkPrefab.transform.localScale = Vector3.one;
		cubeModelChunkPrefab.BoxCollider.size = meshBounds.size;
		cubeModelChunkPrefab.BoxCollider.center = meshBounds.center;
		cubeModelChunkPrefab.gameObject.layer = cubeInstance.GameObject.layer;
		ChunkInstances.ChunkInstanceVariables chunkInstanceVariables = new ChunkInstances.ChunkInstanceVariables
		{
			gameObject = cubeModelChunkPrefab.gameObject,
			collider = cubeModelChunkPrefab.BoxCollider,
			filter = cubeModelChunkPrefab.MeshFilter,
			renderer = cubeModelChunkPrefab.MeshRenderer
		};
		instances.Add(chunkInstanceVariables);
		cubeInstance.ChunkInstances.Add(chunkPos, chunkInstanceVariables);
	}

	private void SetCubeVisibilityWithNeighbors(IntVector pos)
	{
		IntVector cubeVisibility = new IntVector(pos.x, pos.y, pos.z);
		if (cells.TryGetValue(pos, out var value))
		{
			CubeBase.SetCubeFlags(value.cube);
		}
		SetCubeVisibility(cubeVisibility);
		cubeVisibility.x++;
		SetCubeVisibility(cubeVisibility);
		cubeVisibility.x -= 2;
		SetCubeVisibility(cubeVisibility);
		cubeVisibility.x++;
		cubeVisibility.y++;
		SetCubeVisibility(cubeVisibility);
		cubeVisibility.y -= 2;
		SetCubeVisibility(cubeVisibility);
		cubeVisibility.y++;
		cubeVisibility.z++;
		SetCubeVisibility(cubeVisibility);
		cubeVisibility.z -= 2;
		SetCubeVisibility(cubeVisibility);
		cubeVisibility.z++;
	}

	private void SetCubeVisibility(IntVector iVector)
	{
		if (cells.TryGetValue(iVector, out var value))
		{
			value.cube.HiddenSides = 0;
			SetCubeVisibility(cells, iVector, value.cube);
		}
	}

	public void SetCubeVisibility()
	{
		SetCubeVisibility(cells);
	}

	private static void SetCubeVisibility(Dictionary<IntVector, Cell> cells)
	{
		foreach (KeyValuePair<IntVector, Cell> cell in cells)
		{
			SetCubeVisibility(cells, cell.Key, cell.Value.cube);
		}
	}

	private static void SetCubeVisibility(Dictionary<IntVector, Cell> cells, IntVector pos, Cube cube)
	{
		if (!(cube == null) && cells.ContainsKey(pos))
		{
			IntVector key = new IntVector(pos.x, pos.y, pos.z);
			key.y++;
			if (cells.ContainsKey(key))
			{
				Cube neighborCube = cells[key].cube;
				SimpleFaceVisibilityTest(FaceFlags.Top, FaceFlags.Bottom, ref cube, ref neighborCube);
			}
			key.y -= 2;
			if (cells.ContainsKey(key))
			{
				Cube neighborCube = cells[key].cube;
				SimpleFaceVisibilityTest(FaceFlags.Bottom, FaceFlags.Top, ref cube, ref neighborCube);
			}
			key.y++;
			key.z++;
			if (cells.ContainsKey(key))
			{
				Cube neighborCube = cells[key].cube;
				SimpleFaceVisibilityTest(FaceFlags.Back, FaceFlags.Front, ref cube, ref neighborCube);
			}
			key.z -= 2;
			if (cells.ContainsKey(key))
			{
				Cube neighborCube = cells[key].cube;
				SimpleFaceVisibilityTest(FaceFlags.Front, FaceFlags.Back, ref cube, ref neighborCube);
			}
			key.z++;
			key.x++;
			if (cells.ContainsKey(key))
			{
				Cube neighborCube = cells[key].cube;
				SimpleFaceVisibilityTest(FaceFlags.Right, FaceFlags.Left, ref cube, ref neighborCube);
			}
			key.x -= 2;
			if (cells.ContainsKey(key))
			{
				Cube neighborCube = cells[key].cube;
				SimpleFaceVisibilityTest(FaceFlags.Left, FaceFlags.Right, ref cube, ref neighborCube);
			}
			key.x++;
		}
	}

	private static void SimpleFaceVisibilityTest(FaceFlags faceFlagCube, FaceFlags faceFlagOpposite, ref Cube cube, ref Cube neighborCube)
	{
		if (!(neighborCube == null) && ((uint)cube.HiddenSides & (uint)faceFlagCube) == 0)
		{
			if (((uint)cube.UnIndentedSides & (uint)faceFlagCube) != 0 && ((uint)neighborCube.UnIndentedSides & (uint)faceFlagOpposite) != 0)
			{
				cube.HiddenSides |= (byte)faceFlagCube;
				neighborCube.HiddenSides |= (byte)faceFlagOpposite;
			}
			else
			{
				AdvancedFaceVisibilityTest(faceFlagCube, faceFlagOpposite, ref cube, ref neighborCube);
			}
		}
	}

	private static bool AllFaceCornersIsTouchingCubeBorder(Face face, ref Vector3[] faceIndices)
	{
		int index = -1;
		float num = 0.5f;
		switch (face)
		{
		case Face.Top:
			index = 1;
			num = 0.5f;
			break;
		case Face.Bottom:
			index = 1;
			num = -0.5f;
			break;
		case Face.Front:
			index = 2;
			num = -0.5f;
			break;
		case Face.Back:
			index = 2;
			num = 0.5f;
			break;
		case Face.Left:
			index = 0;
			num = -0.5f;
			break;
		case Face.Right:
			index = 0;
			num = 0.5f;
			break;
		}
		Vector3[] array = faceIndices;
		foreach (Vector3 vector in array)
		{
			if (vector[index] != num)
			{
				return false;
			}
		}
		return true;
	}

	private static void AdvancedFaceVisibilityTest(FaceFlags faceFlagCube, FaceFlags faceFlagOpposite, ref Cube cube, ref Cube neighborCube)
	{
		Face face = CubeBase.FaceFlagToFace(faceFlagCube);
		Vector3[] faceIndices = Cube.GetFace(cube.Corners, face);
		if (!AllFaceCornersIsTouchingCubeBorder(face, ref faceIndices))
		{
			return;
		}
		Face face2 = CubeBase.FaceFlagToFace(faceFlagOpposite);
		Vector3[] faceIndices2 = Cube.GetFace(neighborCube.Corners, face2);
		if (!AllFaceCornersIsTouchingCubeBorder(face2, ref faceIndices2))
		{
			return;
		}
		switch (face)
		{
		case Face.Top:
		case Face.Bottom:
		{
			for (int i = 0; i < 4; i++)
			{
				if (faceIndices[i].x != faceIndices2[3 - i].x || faceIndices[i].z != faceIndices2[3 - i].z)
				{
					return;
				}
			}
			cube.HiddenSides |= (byte)faceFlagCube;
			neighborCube.HiddenSides |= (byte)faceFlagOpposite;
			break;
		}
		case Face.Left:
		case Face.Right:
			if (faceIndices[0].z == faceIndices2[1].z && faceIndices[0].y == faceIndices2[1].y && faceIndices[1].z == faceIndices2[0].z && faceIndices[1].y == faceIndices2[0].y && faceIndices[2].z == faceIndices2[3].z && faceIndices[2].y == faceIndices2[3].y && faceIndices[3].z == faceIndices2[2].z && faceIndices[3].y == faceIndices2[2].y)
			{
				cube.HiddenSides |= (byte)faceFlagCube;
				neighborCube.HiddenSides |= (byte)faceFlagOpposite;
			}
			break;
		case Face.Front:
		case Face.Back:
			if (faceIndices[0].x == faceIndices2[1].x && faceIndices[0].y == faceIndices2[1].y && faceIndices[1].x == faceIndices2[0].x && faceIndices[1].y == faceIndices2[0].y && faceIndices[2].x == faceIndices2[3].x && faceIndices[2].y == faceIndices2[3].y && faceIndices[3].x == faceIndices2[2].x && faceIndices[3].y == faceIndices2[2].y)
			{
				cube.HiddenSides |= (byte)faceFlagCube;
				neighborCube.HiddenSides |= (byte)faceFlagOpposite;
			}
			break;
		}
	}

	private static void GetMeshBounds(ref Bounds bounds, Dictionary<IntVector, Cell> cells, Vector3 scale)
	{
		Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		foreach (KeyValuePair<IntVector, Cell> cell in cells)
		{
			if (cell.Value.cube.HiddenSides != 63)
			{
				Vector3 vector = Vector3.one / 2f;
				Vector3 vector2 = cell.Key.ToVector3() + -vector;
				Vector3 vector3 = cell.Key.ToVector3() + vector;
				min.x = ((!(vector2.x < min.x)) ? min.x : vector2.x);
				min.y = ((!(vector2.y < min.y)) ? min.y : vector2.y);
				min.z = ((!(vector2.z < min.z)) ? min.z : vector2.z);
				max.x = ((!(vector3.x < max.x)) ? vector3.x : max.x);
				max.y = ((!(vector3.y < max.y)) ? vector3.y : max.y);
				max.z = ((!(vector3.z < max.z)) ? vector3.z : max.z);
				min.x = ((!(vector3.x < min.x)) ? min.x : vector3.x);
				min.y = ((!(vector3.y < min.y)) ? min.y : vector3.y);
				min.z = ((!(vector3.z < min.z)) ? min.z : vector3.z);
				max.x = ((!(vector2.x < max.x)) ? vector2.x : max.x);
				max.y = ((!(vector2.y < max.y)) ? vector2.y : max.y);
				max.z = ((!(vector2.z < max.z)) ? vector2.z : max.z);
			}
		}
		bounds.SetMinMax(min, max);
	}

	private static int RebuildMesh(Dictionary<IntVector, Cell> cells, Vector3 scale)
	{
		MeshDataPool.Reset();
		int num = 0;
		foreach (KeyValuePair<IntVector, Cell> cell in cells)
		{
			if (cell.Value.cube.HiddenSides == 63)
			{
				continue;
			}
			int index = 0;
			Cube.GetVisibleFaceVertices(cell.Value.cube, ref faceData, cell.Key, cells, ref index);
			for (int i = 0; i < index; i++)
			{
				int num2 = CubeBase.GetMaterial(cell.Value.cube, faceData[i].face);
				if (num2 < 0 || num2 >= 60)
				{
					num2 = 24;
				}
				for (int j = 0; j < 4; j++)
				{
					faceData[i].colors[j].g = TextureAtlas.UV[num2].position.x;
					faceData[i].colors[j].b = TextureAtlas.UV[num2].position.y;
					MeshDataPool.AddVertex(faceData[i].faceVertices[j]);
					MeshDataPool.AddColor(faceData[i].colors[j]);
				}
				MeshDataPool.AddUvRange(GetFaceUvs(faceData[i].faceVertices, faceData[i].face, scale));
				int num3 = num * 4;
				MeshDataPool.AddIndex(num3);
				MeshDataPool.AddIndex(num3 + 3);
				MeshDataPool.AddIndex(num3 + 2);
				MeshDataPool.AddIndex(num3 + 2);
				MeshDataPool.AddIndex(num3 + 1);
				MeshDataPool.AddIndex(num3);
				num++;
			}
		}
		return num * 2;
	}

	private static Vector2[] GetFaceUvs(Vector3[] faceVertices, Face face, Vector3 scale)
	{
		if (scale.x != scale.y || scale.x != scale.z)
		{
			Debug.LogError("algorithm does not support non uniform scale");
		}
		uvOffsetVector = uvOffsetVector0;
		switch (face)
		{
		case Face.Top:
			uvs[0].Set(faceVertices[0].x, faceVertices[0].z);
			uvs[1].Set(faceVertices[1].x, faceVertices[1].z);
			uvs[2].Set(faceVertices[2].x, faceVertices[2].z);
			uvs[3].Set(faceVertices[3].x, faceVertices[3].z);
			break;
		case Face.Bottom:
			uvs[0].Set(0f - faceVertices[0].x, faceVertices[0].z);
			uvs[1].Set(0f - faceVertices[1].x, faceVertices[1].z);
			uvs[2].Set(0f - faceVertices[2].x, faceVertices[2].z);
			uvs[3].Set(0f - faceVertices[3].x, faceVertices[3].z);
			uvOffsetVector = uvOffsetVector1;
			break;
		case Face.Back:
			uvs[0].Set(0f - faceVertices[0].x, faceVertices[0].y);
			uvs[1].Set(0f - faceVertices[1].x, faceVertices[1].y);
			uvs[2].Set(0f - faceVertices[2].x, faceVertices[2].y);
			uvs[3].Set(0f - faceVertices[3].x, faceVertices[3].y);
			uvOffsetVector = uvOffsetVector1;
			break;
		case Face.Front:
			uvs[0].Set(faceVertices[0].x, faceVertices[0].y);
			uvs[1].Set(faceVertices[1].x, faceVertices[1].y);
			uvs[2].Set(faceVertices[2].x, faceVertices[2].y);
			uvs[3].Set(faceVertices[3].x, faceVertices[3].y);
			break;
		case Face.Left:
			uvs[0].Set(0f - faceVertices[0].z, faceVertices[0].y);
			uvs[1].Set(0f - faceVertices[1].z, faceVertices[1].y);
			uvs[2].Set(0f - faceVertices[2].z, faceVertices[2].y);
			uvs[3].Set(0f - faceVertices[3].z, faceVertices[3].y);
			uvOffsetVector = uvOffsetVector1;
			break;
		case Face.Right:
			uvs[0].Set(faceVertices[0].z, faceVertices[0].y);
			uvs[1].Set(faceVertices[1].z, faceVertices[1].y);
			uvs[2].Set(faceVertices[2].z, faceVertices[2].y);
			uvs[3].Set(faceVertices[3].z, faceVertices[3].y);
			break;
		}
		float num = 2f / scale[0];
		uvs[0] += uvOffsetVector;
		uvs[1] += uvOffsetVector;
		uvs[2] += uvOffsetVector;
		uvs[3] += uvOffsetVector;
		uvs[0] /= num;
		uvs[1] /= num;
		uvs[2] /= num;
		uvs[3] /= num;
		return uvs;
	}
}
