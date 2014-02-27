using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
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

	private Ray[] linesOfFire;

	private float maxRange = 50f;

	private HashSet<int> ignoreWoIDs;

	private bool avatarWasHit;

	private Vector3 ownerPosition;

	public static ShotgunShot Create(ShotgunShot prefab, Vector3 position, Ray[] linesOfFire, float maxRange, HashSet<int> ignoreWoIDs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		ShotgunShot shotgunShot = Object.Instantiate((Object)(object)prefab, Vector3.zero, Quaternion.identity) as ShotgunShot;
		shotgunShot.linesOfFire = linesOfFire;
		shotgunShot.maxRange = maxRange;
		shotgunShot.ignoreWoIDs = ignoreWoIDs;
		shotgunShot.ownerPosition = position;
		return shotgunShot;
	}

	private IEnumerator Start()
	{
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
				onHit(hit.position, hit.normal, avatarWasHit, ((Component)this).audio);
			}
		}
		GenerateMesh(m: ((Component)this).GetComponent<MeshFilter>().mesh, hits: hits.ToArray());
		yield return ((MonoBehaviour)this).StartCoroutine(DoFadeAndDestroy(lines: GenerateLines(hits.ToArray(), ownerPosition, rayPrefab), time: fadeTimeSeconds));
	}

	private IEnumerator DoFadeAndDestroy(float time, LineRenderer[] lines)
	{
		float initialTime = time;
		while (time > 0f)
		{
			Color c = ((Component)this).renderer.material.color;
			c.r = (c.g = (c.b = (c.a = 1f - time / initialTime)));
			((Component)this).renderer.material.color = c;
			foreach (LineRenderer line in lines)
			{
				Color linec = ((Component)line).renderer.material.GetColor("_TintColor");
				linec.a = Mathf.Clamp01(linec.a - Time.deltaTime);
				((Component)line).renderer.material.SetColor("_TintColor", linec);
			}
			time -= Time.deltaTime;
			yield return 0;
		}
		foreach (LineRenderer line2 in lines)
		{
			Object.Destroy((Object)(object)((Component)line2).gameObject);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private bool Raycast(Ray ray, ref Hit hit)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		LayerMask val = LayerMask.op_Implicit(1 << LayerMask.NameToLayer("Default"));
		val = LayerMask.op_Implicit(LayerMask.op_Implicit(val) | (1 << (LayerMask.NameToLayer("Player") & 0x1F)));
		if (CollisionDetection.MVHit(ray, out var voxelHit, maxRange, ignoreWoIDs, LayerMask.op_Implicit(val)))
		{
			hit.position = voxelHit.point;
			hit.normal = voxelHit.normal;
			hit.distance = Vector3.Distance(ray.origin, hit.position);
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(voxelHit.woId);
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
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		LineRenderer[] array = new LineRenderer[hits.Length];
		for (int i = 0; i < hits.Length; i++)
		{
			int num = i;
			Object val = Object.Instantiate((Object)(object)prefab, hits[i].position, Quaternion.identity);
			array[num] = (LineRenderer)(object)((val is LineRenderer) ? val : null);
			array[i].SetPosition(0, origin);
			array[i].SetPosition(1, hits[i].position);
		}
		return array;
	}

	private void GenerateMesh(Hit[] hits, Mesh m)
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
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		int[] array = new int[hits.Length * 2 * 3];
		Vector3[] array2 = new Vector3[hits.Length * 4];
		Vector2[] array3 = new Vector2[hits.Length * 4];
		Vector3[] array4 = new Vector3[hits.Length * 4];
		int num = 0;
		int num2 = 0;
		Vector3 up = ((Component)Camera.main).transform.up;
		float num3 = 1.5f;
		for (int i = 0; i < hits.Length; i++)
		{
			Hit hit = hits[i];
			Vector3 val = Vector3.Cross(up, hit.normal);
			Vector3 normalized = val.normalized;
			Vector3 val2 = Vector3.Cross(normalized, hit.normal);
			float num4 = num3 * hit.distance / maxRange;
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
