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

	public static void Initialize()
	{
		sphere = (GameObject)Object.Instantiate(Resources.Load("DebugAssets/sphere/sphere00"));
		Object.Destroy(sphere.GetComponent<Collider>());
		MeshRenderer componentInChildren = sphere.GetComponentInChildren<MeshRenderer>();
		componentInChildren.gameObject.AddComponent<Wireframe>();
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
		Debug.Log("test1");
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
		float distance = 400f;
		if (MVRaycast.MVHit(ray, out var voxelHit, distance))
		{
			Debug.DrawLine(voxelHit.point, voxelHit.point + voxelHit.normal);
			Debug.Log(voxelHit.transform.name);
		}
	}

	private static void ElipsoidAllTest()
	{
		Debug.Log("test2");
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
		float num = 0.5f;
		Vector3 radius = Vector3.one * num;
		radius.y *= 2f;
		float distance = 100f;
		List<VoxelHit> list = MVSweptElipsoidCheck.MVElipsoidCastAll(ray, radius, Quaternion.identity, distance);
		Debug.Log(list.Count);
	}

	private static void ElipsoidStressTest(int iterations)
	{
		Debug.Log("test3");
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
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
		Debug.Log("test4");
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
		rayPrev.origin = ray.origin;
		rayPrev.direction = ray.direction;
		VoxelHit voxelHit = default;
		float num = 0.5f;
		float distance = 300f;
		Vector3 vector = Vector3.one * num;
		Quaternion identity = Quaternion.identity;
		identity.SetFromToRotation(Vector3.up, (Vector3.left + Vector3.up / 2f).normalized);
		sphere.transform.rotation = identity;
		sphere.transform.localScale = vector;
		worldToLocalMatrix = sphere.transform.worldToLocalMatrix;
		localToWorldMatrix = sphere.transform.localToWorldMatrix;
		if (MVSweptElipsoidCheck.MVElipsoidCast(ray, vector, sphere.transform.rotation, distance, out voxelHit))
		{
			MVGameControllerBase.GizmoDrawer.AddSphere(ray.origin + ray.direction * voxelHit.distance, num, Color.blue, clear: true);
			sphere.transform.position = ray.origin + ray.direction * voxelHit.distance;
			Debug.DrawLine(voxelHit.point, voxelHit.point + voxelHit.normal, Color.yellow);
			Debug.DrawLine(voxelHit.point, voxelHit.point + Vector3.right * 0.3f, Color.yellow);
			Vector3 intersection = default;
			float distance2 = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, ray.origin + 100f * ray.direction, ray.origin + 100f * -ray.direction, ref distance2, ref intersection))
			{
				Debug.Log("Not within line segment");
			}
		}
	}

	private static void ElipsoidOverlapCheckTestTransform()
	{
		Debug.Log("test5");
		float num = 0.5f;
		Vector3 position = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f);
		Ray ray = Camera.main.ScreenPointToRay(position);
		ray.origin += ray.direction * 5f;
		Vector3 localScale = Vector3.one * num;
		localScale.y *= 4f;
		Quaternion identity = Quaternion.identity;
		identity.SetFromToRotation(Vector3.right, (Vector3.forward + Vector3.left + Vector3.up * 2f).normalized);
		sphere.transform.rotation = identity;
		sphere.transform.localScale = localScale;
		sphere.transform.position = ray.origin;
		MeshFilter componentInChildren = sphere.GetComponentInChildren<MeshFilter>();
		Bounds bounds = componentInChildren.sharedMesh.bounds;
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.K))
		{
			List<MVOverlapResult> list = MVElipsoidOverlapCheck.ElipsoidOverlapCheckSector(ray.origin, sphere.transform, bounds);
			foreach (MVOverlapResult item in list)
			{
				ICubeModelCollider cubeModelCollider = (ICubeModelCollider)MVGameControllerBase.WOCM.GetWorldObjectClient(item.woId);
				IntVector[] localCubePos = item.localCubePos;
				foreach (IntVector intVector in localCubePos)
				{
					Debug.LogError("Disabled this test stuff");
				}
			}
		}
		if (MVElipsoidOverlapCheck.ElipsoidOverlapCheckBool(ray.origin, sphere.transform, bounds))
		{
			Debug.Log("is overlapping");
		}
	}

	private static void ElipsoidOverlapCheckTest()
	{
		Debug.Log("test6");
		float num = 0.5f;
		Vector3 position = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f);
		Ray ray = Camera.main.ScreenPointToRay(position);
		ray.origin += ray.direction * 5f;
		Vector3 vector = Vector3.one * num;
		vector.y *= 4f;
		Quaternion identity = Quaternion.identity;
		identity.SetFromToRotation(Vector3.right, (Vector3.forward + Vector3.left + Vector3.up * 2f).normalized);
		sphere.transform.rotation = identity;
		sphere.transform.localScale = vector;
		sphere.transform.position = ray.origin;
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.K))
		{
			List<MVOverlapResult> list = MVElipsoidOverlapCheck.ElipsoidOverlapCheckSector(vector, ray.origin, sphere.transform.rotation);
			foreach (MVOverlapResult item in list)
			{
				ICubeModelCollider cubeModelCollider = (ICubeModelCollider)MVGameControllerBase.WOCM.GetWorldObjectClient(item.woId);
				IntVector[] localCubePos = item.localCubePos;
				foreach (IntVector intVector in localCubePos)
				{
					Debug.LogError("Disabled this test stuff");
				}
			}
		}
		if (MVElipsoidOverlapCheck.ElipsoidOverlapCheckBool(vector, ray.origin, sphere.transform.rotation))
		{
			Debug.Log("is overlapping");
		}
	}

	private static void ElipsoidFalsePositiveTest()
	{
		Debug.Log("test7");
		VoxelHit voxelHit = default;
		float num = 0.5f;
		float distance = 300f;
		Vector3 position = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f);
		Ray ray = Camera.main.ScreenPointToRay(position);
		ray.origin += ray.direction * 10f;
		Vector3 vector = Vector3.one * num;
		sphere.transform.localScale = vector;
		sphere.transform.position = ray.origin;
		if (MVSweptElipsoidCheck.MVElipsoidCast(ray, vector, sphere.transform.rotation, distance, out voxelHit))
		{
			MVGameControllerBase.GizmoDrawer.AddSphere(ray.origin, num, Color.red, clear: true);
			Debug.DrawLine(voxelHit.point, voxelHit.point + voxelHit.normal, Color.yellow);
		}
		else
		{
			MVGameControllerBase.GizmoDrawer.AddSphere(ray.origin, num, Color.white, clear: true);
		}
	}

	private static void ElipsoidDownTest()
	{
		Debug.Log("test8");
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
		rayPrev.origin = ray.origin;
		rayPrev.direction = ray.direction;
		VoxelHit voxelHit = default;
		float num = 0.5f;
		float distance = 15f;
		Vector3 radius = Vector3.one * num;
		radius.y *= 2f;
		if (MVSweptElipsoidCheck.MVElipsoidCast(ray, radius, Quaternion.identity, distance, out voxelHit))
		{
			MVGameControllerBase.GizmoDrawer.AddSphere(ray.origin + ray.direction * voxelHit.distance, num, Color.red, clear: true);
			Debug.DrawLine(voxelHit.point, voxelHit.point + voxelHit.normal, Color.yellow);
			Debug.DrawLine(voxelHit.point, voxelHit.point + Vector3.right * 0.3f, Color.yellow);
			Vector3 intersection = default;
			float distance2 = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, ray.origin + 100f * ray.direction, ray.origin + 100f * -ray.direction, ref distance2, ref intersection))
			{
				Debug.Log("Not within line segment");
			}
			Ray ray2 = new Ray(ray.origin + ray.direction * voxelHit.distance + Vector3.up * 0.1f, Vector3.down);
			if (MVSweptElipsoidCheck.MVElipsoidCast(ray2, radius, Quaternion.identity, 10f, out voxelHit))
			{
				Debug.Log("Did hit");
			}
			else
			{
				Debug.Log("Did not hit");
			}
		}
	}
}
