using UnityEngine;

public class UXSelectionBoxOld : MonoBehaviour
{
	public SelectionBoxSpec spec;

	private int verticesPerCorner;

	private int trianglesPerCorner;

	private void Awake()
	{
		verticesPerCorner = 7 + spec.cornerSteps;
		trianglesPerCorner = 4 + spec.cornerSteps + 1;
		((Component)this).GetComponent<MeshFilter>().mesh = BuildMesh();
	}

	private Mesh BuildMesh()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Mesh val = new Mesh();
		BuildCorner(out var vertices, out var triangles);
		Vector3[] array = new Vector3[verticesPerCorner * 4];
		int[] array2 = new int[trianglesPerCorner * 3 * 4];
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 4; i++)
		{
			Quaternion val2 = Quaternion.Euler(0f, 0f, 90f * (float)i);
			for (int j = 0; j < vertices.Length; j++)
			{
				ref Vector3 reference = ref array[num + j];
				reference = val2 * vertices[j];
			}
			for (int k = 0; k < triangles.Length; k++)
			{
				array2[num2 + k] = triangles[k] + num;
			}
			num += verticesPerCorner;
			num2 += trianglesPerCorner * 3;
		}
		val.vertices = array;
		val.triangles = array2;
		val.RecalculateBounds();
		val.RecalculateNormals();
		return val;
	}

	private void BuildCorner(out Vector3[] vertices, out int[] triangles)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		int num = 7 + spec.cornerSteps;
		int num2 = 4 + spec.cornerSteps + 1;
		vertices = new Vector3[num];
		triangles = new int[3 * num2];
		float num3 = spec.width * 0.5f;
		float num4 = spec.height * 0.5f;
		ref Vector3 reference = ref vertices[0];
		reference = new Vector3(num3 - spec.cornerSize, num4 - spec.lineWidth, 0f);
		ref Vector3 reference2 = ref vertices[1];
		reference2 = new Vector3(num3 - spec.cornerSize, num4, 0f);
		ref Vector3 reference3 = ref vertices[2];
		reference3 = new Vector3(num3 - spec.lineWidth, num4 - spec.lineWidth, 0f);
		ref Vector3 reference4 = ref vertices[3];
		reference4 = new Vector3(num3 - spec.lineWidth, num4, 0f);
		for (int i = 0; i < spec.cornerSteps; i++)
		{
			ref Vector3 reference5 = ref vertices[4 + i];
			reference5 = new Vector3(num3 - spec.lineWidth, num4 - spec.lineWidth, 0f) + Quaternion.AngleAxis((float)(i + 1) * (90f / (float)spec.cornerSteps), -Vector3.forward) * new Vector3(0f, spec.lineWidth, 0f);
		}
		ref Vector3 reference6 = ref vertices[4 + spec.cornerSteps];
		reference6 = new Vector3(num3, num4 - spec.lineWidth, 0f);
		ref Vector3 reference7 = ref vertices[4 + spec.cornerSteps + 1];
		reference7 = new Vector3(num3, num4 - spec.cornerSize, 0f);
		ref Vector3 reference8 = ref vertices[4 + spec.cornerSteps + 2];
		reference8 = new Vector3(num3 - spec.lineWidth, num4 - spec.cornerSize, 0f);
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		triangles[3] = 1;
		triangles[4] = 3;
		triangles[5] = 2;
		for (int j = 0; j < spec.cornerSteps + 1; j++)
		{
			triangles[(2 + j) * 3] = 2;
			triangles[(2 + j) * 3 + 1] = 3 + j;
			triangles[(2 + j) * 3 + 2] = 3 + j + 1;
		}
		triangles[(spec.cornerSteps + 3) * 3] = 2;
		triangles[(spec.cornerSteps + 3) * 3 + 1] = 4 + spec.cornerSteps;
		triangles[(spec.cornerSteps + 3) * 3 + 2] = 4 + spec.cornerSteps + 1;
		triangles[(spec.cornerSteps + 3 + 1) * 3] = 2;
		triangles[(spec.cornerSteps + 3 + 1) * 3 + 1] = 4 + spec.cornerSteps + 1;
		triangles[(spec.cornerSteps + 3 + 1) * 3 + 2] = 4 + spec.cornerSteps + 2;
	}
}
