using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class ObjExporterScript
{
	public static string MeshToString(MeshFilter mf)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		Mesh mesh = mf.mesh;
		Material[] sharedMaterials = ((Component)mf).renderer.sharedMaterials;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("g ").Append(((Object)mf).name).Append("\n");
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 val = vertices[i];
			stringBuilder.Append($"v {val.x} {val.y} {val.z}\n");
		}
		stringBuilder.Append("\n");
		Vector3[] normals = mesh.normals;
		for (int j = 0; j < normals.Length; j++)
		{
			Vector3 val2 = normals[j];
			stringBuilder.Append($"vn {val2.x} {val2.y} {val2.z}\n");
		}
		stringBuilder.Append("\n");
		Vector2[] uv = mesh.uv;
		for (int k = 0; k < uv.Length; k++)
		{
			Vector2 val3 = uv[k];
			stringBuilder.Append($"vt {val3.x} {val3.y}\n");
		}
		stringBuilder.Append("\n");
		Vector2[] uv2 = mesh.uv1;
		for (int l = 0; l < uv2.Length; l++)
		{
			Vector2 val4 = uv2[l];
			stringBuilder.Append($"vt1 {val4.x} {val4.y}\n");
		}
		stringBuilder.Append("\n");
		Vector2[] uv3 = mesh.uv2;
		for (int m = 0; m < uv3.Length; m++)
		{
			Vector2 val5 = uv3[m];
			stringBuilder.Append($"vt2 {val5.x} {val5.y}\n");
		}
		stringBuilder.Append("\n");
		Color[] colors = mesh.colors;
		for (int n = 0; n < colors.Length; n++)
		{
			Color val6 = colors[n];
			stringBuilder.Append(string.Format("vc {0} {1} {2} {3}\n", new object[4] { val6.r, val6.g, val6.b, val6.a }));
		}
		for (int num = 0; num < mesh.subMeshCount; num++)
		{
			stringBuilder.Append("\n");
			stringBuilder.Append("usemtl ").Append(((Object)sharedMaterials[num]).name).Append("\n");
			stringBuilder.Append("usemap ").Append(((Object)sharedMaterials[num]).name).Append("\n");
			int[] triangles = mesh.GetTriangles(num);
			for (int num2 = 0; num2 < triangles.Length; num2 += 3)
			{
				stringBuilder.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[num2] + 1, triangles[num2 + 1] + 1, triangles[num2 + 2] + 1));
			}
		}
		return stringBuilder.ToString();
	}

	public static void CubeModelToFile(MVCubeModelBase cm)
	{
		bool append = false;
		foreach (GameObject chunk in cm.Chunks)
		{
			MeshToFile(chunk.GetComponent<MeshFilter>(), ((Object)cm.GameObject).name + ".obj", append);
			append = true;
		}
		Debug.Log((object)("Saved obj file " + ((Object)cm.GameObject).name + ".obj"));
	}

	public static void MeshToFile(MeshFilter mf, string filename, bool append)
	{
		try
		{
			using StreamWriter streamWriter = new StreamWriter(filename, append);
			streamWriter.WriteLine(MeshToString(mf));
		}
		catch (Exception)
		{
		}
	}
}
