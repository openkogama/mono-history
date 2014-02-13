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

	private static int chunkSize = 32;

	private IntVector chunkPos;

	private List<GameObject> instances = new List<GameObject>();

	private SharedMeshData[] meshes = new SharedMeshData[3];

	private string name;

	private int cubeCount;

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

	private static float bookKeepingFloat = 0f;

	public static int ChunkSize => chunkSize;

	public int CubeCount => cubeCount;

	public CubeModelChunk(IntVector iVector)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected Obj, but got Unknown
		name = "chunk" + iVector.x + "." + iVector.y + "." + iVector.z;
		chunkPos = iVector;
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
	}

	public CubeModelChunk CloneGeometry(Vector3 scale)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
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
			cells.Add(iVector, new Cell(cube, byte.MaxValue));
		}
		else
		{
			Cell value = cells[iVector];
			value.cube = cube;
			cells[iVector] = value;
		}
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
		Debug.LogError((object)"No cube found in chunk. This is a problem");
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
		foreach (GameObject instance in instances)
		{
			Object.Destroy((Object)(object)instance);
		}
	}

	private static void CalculateLights(Dictionary<IntVector, Cell> cells)
	{
		Dictionary<IntVector, Cell> dictionary = new Dictionary<IntVector, Cell>();
		foreach (KeyValuePair<IntVector, Cell> cell in cells)
		{
			Cell value = cell.Value;
			value.lightValue = (byte)(((value.cube.UnIndentedSides & 0x3F) != 63) ? 255u : 0u);
			dictionary.Add(cell.Key, value);
		}
		foreach (KeyValuePair<IntVector, Cell> item in dictionary)
		{
			cells[item.Key] = item.Value;
		}
	}

	public void RebuildChunk(Vector3 scale)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (UseAOShadows)
		{
			CalculateLights(cells);
		}
		MeshData meshData = new MeshData();
		RebuildMesh(ref meshData, cells, scale);
		meshData.SetToMesh(ref meshes[0].mesh, ref meshes[0].materials);
		UpdateInstances();
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
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
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
		val.transform.parent = cubeInstance.Transform;
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

	private static void RebuildMesh(ref MeshData meshData, Dictionary<IntVector, Cell> cells, Vector3 scale, int power = 1)
	{
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		meshData.vertices = new List<Vector3>();
		meshData.uv = new List<Vector2>();
		meshData.colors = new List<Color>();
		int num = 0;
		meshData.materials = new List<Material>();
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (KeyValuePair<IntVector, Cell> cell in cells)
		{
			if (cell.Value.cube.HiddenSides == 63)
			{
				continue;
			}
			int index = 0;
			Cube.GetVisibleFaceVertices(cell.Value.cube, ref faceData, cell.Key, cells, ref index);
			if (power != 1)
			{
				for (int i = 0; i < index; i++)
				{
					for (int j = 0; j < faceData[i].faceVertices.Length; j++)
					{
						ref Vector3 reference = ref faceData[i].faceVertices[j];
						reference *= (float)power;
					}
				}
			}
			for (int k = 0; k < index; k++)
			{
				for (int l = 0; l < 4; l++)
				{
					meshData.vertices.Add(faceData[k].faceVertices[l]);
					meshData.colors.Add(faceData[k].colors[l]);
				}
				meshData.uv.AddRange(GetFaceUvs(faceData[k].faceVertices, faceData[k].face, scale));
				byte material = CubeBase.GetMaterial(cell.Value.cube, faceData[k].face);
				Material material2 = MVGameController.Instance.Game.MaterialRepository.GetMaterial(material).material;
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
}
