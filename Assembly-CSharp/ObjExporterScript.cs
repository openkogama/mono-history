using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MV.WorldObject;
using UnityEngine;

public static class ObjExporterScript
{
	public static string MeshToString(MeshFilter mf)
	{
		Mesh sharedMesh = mf.sharedMesh;
		Material[] sharedMaterials = mf.GetComponent<Renderer>().sharedMaterials;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("g ").Append(mf.name).Append("\n");
		Vector3[] vertices = sharedMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = vertices[i];
			stringBuilder.Append($"v {vector.x} {vector.y} {vector.z}\n");
		}
		stringBuilder.Append("\n");
		Vector3[] normals = sharedMesh.normals;
		for (int j = 0; j < normals.Length; j++)
		{
			Vector3 vector2 = normals[j];
			stringBuilder.Append($"vn {vector2.x} {vector2.y} {vector2.z}\n");
		}
		stringBuilder.Append("\n");
		Vector2[] uv = sharedMesh.uv;
		for (int k = 0; k < uv.Length; k++)
		{
			Vector2 vector3 = uv[k];
			stringBuilder.Append($"vt {vector3.x} {vector3.y}\n");
		}
		stringBuilder.Append("\n");
		Vector2[] uv2 = sharedMesh.uv2;
		for (int l = 0; l < uv2.Length; l++)
		{
			Vector2 vector4 = uv2[l];
			stringBuilder.Append($"vt1 {vector4.x} {vector4.y}\n");
		}
		stringBuilder.Append("\n");
		Vector2[] uv3 = sharedMesh.uv2;
		for (int m = 0; m < uv3.Length; m++)
		{
			Vector2 vector5 = uv3[m];
			stringBuilder.Append($"vt2 {vector5.x} {vector5.y}\n");
		}
		stringBuilder.Append("\n");
		Color[] colors = sharedMesh.colors;
		for (int n = 0; n < colors.Length; n++)
		{
			Color color = colors[n];
			stringBuilder.Append($"vc {color.r} {color.g} {color.b} {color.a}\n");
		}
		for (int num = 0; num < sharedMesh.subMeshCount; num++)
		{
			stringBuilder.Append("\n");
			stringBuilder.Append("usemtl ").Append(sharedMaterials[num].name).Append("\n");
			stringBuilder.Append("usemap ").Append(sharedMaterials[num].name).Append("\n");
			int[] triangles = sharedMesh.GetTriangles(num);
			for (int num2 = 0; num2 < triangles.Length; num2 += 3)
			{
				stringBuilder.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[num2] + 1, triangles[num2 + 1] + 1, triangles[num2 + 2] + 1));
			}
		}
		return stringBuilder.ToString();
	}

	public static void CubeModelToFile(MVCubeModelBase cm)
	{
		foreach (KeyValuePair<IntVector, GameObject> item in (IEnumerable)cm.ChunkInstances)
		{
			MeshToFile(item.Value.GetComponent<MeshFilter>(), cm.GameObject.name + item.Value.name + ".obj", append: false);
			Debug.Log("Saved obj file " + cm.GameObject.name + item.Value.name + ".obj");
		}
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
