using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshDecal : MonoBehaviour
{
	public struct Hit(Vector3 position, Vector3 normal, float distance)
	{
		public Vector3 position = position;

		public Vector3 normal = normal;

		public float distance = distance;
	}

	public float timeout = 5f;

	private float currentTime;

	public static MeshDecal Create(Hit hit, Material decalMaterial, Transform parent)
	{
		return Create(new Hit[1] { hit }, decalMaterial, parent);
	}

	public static MeshDecal Create(Hit[] hits, Material decalMaterial, Transform parent)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		GameObject val = new GameObject("Decal");
		MeshDecal meshDecal = val.AddComponent<MeshDecal>();
		MeshFilter component = val.GetComponent<MeshFilter>();
		meshDecal.GenerateMesh(hits, component.mesh);
		((Component)meshDecal).renderer.material = decalMaterial;
		((Component)meshDecal).transform.parent = parent;
		return meshDecal;
	}

	private void Update()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		currentTime += Time.deltaTime;
		if (currentTime >= timeout)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		Color color = ((Component)this).renderer.material.color;
		color.r = (color.g = (color.b = (color.a = currentTime / timeout)));
		((Component)this).renderer.material.color = color;
	}

	private void GenerateMesh(Hit[] hits, Mesh m, float maxDistance = 50f)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		int[] array = new int[hits.Length * 2 * 3];
		Vector3[] array2 = new Vector3[hits.Length * 4];
		Vector2[] array3 = new Vector2[hits.Length * 4];
		Vector3[] array4 = new Vector3[hits.Length * 4];
		int num = 0;
		int num2 = 0;
		Vector3 up = ((Component)Camera.main).transform.up;
		float num3 = 0.5f;
		for (int i = 0; i < hits.Length; i++)
		{
			Hit hit = hits[i];
			Vector3 val = Vector3.Cross(up, hit.normal);
			Vector3 normalized = val.normalized;
			Vector3 val2 = Vector3.Cross(normalized, hit.normal);
			float num4 = num3;
			normalized *= num4;
			val2 *= num4;
			ref Vector3 reference = ref array4[num2];
			reference = hit.normal;
			ref Vector2 reference2 = ref array3[num2];
			reference2 = new Vector2(0f, 0f);
			ref Vector3 reference3 = ref array2[num2++];
			reference3 = hit.position - normalized + val2;
			ref Vector3 reference4 = ref array4[num2];
			reference4 = hit.normal;
			ref Vector2 reference5 = ref array3[num2];
			reference5 = new Vector2(1f, 0f);
			ref Vector3 reference6 = ref array2[num2++];
			reference6 = hit.position + normalized + val2;
			ref Vector3 reference7 = ref array4[num2];
			reference7 = hit.normal;
			ref Vector2 reference8 = ref array3[num2];
			reference8 = new Vector2(1f, 1f);
			ref Vector3 reference9 = ref array2[num2++];
			reference9 = hit.position + normalized - val2;
			ref Vector3 reference10 = ref array4[num2];
			reference10 = hit.normal;
			ref Vector2 reference11 = ref array3[num2];
			reference11 = new Vector2(0f, 1f);
			ref Vector3 reference12 = ref array2[num2++];
			reference12 = hit.position - normalized - val2;
			array[num++] = num2 - 4;
			array[num++] = num2 - 3;
			array[num++] = num2 - 2;
			array[num++] = num2 - 2;
			array[num++] = num2 - 1;
			array[num++] = num2 - 4;
		}
		m.vertices = array2;
		m.triangles = array;
		m.uv = array3;
		m.normals = array4;
	}
}
