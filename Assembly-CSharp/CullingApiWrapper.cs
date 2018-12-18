using System;
using System.Collections.Generic;
using UnityEngine;

public static class CullingApiWrapper
{
	public const int dynamicObjectsDistanceBand = 3;

	public const int overrideDynamicObjectsDistanceBand = 2;

	public const int npcDistanceBandOverride = 3;

	public static float baseDistance = 2.5f;

	public static BoundingSphere[] spheres;

	private const int extraSpheres = 1000;

	private static CullingGroup cullingGroup;

	private static Dictionary<int, ICullingSubscriber> cullingSubscribers = new Dictionary<int, ICullingSubscriber>();

	private static readonly float[] sizes = new float[7] { 0.05f, 1f, 2f, 4f, 8f, 20f, 28f };

	public static int NumBoundSpheres { get; private set; }

	public static void Init(int initialSphereCount, Camera camera, float newBaseDistance, Transform distanceReferencePoint)
	{
		spheres = new BoundingSphere[initialSphereCount + 1000];
		CullingApiWrapper.cullingGroup = new CullingGroup();
		CullingApiWrapper.cullingGroup.targetCamera = camera;
		SetDistanceReferencePoint(distanceReferencePoint);
		CullingApiWrapper.cullingGroup.SetBoundingSpheres(spheres);
		CullingApiWrapper.cullingGroup.SetBoundingSphereCount(0);
		CullingGroup cullingGroup = CullingApiWrapper.cullingGroup;
		cullingGroup.onStateChanged = (CullingGroup.StateChanged)Delegate.Combine(cullingGroup.onStateChanged, new CullingGroup.StateChanged(OnStateChanged));
		baseDistance = newBaseDistance;
		ChangeDistances(baseDistance, camera);
	}

	public static bool IsVisible(int index)
	{
		return cullingGroup.IsVisible(index);
	}

	public static int GetDistance(int index)
	{
		return cullingGroup.GetDistance(index);
	}

	public static void SetDistanceReferencePoint(Transform distanceReferencePoint)
	{
		cullingGroup.SetDistanceReferencePoint(distanceReferencePoint);
	}

	public static void Subscribe(ICullingSubscriber iCullingGroupSubscriber)
	{
		if (spheres.Length == NumBoundSpheres)
		{
			Array.Resize(ref spheres, spheres.Length + 1000);
			cullingGroup.SetBoundingSpheres(spheres);
		}
		cullingSubscribers.Add(NumBoundSpheres, iCullingGroupSubscriber);
		AddBoundingSphere();
		iCullingGroupSubscriber.CullingIndex = NumBoundSpheres - 1;
	}

	public static void UnSubscribe(ICullingSubscriber unSubscriber)
	{
		if (unSubscriber.CullingIndex < 0)
		{
			Debug.LogError("Trying to unsubscribe object with no index");
			return;
		}
		if (NumBoundSpheres > 1)
		{
			ref BoundingSphere reference = ref spheres[unSubscriber.CullingIndex];
			reference = spheres[NumBoundSpheres - 1];
			ICullingSubscriber cullingSubscriber = cullingSubscribers[NumBoundSpheres - 1];
			cullingSubscribers[unSubscriber.CullingIndex] = cullingSubscriber;
			cullingSubscribers.Remove(NumBoundSpheres - 1);
			cullingSubscriber.CullingIndex = unSubscriber.CullingIndex;
		}
		else
		{
			cullingSubscribers.Remove(NumBoundSpheres - 1);
		}
		unSubscriber.CullingIndex = -1;
		NumBoundSpheres--;
		if (cullingGroup != null)
		{
			cullingGroup.SetBoundingSphereCount(NumBoundSpheres);
		}
	}

	public static void DebugVisualize()
	{
		for (int i = 0; i < NumBoundSpheres; i++)
		{
			Gizmos.DrawWireSphere(spheres[i].position, spheres[i].radius);
		}
	}

	public static bool Visible(CullingGroupEvent cullingGroupEvent, int distanceBandIndex)
	{
		if (cullingGroupEvent.hasBecomeInvisible || (cullingGroupEvent.currentDistance > distanceBandIndex && cullingGroupEvent.isVisible))
		{
			return false;
		}
		if (cullingGroupEvent.hasBecomeVisible || (cullingGroupEvent.isVisible && cullingGroupEvent.currentDistance <= distanceBandIndex))
		{
			return true;
		}
		return cullingGroupEvent.isVisible && cullingGroupEvent.currentDistance <= distanceBandIndex;
	}

	public static void ChangeDistances(float newBaseDistance, Camera camera)
	{
		float[] array = UpdateDistances(newBaseDistance);
		cullingGroup.SetBoundingDistances(array);
		float num = 0.6f;
		camera.farClipPlane = (array[array.Length - 1] + sizes[sizes.Length - 1]) * num;
	}

	public static int GetDistanceBand(float radius)
	{
		for (int i = 0; i < sizes.Length; i++)
		{
			if (radius < sizes[i])
			{
				return i;
			}
		}
		return sizes.Length - 1;
	}

	private static float[] UpdateDistances(float newBaseDistance)
	{
		baseDistance = newBaseDistance;
		float[] array = new float[sizes.Length];
		for (int i = 0; i < sizes.Length; i++)
		{
			array[i] = sizes[i] / sizes[0] * newBaseDistance;
		}
		return array;
	}

	public static void Destroy()
	{
		if (cullingGroup != null)
		{
			cullingGroup.onStateChanged = null;
			cullingGroup.Dispose();
			cullingGroup = null;
		}
	}

	public static void PostDestroyCleanup()
	{
		List<ICullingSubscriber> list = new List<ICullingSubscriber>(0);
		foreach (KeyValuePair<int, ICullingSubscriber> cullingSubscriber in cullingSubscribers)
		{
			list.Add(cullingSubscriber.Value);
		}
		foreach (ICullingSubscriber item in list)
		{
			UnSubscribe(item);
		}
		if (list.Count > 0)
		{
			Debug.LogWarningFormat("{0} subscribers are never unsubscribing.", list.Count);
		}
	}

	public static void DebugCullingEvent(CullingGroupEvent cullingGroupEvent)
	{
		Debug.LogFormat("cullingGroupEvent.\ncurrentDistance {0}.\n previousDistance {1}.\n hasBecomeVisible {2}.\n hasBecomeInvisible {3}.\n isVisible {4}.\n wasVisible {5}.", cullingGroupEvent.currentDistance, cullingGroupEvent.previousDistance, cullingGroupEvent.hasBecomeVisible, cullingGroupEvent.hasBecomeInvisible, cullingGroupEvent.isVisible, cullingGroupEvent.wasVisible);
	}

	private static void AddBoundingSphere()
	{
		ref BoundingSphere reference = ref spheres[NumBoundSpheres];
		reference = new BoundingSphere(Vector3.zero, 1f);
		NumBoundSpheres++;
		cullingGroup.SetBoundingSphereCount(NumBoundSpheres);
	}

	private static void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		cullingSubscribers[cullingGroupEvent.index].OnStateChanged(cullingGroupEvent);
	}
}
