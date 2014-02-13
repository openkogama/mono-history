using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class CollisionDetectionTests
{
	private static Ray rayPrev = default;

	private static bool isInitialized = false;

	public static Matrix4x4 worldToLocalMatrix;

	public static Matrix4x4 localToWorldMatrix;

	public static GameObject sphere;

	private static bool DEBUG_MODE = false;

	static CollisionDetectionTests()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
	}

	public static void Initialize()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected Obj, but got Unknown
		sphere = (GameObject)Object.Instantiate(Resources.Load("DebugAssets/sphere/sphere00"));
		Object.Destroy((Object)(object)sphere.GetComponent<Collider>());
		MeshRenderer componentInChildren = sphere.GetComponentInChildren<MeshRenderer>();
		((Component)componentInChildren).gameObject.AddComponent<Wireframe>();
	}

	public static void Update()
	{
		if (DEBUG_MODE)
		{
			if (!isInitialized)
			{
				Initialize();
				isInitialized = true;
			}
			ElipsoidOverlapCheckTestTransform();
		}
	}

	private static void RaycastTest()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		float distance = 400f;
		if (MVRaycast.MVHit(ray, out var voxelHit, distance))
		{
			Debug.DrawLine(voxelHit.point, voxelHit.point + voxelHit.normal);
			Debug.Log((object)((Object)voxelHit.transform).name);
		}
	}

	private static void ElipsoidAllTest()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		float num = 0.5f;
		Vector3 radius = Vector3.one * num;
		radius.y *= 2f;
		float distance = 100f;
		List<VoxelHit> list = MVSweptElipsoidCheck.MVElipsoidCastAll(ray, radius, Quaternion.identity, distance);
		Debug.Log((object)list.Count);
	}

	private static void ElipsoidStressTest(int iterations)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		float num = 0.5f;
		Vector3 radius = Vector3.one * num;
		radius.y *= 2f;
		float distance = 100f;
		VoxelHit voxelHit = default;
		for (int i = 0; i < iterations; i++)
		{
			MVSweptElipsoidCheck.MVElipsoidCast(ray, radius, Quaternion.identity, distance, out voxelHit);
		}
	}

	private static void ElipsoidTest()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		rayPrev.origin = ray.origin;
		rayPrev.direction = ray.direction;
		VoxelHit voxelHit = default;
		float num = 0.5f;
		float distance = 300f;
		Vector3 val = Vector3.one * num;
		Quaternion identity = Quaternion.identity;
		Vector3 up = Vector3.up;
		Vector3 val2 = Vector3.left + Vector3.up / 2f;
		identity.SetFromToRotation(up, val2.normalized);
		sphere.transform.rotation = identity;
		sphere.transform.localScale = val;
		worldToLocalMatrix = sphere.transform.worldToLocalMatrix;
		localToWorldMatrix = sphere.transform.localToWorldMatrix;
		if (MVSweptElipsoidCheck.MVElipsoidCast(ray, val, sphere.transform.rotation, distance, out voxelHit))
		{
			((Component)MVGameController.Instance).GetComponent<GizmoDrawer>().AddSphere(ray.origin + ray.direction * voxelHit.distance, num, Color.blue, clear: true);
			sphere.transform.position = ray.origin + ray.direction * voxelHit.distance;
			Debug.DrawLine(voxelHit.point, voxelHit.point + voxelHit.normal, Color.yellow);
			Debug.DrawLine(voxelHit.point, voxelHit.point + Vector3.right * 0.3f, Color.yellow);
			Vector3 intersection = default;
			float distance2 = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, ray.origin + 100f * ray.direction, ray.origin + 100f * -ray.direction, ref distance2, ref intersection))
			{
				Debug.Log((object)"Not within line segment");
			}
		}
	}

	private static void ElipsoidOverlapCheckTestTransform()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.5f;
		Vector3 val = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f);
		Ray val2 = Camera.main.ScreenPointToRay(val);
		val2.origin += val2.direction * 5f;
		Vector3 localScale = Vector3.one * num;
		localScale.y *= 4f;
		Quaternion identity = Quaternion.identity;
		Vector3 right = Vector3.right;
		Vector3 val3 = Vector3.forward + Vector3.left + Vector3.up * 2f;
		identity.SetFromToRotation(right, val3.normalized);
		sphere.transform.rotation = identity;
		sphere.transform.localScale = localScale;
		sphere.transform.position = val2.origin;
		MeshFilter componentInChildren = sphere.GetComponentInChildren<MeshFilter>();
		Bounds bounds = componentInChildren.sharedMesh.bounds;
		if (Input.GetKeyDown((KeyCode)107))
		{
			List<MVOverlapResult> list = MVElipsoidOverlapCheck.ElipsoidOverlapCheckSector(val2.origin, sphere.transform, bounds);
			foreach (MVOverlapResult item in list)
			{
				MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)MVGameController.Instance.WOCM.GetWorldObjectClient(item.woId);
				IntVector[] localCubePos = item.localCubePos;
				foreach (IntVector pos in localCubePos)
				{
					mVCubeModelBase.RemoveCube(pos);
				}
				mVCubeModelBase.HandleDelta();
			}
		}
		if (MVElipsoidOverlapCheck.ElipsoidOverlapCheckBool(val2.origin, sphere.transform, bounds))
		{
			Debug.Log((object)"is overlapping");
		}
	}

	private static void ElipsoidOverlapCheckTest()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.5f;
		Vector3 val = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f);
		Ray val2 = Camera.main.ScreenPointToRay(val);
		val2.origin += val2.direction * 5f;
		Vector3 val3 = Vector3.one * num;
		val3.y *= 4f;
		Quaternion identity = Quaternion.identity;
		Vector3 right = Vector3.right;
		Vector3 val4 = Vector3.forward + Vector3.left + Vector3.up * 2f;
		identity.SetFromToRotation(right, val4.normalized);
		sphere.transform.rotation = identity;
		sphere.transform.localScale = val3;
		sphere.transform.position = val2.origin;
		if (Input.GetKeyDown((KeyCode)107))
		{
			List<MVOverlapResult> list = MVElipsoidOverlapCheck.ElipsoidOverlapCheckSector(val3, val2.origin, sphere.transform.rotation);
			foreach (MVOverlapResult item in list)
			{
				MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)MVGameController.Instance.WOCM.GetWorldObjectClient(item.woId);
				IntVector[] localCubePos = item.localCubePos;
				foreach (IntVector pos in localCubePos)
				{
					mVCubeModelBase.RemoveCube(pos);
				}
				mVCubeModelBase.HandleDelta();
			}
		}
		if (MVElipsoidOverlapCheck.ElipsoidOverlapCheckBool(val3, val2.origin, sphere.transform.rotation))
		{
			Debug.Log((object)"is overlapping");
		}
	}

	private static void ElipsoidFalsePositiveTest()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		VoxelHit voxelHit = default;
		float num = 0.5f;
		float distance = 300f;
		Vector3 val = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f);
		Ray ray = Camera.main.ScreenPointToRay(val);
		ray.origin += ray.direction * 10f;
		Vector3 val2 = Vector3.one * num;
		sphere.transform.localScale = val2;
		sphere.transform.position = ray.origin;
		if (MVSweptElipsoidCheck.MVElipsoidCast(ray, val2, sphere.transform.rotation, distance, out voxelHit))
		{
			((Component)MVGameController.Instance).GetComponent<GizmoDrawer>().AddSphere(ray.origin, num, Color.red, clear: true);
			Debug.DrawLine(voxelHit.point, voxelHit.point + voxelHit.normal, Color.yellow);
		}
		else
		{
			((Component)MVGameController.Instance).GetComponent<GizmoDrawer>().AddSphere(ray.origin, num, Color.white, clear: true);
		}
	}

	private static void ElipsoidDownTest()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		rayPrev.origin = ray.origin;
		rayPrev.direction = ray.direction;
		VoxelHit voxelHit = default;
		float num = 0.5f;
		float distance = 15f;
		Vector3 radius = Vector3.one * num;
		radius.y *= 2f;
		if (MVSweptElipsoidCheck.MVElipsoidCast(ray, radius, Quaternion.identity, distance, out voxelHit))
		{
			((Component)MVGameController.Instance).GetComponent<GizmoDrawer>().AddSphere(ray.origin + ray.direction * voxelHit.distance, num, Color.red, clear: true);
			Debug.DrawLine(voxelHit.point, voxelHit.point + voxelHit.normal, Color.yellow);
			Debug.DrawLine(voxelHit.point, voxelHit.point + Vector3.right * 0.3f, Color.yellow);
			Vector3 intersection = default;
			float distance2 = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, ray.origin + 100f * ray.direction, ray.origin + 100f * -ray.direction, ref distance2, ref intersection))
			{
				Debug.Log((object)"Not within line segment");
			}
			Ray ray2 = new Ray(ray.origin + ray.direction * voxelHit.distance + Vector3.up * 0.1f, Vector3.down);
			if (MVSweptElipsoidCheck.MVElipsoidCast(ray2, radius, Quaternion.identity, 10f, out voxelHit))
			{
				Debug.Log((object)"Did hit");
			}
			else
			{
				Debug.Log((object)"Did not hit");
			}
		}
	}
}
