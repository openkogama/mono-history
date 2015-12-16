using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
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
		GameObject gameObject = new GameObject("Decal");
		MeshDecal meshDecal = gameObject.AddComponent<MeshDecal>();
		MeshFilter component = gameObject.GetComponent<MeshFilter>();
		meshDecal.GenerateMesh(hits, component.mesh);
		meshDecal.GetComponent<Renderer>().material = decalMaterial;
		meshDecal.transform.parent = parent;
		return meshDecal;
	}

	private void Update()
	{
		currentTime += Time.deltaTime;
		if (currentTime >= timeout)
		{
			Object.Destroy(gameObject);
		}
		Color color = GetComponent<Renderer>().material.color;
		color.r = (color.g = (color.b = (color.a = currentTime / timeout)));
		GetComponent<Renderer>().material.color = color;
	}

	private void GenerateMesh(Hit[] hits, Mesh m, float maxDistance = 50f)
	{
		int[] array = new int[hits.Length * 2 * 3];
		Vector3[] array2 = new Vector3[hits.Length * 4];
		Vector2[] array3 = new Vector2[hits.Length * 4];
		Vector3[] array4 = new Vector3[hits.Length * 4];
		int num = 0;
		int num2 = 0;
		Vector3 up = Camera.main.transform.up;
		float num3 = 0.5f;
		for (int i = 0; i < hits.Length; i++)
		{
			Hit hit = hits[i];
			Vector3 normalized = Vector3.Cross(up, hit.normal).normalized;
			Vector3 vector = Vector3.Cross(normalized, hit.normal);
			float num4 = num3;
			normalized *= num4;
			vector *= num4;
			ref Vector3 reference = ref array4[num2];
			reference = hit.normal;
			ref Vector2 reference2 = ref array3[num2];
			reference2 = new Vector2(0f, 0f);
			ref Vector3 reference3 = ref array2[num2++];
			reference3 = hit.position - normalized + vector;
			ref Vector3 reference4 = ref array4[num2];
			reference4 = hit.normal;
			ref Vector2 reference5 = ref array3[num2];
			reference5 = new Vector2(1f, 0f);
			ref Vector3 reference6 = ref array2[num2++];
			reference6 = hit.position + normalized + vector;
			ref Vector3 reference7 = ref array4[num2];
			reference7 = hit.normal;
			ref Vector2 reference8 = ref array3[num2];
			reference8 = new Vector2(1f, 1f);
			ref Vector3 reference9 = ref array2[num2++];
			reference9 = hit.position + normalized - vector;
			ref Vector3 reference10 = ref array4[num2];
			reference10 = hit.normal;
			ref Vector2 reference11 = ref array3[num2];
			reference11 = new Vector2(0f, 1f);
			ref Vector3 reference12 = ref array2[num2++];
			reference12 = hit.position - normalized - vector;
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
