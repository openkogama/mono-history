using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class Missile : MonoBehaviour
{
	private bool isFired;

	public GameObject explosionPrefab;

	public static Missile CreateMissile()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/Missile")) as GameObject;
		return gameObject.GetComponent<Missile>();
	}

	public void Fire(Vector3 target)
	{
		if (!isFired)
		{
			StartCoroutine(DoFire(target));
		}
	}

	private IEnumerator DoFire(Vector3 target)
	{
		isFired = false;
		Vector3 start = transform.position;
		float distance = (target - start).magnitude;
		float avgSpeed = 100f;
		Vector3 target2 = default;
		yield return StartCoroutine(pTween.To(distance / avgSpeed, (float t) =>
		{
			transform.position = Vector3.Lerp(start, target2, t * t);
		}));
		Object.Instantiate(explosionPrefab, target, Quaternion.identity);
		RemoveCubes(target);
		Object.Destroy(gameObject);
	}

	private void RemoveCubes(Vector3 target)
	{
		List<MVOverlapResult> list = MVElipsoidOverlapCheck.ElipsoidOverlapCheckSector(Vector3.one * 5f, target, Quaternion.identity);
		foreach (MVOverlapResult item in list)
		{
			IntVector[] localCubePos = item.localCubePos;
			foreach (IntVector pos in localCubePos)
			{
				((MVCubeModelBase)MVGameControllerBase.WOCM.GetWorldObjectClient(item.woId)).RemoveCube(pos);
			}
			((MVCubeModelBase)MVGameControllerBase.WOCM.GetWorldObjectClient(item.woId)).HandleDelta();
		}
	}
}
