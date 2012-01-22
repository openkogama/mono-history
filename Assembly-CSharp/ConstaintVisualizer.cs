using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class ConstaintVisualizer : MonoBehaviour
{
	private Vector3[] corners = new Vector3[8]
	{
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, -0.5f)
	};

	public ConstaintVisualizer()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Init(MVCubeModelBase targetCubeModel, string layer = "UIItems")
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).gameObject.layer = LayerMask.NameToLayer(layer);
		CreateInsideOutCube();
		((Component)this).transform.position = GetPosition(targetCubeModel);
		((Component)this).transform.rotation = targetCubeModel.GameObject.transform.rotation;
		Vector3 localScale = new Vector3((float)SharedCubeFunctions.CubeConstraint.x, (float)SharedCubeFunctions.CubeConstraint.y, (float)SharedCubeFunctions.CubeConstraint.z);
		localScale.x *= targetCubeModel.GameObject.transform.localScale.x;
		localScale.y *= targetCubeModel.GameObject.transform.localScale.y;
		localScale.z *= targetCubeModel.GameObject.transform.localScale.z;
		((Component)this).transform.localScale = localScale;
	}

	private static Vector3 GetPosition(MVCubeModelBase targetCubeModel)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Bounds meshBounds = targetCubeModel.GetMeshBounds();
		Vector3 v = meshBounds.center;
		for (int i = 0; i < 3; i++)
		{
			Vector3 cubeConstraintVector = SharedCubeFunctions.CubeConstraintVector3;
			if (cubeConstraintVector[i] % 2f == 0f)
			{
				if (v[i] % 1f != 0f)
				{
					v[i] = Mathf.Ceil(v[i]) - 0.5f;
				}
				continue;
			}
			IntVector min = default;
			IntVector max = default;
			SharedCollisionFunctions.GetVoxelBounds(ref min, ref max, targetCubeModel.GetMeshBounds());
			short num = (max - min + new IntVector(1, 1, 1))[i];
			Vector3 cubeConstraintVector2 = SharedCubeFunctions.CubeConstraintVector3;
			if (num == (int)cubeConstraintVector2[i])
			{
				int num2 = min[i] + (max[i] - min[i]) / 2;
				v[i] = num2;
			}
		}
		Vector3 val = (SharedCubeFunctions.CubeConstraintVector3 - Vector3.one * 1f) / 2f;
		MathFunctions.ClampVector(ref v, -val, val);
		return targetCubeModel.GameObject.transform.TransformPoint(v);
	}

	public void UpdatePosition(MVCubeModelBase targetCubeModel)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = GetPosition(targetCubeModel);
	}

	private void CreateInsideOutCube()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected Obj, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		MeshFilter val = ((Component)this).gameObject.AddComponent<MeshFilter>();
		MeshRenderer val2 = ((Component)this).gameObject.AddComponent<MeshRenderer>();
		((Renderer)val2).material = (Material)Resources.Load("Materials/ModelConstraints");
		Mesh mesh = val.mesh;
		List<int> list = new List<int>();
		List<Vector2> list2 = new List<Vector2>();
		mesh.vertices = GetVertices(corners);
		for (int i = 0; i < 6; i++)
		{
			list.Add(i * 4 + 2);
			list.Add(i * 4 + 3);
			list.Add(i * 4);
			list.Add(i * 4);
			list.Add(i * 4 + 1);
			list.Add(i * 4 + 2);
			list2.Add(new Vector2(0f, 0f));
			list2.Add(new Vector2(1f, 0f));
			list2.Add(new Vector2(1f, 1f));
			list2.Add(new Vector2(0f, 1f));
		}
		mesh.uv = list2.ToArray();
		mesh.triangles = list.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}

	private Vector3[] GetVertices(Vector3[] corners)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		List<Vector3> list = new List<Vector3>(corners);
		list.Add(list[7]);
		list.Add(list[6]);
		list.Add(list[1]);
		list.Add(list[0]);
		list.Add(list[5]);
		list.Add(list[4]);
		list.Add(list[3]);
		list.Add(list[2]);
		list.Add(list[4]);
		list.Add(list[7]);
		list.Add(list[0]);
		list.Add(list[3]);
		list.Add(list[6]);
		list.Add(list[5]);
		list.Add(list[2]);
		list.Add(list[1]);
		return list.ToArray();
	}
}
