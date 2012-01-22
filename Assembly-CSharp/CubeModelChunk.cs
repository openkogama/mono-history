using System;
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

	internal struct MipMeshBookkeeping(List<Cube> cubes)
	{
		private List<Cube> cubes = cubes;

		public int CubesCount => cubes.Count;

		public void AddCube(Cube cube)
		{
			cubes.Add(cube);
		}

		public int GetDominantCubeMaterial(Dictionary<int, int> materialCounts)
		{
			materialCounts.Clear();
			foreach (Cube cube in cubes)
			{
				int material = CubeBase.GetMaterial(cube, Face.Top);
				if (!materialCounts.ContainsKey(material))
				{
					materialCounts.Add(material, 0);
				}
				materialCounts[material]++;
			}
			int result = -1;
			int num = 0;
			foreach (KeyValuePair<int, int> materialCount in materialCounts)
			{
				if (materialCount.Value > num)
				{
					result = materialCount.Key;
					num = materialCount.Value;
				}
			}
			return result;
		}
	}

	public static readonly bool UseACShadows = false;

	private static int chunkSize = 32;

	private IntVector chunkPos;

	private List<GameObject> instances = new List<GameObject>();

	private SharedMeshData[] meshes = new SharedMeshData[3];

	private string name;

	private int cubeCount;

	private Cells cells;

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

	private static float bookKeepingFloat = 0f;

	private static Vector2 cubePosOffset = new Vector2(0f, 0f);

	private static Dictionary<int, int> materialCounts = new Dictionary<int, int>();

	private static Dictionary<IntVector, MipMeshBookkeeping> mipMeshBookkeeping = new Dictionary<IntVector, MipMeshBookkeeping>();

	private static IntVector intVectorBookkeeping = default;

	public static int ChunkSize => chunkSize;

	public int CubeCount => cubeCount;

	public CubeModelChunk(IntVector iVector)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected Obj, but got Unknown
		name = "chunk" + iVector.x + "." + iVector.y + "." + iVector.z;
		chunkPos = iVector;
		cells = new Cells(chunkPos, ChunkSize);
		for (int i = 0; i < meshes.Length; i++)
		{
			ref SharedMeshData reference = ref meshes[i];
			reference = new SharedMeshData(new Mesh());
		}
	}

	static CubeModelChunk()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
	}

	public CubeModelChunk CloneGeometry(Vector3 scale)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		CubeModelChunk cubeModelChunk = new CubeModelChunk(chunkPos);
		cubeModelChunk.cells = cells.Clone();
		cubeModelChunk.cubeCount = cubeCount;
		cubeModelChunk.RebuildChunk(scale);
		cubeModelChunk.RebuildMipMapMesh(scale);
		return cubeModelChunk;
	}

	public bool CompareGeometry(CubeModelChunk chunk)
	{
		for (int i = 0; i < chunkSize; i++)
		{
			for (int j = 0; j < chunkSize; j++)
			{
				for (int k = 0; k < chunkSize; k++)
				{
					if ((chunk.cells[i, j, k].cube == null && cells[i, j, k].cube != null) || (chunk.cells[i, j, k].cube != null && cells[i, j, k].cube == null))
					{
						return false;
					}
					if (chunk.cells[i, j, k].cube != cells[i, j, k].cube)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public Cube GetCube(IntVector iVector)
	{
		return cells.GetCell(iVector).cube;
	}

	public bool ContainsCube(IntVector iVector)
	{
		return !(cells.GetCell(iVector).cube == null);
	}

	public void AddToChunk(IntVector iVector, Cube cube, bool setVisibility = true)
	{
		if (!cells.ContainsCube(iVector))
		{
			cubeCount++;
		}
		cells.SetCube(iVector, cube);
		if (setVisibility)
		{
			SetCubeVisibilityWithNeighbors(cells.GetArrayCoords(iVector));
		}
	}

	public IntVector GetFirstSolidCubePos()
	{
		for (int i = 0; i < ChunkSize; i++)
		{
			for (int j = 0; j < ChunkSize; j++)
			{
				for (int k = 0; k < ChunkSize; k++)
				{
					IntVector worldCoords = cells.GetWorldCoords(new IntVector((short)i, (short)j, (short)k));
					if (cells.ContainsCube(worldCoords))
					{
						return worldCoords;
					}
				}
			}
		}
		Debug.LogError((object)"No cube found in chunk. This is a problem");
		return IntVector.One;
	}

	public void RemoveFromChunk(IntVector iVector)
	{
		if (cells.ContainsCube(iVector))
		{
			cells.RemoveCube(iVector);
			SetCubeVisibilityWithNeighbors(cells.GetArrayCoords(iVector));
			cubeCount--;
		}
	}

	public void Destroy()
	{
		foreach (GameObject instance in instances)
		{
			Object.Destroy((Object)(object)instance);
		}
	}

	private static void CalculateLights(Cells cells)
	{
		Cell[,,] cellsArray = cells.CellsArray;
		byte b = 0;
		for (int i = 0; i < cells.ChunkSize; i++)
		{
			for (int j = 0; j < cells.ChunkSize; j++)
			{
				for (int k = 0; k < cells.ChunkSize; k++)
				{
					b = (byte)((cellsArray[i, j, k].cube == null) ? byte.MaxValue : 0);
					Cell cell = cellsArray[i, j, k];
					cell.lightValue = b;
					cellsArray[i, j, k] = cell;
				}
			}
		}
	}

	public void RebuildChunk(Vector3 scale)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		DateTime now = DateTime.Now;
		if (UseACShadows)
		{
			CalculateLights(cells);
		}
		MeshData meshData = new MeshData();
		RebuildMesh(ref meshData, cells, scale);
		meshData.SetToMesh(ref meshes[0].mesh, ref meshes[0].materials);
		UpdateInstances();
	}

	public void RebuildMipMapMesh(Vector3 scale)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected Obj, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected Obj, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		MeshData meshData = new MeshData();
		if ((Object)(object)meshes[1].mesh == (Object)null)
		{
			meshes[1].mesh = new Mesh();
		}
		if ((Object)(object)meshes[2].mesh == (Object)null)
		{
			meshes[2].mesh = new Mesh();
		}
		int power = 2;
		if (GetMipMeshCells(cells, 2, out var cellsMipmap))
		{
			SetCubeVisibility(cellsMipmap);
			CalculateLights(cellsMipmap);
			RebuildMesh(ref meshData, cellsMipmap, scale, power);
			meshData.SetToMesh(ref meshes[1].mesh, ref meshes[1].materials);
		}
		if (GetMipMeshCells(cellsMipmap, 2, out var cellsMipmap2))
		{
			SetCubeVisibility(cellsMipmap2);
			CalculateLights(cellsMipmap2);
			RebuildMesh(ref meshData, cellsMipmap2, scale, 4);
			meshData.SetToMesh(ref meshes[2].mesh, ref meshes[2].materials);
		}
	}

	public SharedMeshData GetMeshData(MeshSetting mipMesh)
	{
		return meshes[(int)mipMesh];
	}

	private void UpdateInstances()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		List<int> list = new List<int>();
		for (int i = 0; i < instances.Count; i++)
		{
			if ((Object)(object)instances[i] != (Object)null)
			{
				MeshRenderer component = instances[i].GetComponent<MeshRenderer>();
				((Renderer)component).sharedMaterials = meshes[0].materials;
				MVGameController.Instance.WOCM.UpdateWorldBounds(((Renderer)component).bounds);
			}
			else
			{
				list.Add(i);
			}
		}
		foreach (int item in list)
		{
			instances.RemoveAt(item);
		}
		foreach (GameObject instance in instances)
		{
			BoxCollider component2 = instance.GetComponent<BoxCollider>();
			if ((Object)(object)component2 != (Object)null)
			{
				Bounds bounds = meshes[0].mesh.bounds;
				component2.size = bounds.size;
				component2.center = bounds.center;
			}
			else
			{
				instance.AddComponent<BoxCollider>();
			}
		}
	}

	public void SetInstanceDataRef(IntVector chunkPos, MVCubeModelBase cubeInstance)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		MeshFilter val2 = val.AddComponent<MeshFilter>();
		MeshRenderer val3 = val.AddComponent<MeshRenderer>();
		val2.sharedMesh = meshes[0].mesh;
		((Renderer)val3).sharedMaterials = meshes[0].materials;
		BoxCollider component = val.GetComponent<BoxCollider>();
		if ((Object)(object)component != (Object)null)
		{
			Bounds bounds = val2.sharedMesh.bounds;
			component.size = bounds.size;
			component.center = bounds.center;
		}
		else
		{
			val.AddComponent<BoxCollider>();
		}
		val.transform.parent = cubeInstance.GameObject.transform;
		val.transform.localPosition = Vector3.zero;
		val.transform.localRotation = Quaternion.identity;
		val.transform.localScale = Vector3.one;
		val.layer = cubeInstance.GameObject.layer;
		instances.Add(val);
		cubeInstance.chunkInstances.Add(chunkPos, val);
	}

	private void SetCubeVisibilityWithNeighbors(IntVector pos)
	{
		IntVector cubeVisibility = new IntVector(pos.x, pos.y, pos.z);
		Cube cube = cells[pos].cube;
		if (cube != null)
		{
			CubeBase.SetCubeFlags(cube);
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
		if (cells.IsWithinArrayCoordsRange(iVector))
		{
			Cube cube = cells[iVector].cube;
			if (cube != null)
			{
				cube.HiddenSides = 0;
				SetCubeVisibility(cells, iVector, cube);
			}
		}
	}

	public void SetCubeVisibility()
	{
		SetCubeVisibility(cells);
	}

	private static void SetCubeVisibility(Cells cells)
	{
		for (int i = 0; i < cells.ChunkSize; i++)
		{
			for (int j = 0; j < cells.ChunkSize; j++)
			{
				for (int k = 0; k < cells.ChunkSize; k++)
				{
					SetCubeVisibility(cells, new IntVector((short)i, (short)j, (short)k), cells[i, j, k].cube);
				}
			}
		}
	}

	private static void SetCubeVisibility(Cells cells, IntVector pos, Cube cube)
	{
		if (!(cube == null))
		{
			IntVector intVector = new IntVector(pos.x, pos.y, pos.z);
			intVector.y++;
			if (cells.IsWithinArrayCoordsRange(intVector))
			{
				Cube neighborCube = cells[intVector].cube;
				SimpleFaceVisibilityTest(FaceFlags.Top, FaceFlags.Bottom, ref cube, ref neighborCube);
			}
			intVector.y -= 2;
			if (cells.IsWithinArrayCoordsRange(intVector))
			{
				Cube neighborCube = cells[intVector].cube;
				SimpleFaceVisibilityTest(FaceFlags.Bottom, FaceFlags.Top, ref cube, ref neighborCube);
			}
			intVector.y++;
			intVector.z++;
			if (cells.IsWithinArrayCoordsRange(intVector))
			{
				Cube neighborCube = cells[intVector].cube;
				SimpleFaceVisibilityTest(FaceFlags.Back, FaceFlags.Front, ref cube, ref neighborCube);
			}
			intVector.z -= 2;
			if (cells.IsWithinArrayCoordsRange(intVector))
			{
				Cube neighborCube = cells[intVector].cube;
				SimpleFaceVisibilityTest(FaceFlags.Front, FaceFlags.Back, ref cube, ref neighborCube);
			}
			intVector.z++;
			intVector.x++;
			if (cells.IsWithinArrayCoordsRange(intVector))
			{
				Cube neighborCube = cells[intVector].cube;
				SimpleFaceVisibilityTest(FaceFlags.Right, FaceFlags.Left, ref cube, ref neighborCube);
			}
			intVector.x -= 2;
			if (cells.IsWithinArrayCoordsRange(intVector))
			{
				Cube neighborCube = cells[intVector].cube;
				SimpleFaceVisibilityTest(FaceFlags.Left, FaceFlags.Right, ref cube, ref neighborCube);
			}
			intVector.x++;
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
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		float num2 = 0.5f;
		switch (face)
		{
		case Face.Top:
			num = 1;
			num2 = 0.5f;
			break;
		case Face.Bottom:
			num = 1;
			num2 = -0.5f;
			break;
		case Face.Front:
			num = 2;
			num2 = -0.5f;
			break;
		case Face.Back:
			num = 2;
			num2 = 0.5f;
			break;
		case Face.Left:
			num = 0;
			num2 = -0.5f;
			break;
		case Face.Right:
			num = 0;
			num2 = 0.5f;
			break;
		}
		Vector3[] array = faceIndices;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 val = array[i];
			if (val[num] != num2)
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

	private static void RebuildMesh(ref MeshData meshData, Cells cells, Vector3 scale, int power = 1)
	{
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		meshData.vertices = new List<Vector3>();
		meshData.uv = new List<Vector2>();
		meshData.colors = new List<Color>();
		int num = 0;
		meshData.materials = new List<Material>();
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		Cell[,,] cellsArray = cells.CellsArray;
		for (int i = 0; i < cells.ChunkSize; i++)
		{
			for (int j = 0; j < cells.ChunkSize; j++)
			{
				for (int k = 0; k < cells.ChunkSize; k++)
				{
					if (cellsArray[i, j, k].cube == null || cellsArray[i, j, k].cube.HiddenSides == 63)
					{
						continue;
					}
					IntVector intVector = new IntVector((short)i, (short)j, (short)k);
					IntVector worldCoords = cells.GetWorldCoords(intVector);
					int index = 0;
					Cube.GetVisibleFaceVertices(cellsArray[i, j, k].cube, ref faceData, worldCoords, intVector, cells, ref index);
					if (power != 1)
					{
						for (int l = 0; l < index; l++)
						{
							for (int m = 0; m < faceData[l].faceVertices.Length; m++)
							{
								ref Vector3 reference = ref faceData[l].faceVertices[m];
								reference *= (float)power;
							}
						}
					}
					for (int n = 0; n < index; n++)
					{
						for (int num2 = 0; num2 < 4; num2++)
						{
							meshData.vertices.Add(faceData[n].faceVertices[num2]);
							meshData.colors.Add(faceData[n].colors[num2]);
						}
						meshData.uv.AddRange(GetFaceUvs(faceData[n].faceVertices, faceData[n].face, scale));
						byte material = CubeBase.GetMaterial(cells[i, j, k].cube, faceData[n].face);
						Material material2 = MVGameController.Instance.WOCM.MaterialRepository.GetMaterial(material).material;
						if (!dictionary.ContainsKey(material))
						{
							meshData.materials.Add(material2);
							meshData.subMeshTriangles.Add(new List<int>());
							dictionary.Add(material, meshData.materials.Count - 1);
						}
						int index2 = dictionary[material];
						meshData.subMeshTriangles[index2].Add(num * 4);
						meshData.subMeshTriangles[index2].Add(num * 4 + 3);
						meshData.subMeshTriangles[index2].Add(num * 4 + 2);
						meshData.subMeshTriangles[index2].Add(num * 4 + 2);
						meshData.subMeshTriangles[index2].Add(num * 4 + 1);
						meshData.subMeshTriangles[index2].Add(num * 4);
						num++;
					}
				}
			}
		}
	}

	private static Vector2[] GetFaceUvs(Vector3[] faceVertices, Face face, Vector3 scale)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		bookKeepingFloat = 0f;
		if (scale.x != scale.y || scale.x != scale.z)
		{
			Debug.LogError((object)"algorithm does not support non uniform scale");
		}
		uvOffsetVector = uvOffsetVector0;
		switch (face)
		{
		case Face.Top:
			MathFunctions.Vector3ToVector2(ref faceVertices[0], ref uvs[0], 1);
			MathFunctions.Vector3ToVector2(ref faceVertices[1], ref uvs[1], 1);
			MathFunctions.Vector3ToVector2(ref faceVertices[2], ref uvs[2], 1);
			MathFunctions.Vector3ToVector2(ref faceVertices[3], ref uvs[3], 1);
			break;
		case Face.Bottom:
		{
			MathFunctions.Vector3ToVector2(ref faceVertices[0], ref uvs[0], 1);
			MathFunctions.Vector3ToVector2(ref faceVertices[1], ref uvs[1], 1);
			MathFunctions.Vector3ToVector2(ref faceVertices[2], ref uvs[2], 1);
			MathFunctions.Vector3ToVector2(ref faceVertices[3], ref uvs[3], 1);
			for (int l = 0; l < 4; l++)
			{
				uvs[l].x = 0f - uvs[l].x;
			}
			uvOffsetVector = uvOffsetVector1;
			break;
		}
		case Face.Back:
		{
			MathFunctions.Vector3ToVector2(ref faceVertices[0], ref uvs[0], 2);
			MathFunctions.Vector3ToVector2(ref faceVertices[1], ref uvs[1], 2);
			MathFunctions.Vector3ToVector2(ref faceVertices[2], ref uvs[2], 2);
			MathFunctions.Vector3ToVector2(ref faceVertices[3], ref uvs[3], 2);
			for (int j = 0; j < 4; j++)
			{
				uvs[j].x = 0f - uvs[j].x;
			}
			uvOffsetVector = uvOffsetVector1;
			break;
		}
		case Face.Front:
			MathFunctions.Vector3ToVector2(ref faceVertices[0], ref uvs[0], 2);
			MathFunctions.Vector3ToVector2(ref faceVertices[1], ref uvs[1], 2);
			MathFunctions.Vector3ToVector2(ref faceVertices[2], ref uvs[2], 2);
			MathFunctions.Vector3ToVector2(ref faceVertices[3], ref uvs[3], 2);
			break;
		case Face.Left:
		{
			MathFunctions.Vector3ToVector2(ref faceVertices[0], ref uvs[0], 0);
			MathFunctions.Vector3ToVector2(ref faceVertices[1], ref uvs[1], 0);
			MathFunctions.Vector3ToVector2(ref faceVertices[2], ref uvs[2], 0);
			MathFunctions.Vector3ToVector2(ref faceVertices[3], ref uvs[3], 0);
			for (int k = 0; k < 4; k++)
			{
				bookKeepingFloat = uvs[k].x;
				uvs[k].x = uvs[k].y;
				uvs[k].y = bookKeepingFloat;
				uvs[k].x = 0f - uvs[k].x;
			}
			uvOffsetVector = uvOffsetVector1;
			break;
		}
		case Face.Right:
		{
			MathFunctions.Vector3ToVector2(ref faceVertices[0], ref uvs[0], 0);
			MathFunctions.Vector3ToVector2(ref faceVertices[1], ref uvs[1], 0);
			MathFunctions.Vector3ToVector2(ref faceVertices[2], ref uvs[2], 0);
			MathFunctions.Vector3ToVector2(ref faceVertices[3], ref uvs[3], 0);
			for (int i = 0; i < 4; i++)
			{
				bookKeepingFloat = uvs[i].x;
				uvs[i].x = uvs[i].y;
				uvs[i].y = bookKeepingFloat;
			}
			break;
		}
		}
		float num = 2f / scale[0];
		for (int m = 0; m < 4; m++)
		{
			ref Vector2 reference = ref uvs[m];
			reference += uvOffsetVector;
			ref Vector2 reference2 = ref uvs[m];
			reference2 /= num;
		}
		return uvs;
	}

	private static bool GetMipMeshCells(Cells cells, int gridPower, out Cells cellsMipmap)
	{
		cellsMipmap = new Cells(cells.ChunkPos, cells.ChunkSize / gridPower);
		if (!Mathf.IsPowerOfTwo(gridPower))
		{
			Debug.LogError((object)"gridPower must be power of 2!");
			return false;
		}
		mipMeshBookkeeping.Clear();
		int num = gridPower * gridPower * gridPower / 3;
		for (int i = 0; i < cells.ChunkSize; i++)
		{
			for (int j = 0; j < cells.ChunkSize; j++)
			{
				for (int k = 0; k < cells.ChunkSize; k++)
				{
					intVectorBookkeeping.x = (short)i;
					intVectorBookkeeping.y = (short)j;
					intVectorBookkeeping.z = (short)k;
					if (!(cells[i, j, k].cube == null))
					{
						intVectorBookkeeping /= gridPower;
						if (!mipMeshBookkeeping.ContainsKey(intVectorBookkeeping))
						{
							mipMeshBookkeeping.Add(intVectorBookkeeping, new MipMeshBookkeeping(new List<Cube>()));
						}
						mipMeshBookkeeping[intVectorBookkeeping].AddCube(cells[i, j, k].cube);
					}
				}
			}
		}
		foreach (KeyValuePair<IntVector, MipMeshBookkeeping> item in mipMeshBookkeeping)
		{
			if (item.Value.CubesCount >= num)
			{
				cellsMipmap[item.Key] = new Cell(new Cube(CubeBase.IdentityByteCorners, Cube.CreateMaterialArray((byte)item.Value.GetDominantCubeMaterial(materialCounts))), 0);
			}
		}
		return true;
	}
}
