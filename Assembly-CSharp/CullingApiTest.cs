using System.Collections.Generic;
using UnityEngine;

public class CullingApiTest : MonoBehaviour
{
	private class TestClass : ICullingSubscriber
	{
		public int CullingIndex { get; set; }

		public void OnStateChanged(CullingGroupEvent cullingGroupEvent)
		{
			Debug.Log("cullingGroupEvent " + cullingGroupEvent.index);
		}
	}

	private List<TestClass> testClasses = new List<TestClass>();

	[SerializeField]
	private bool addBoundingSphere;

	[SerializeField]
	private bool unSubscribe;

	[SerializeField]
	private bool distancesChange;

	[SerializeField]
	private Transform movingTransform;

	private Camera mainCamera;

	private void Start()
	{
		mainCamera = Camera.main;
		CullingApiWrapper.Init(0, mainCamera, CullingApiWrapper.baseDistance, mainCamera.transform);
		Subscribe();
	}

	private void Subscribe()
	{
		testClasses.Add(new TestClass());
		CullingApiWrapper.Subscribe(testClasses[testClasses.Count - 1]);
	}

	private void UnSubscribe()
	{
		if (testClasses.Count != 0)
		{
			int index = Random.Range(0, testClasses.Count - 1);
			CullingApiWrapper.UnSubscribe(testClasses[index]);
			testClasses.RemoveAt(index);
		}
	}

	private void Update()
	{
		if (addBoundingSphere)
		{
			Subscribe();
			addBoundingSphere = false;
		}
		if (unSubscribe)
		{
			UnSubscribe();
			unSubscribe = false;
		}
		if (distancesChange)
		{
			CullingApiWrapper.ChangeDistances(10f, mainCamera);
			distancesChange = false;
		}
		for (int i = 0; i < CullingApiWrapper.NumBoundSpheres; i++)
		{
			CullingApiWrapper.spheres[i].position = movingTransform.position;
			CullingApiWrapper.spheres[i].radius = 2f;
		}
	}

	private void OnDestroy()
	{
		CullingApiWrapper.Destroy();
	}
}
