using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ShotgunShot : MonoBehaviour
{
	private struct Hit
	{
		public Vector3 position;

		public Vector3 normal;

		public float distance;
	}

	public delegate void OnDirectHitDelegate(MVWorldObjectClient wo, Vector3 position, Vector3 normal, Ray lineOfFire);

	public delegate void OnHitDelegate(Vector3 position, Vector3 normal, bool hasHitAvatar, AudioSource audioSource);

	public OnDirectHitDelegate onDirectHit;

	public OnHitDelegate onHit;

	public LineRenderer rayPrefab;

	public float fadeTimeSeconds = 5f;

	private MeshRenderer meshRenderer;

	private MeshFilter meshFilter;

	private AudioSource aSource;

	private Ray[] linesOfFire;

	private float maxRange = 50f;

	private HashSet<int> ignoreWoIDs;

	private bool avatarWasHit;

	private Vector3 ownerPosition;

	public static ShotgunShot Create(ShotgunShot prefab, Vector3 position, Ray[] linesOfFire, float maxRange, HashSet<int> ignoreWoIDs)
	{
		ShotgunShot shotgunShot = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as ShotgunShot;
		shotgunShot.linesOfFire = linesOfFire;
		shotgunShot.maxRange = maxRange;
		shotgunShot.ignoreWoIDs = ignoreWoIDs;
		shotgunShot.ownerPosition = position;
		return shotgunShot;
	}

	private IEnumerator Start()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		aSource = GetComponent<AudioSource>();
		List<Hit> hits = new List<Hit>();
		for (int i = 0; i < linesOfFire.Length; i++)
		{
			Hit hit = default;
			bool success = Raycast(linesOfFire[i], ref hit);
			if (success)
			{
				hits.Add(hit);
			}
			if (success && onHit != null)
			{
				onHit(hit.position, hit.normal, avatarWasHit, aSource);
			}
		}
		GenerateMesh(m: meshFilter.mesh, hits: hits.ToArray());
		yield return StartCoroutine(DoFadeAndDestroy(lines: GenerateLines(hits.ToArray(), ownerPosition, rayPrefab), time: fadeTimeSeconds));
	}

	private IEnumerator DoFadeAndDestroy(float time, LineRenderer[] lines)
	{
		float initialTime = time;
		while (time > 0f)
		{
			Color c = meshRenderer.material.color;
			c.r = (c.g = (c.b = (c.a = 1f - time / initialTime)));
			meshRenderer.material.color = c;
			foreach (LineRenderer line in lines)
			{
				Color linec = line.material.GetColor("_TintColor");
				linec.a = Mathf.Clamp01(linec.a - Time.deltaTime);
				line.material.SetColor("_TintColor", linec);
			}
			time -= Time.deltaTime;
			yield return 0;
		}
		foreach (LineRenderer line2 in lines)
		{
			Object.Destroy(line2.gameObject);
		}
		Object.Destroy(gameObject);
	}

	private bool Raycast(Ray ray, ref Hit hit)
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Default");
		layerMask = (int)layerMask | (1 << (LayerMask.NameToLayer("Player") & 0x1F));
		if (CollisionDetection.MVHit(ray, out var voxelHit, maxRange, ignoreWoIDs, layerMask))
		{
			hit.position = voxelHit.point;
			hit.normal = voxelHit.normal;
			hit.distance = Vector3.Distance(ray.origin, hit.position);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
			avatarWasHit = true;
			if (onDirectHit != null)
			{
				onDirectHit(worldObjectClient, hit.position, hit.normal, ray);
			}
			return true;
		}
		return false;
	}

	private LineRenderer[] GenerateLines(Hit[] hits, Vector3 origin, LineRenderer prefab)
	{
		LineRenderer[] array = new LineRenderer[hits.Length];
		for (int i = 0; i < hits.Length; i++)
		{
			array[i] = Object.Instantiate(prefab, hits[i].position, Quaternion.identity) as LineRenderer;
			array[i].SetPosition(0, origin);
			array[i].SetPosition(1, hits[i].position);
		}
		return array;
	}

	private void GenerateMesh(Hit[] hits, Mesh m)
	{
		int[] array = new int[hits.Length * 2 * 3];
		Vector3[] array2 = new Vector3[hits.Length * 4];
		Vector2[] array3 = new Vector2[hits.Length * 4];
		Vector3[] array4 = new Vector3[hits.Length * 4];
		int num = 0;
		int num2 = 0;
		Vector3 up = Camera.main.transform.up;
		float num3 = 1.5f;
		for (int i = 0; i < hits.Length; i++)
		{
			Hit hit = hits[i];
			Vector3 normalized = Vector3.Cross(up, hit.normal).normalized;
			Vector3 vector = Vector3.Cross(normalized, hit.normal);
			float num4 = num3 * hit.distance / maxRange;
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
