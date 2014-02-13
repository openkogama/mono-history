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
		Object val = Object.Instantiate(Resources.Load("Prefabs/Missile"));
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		return val2.GetComponent<Missile>();
	}

	public void Fire(Vector3 target)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!isFired)
		{
			((MonoBehaviour)this).StartCoroutine(DoFire(target));
		}
	}

	private IEnumerator DoFire(Vector3 target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		isFired = false;
		Vector3 start = ((Component)this).transform.position;
		Vector3 val = target - start;
		float distance = val.magnitude;
		float avgSpeed = 100f;
		Vector3 target2 = default;
		yield return ((MonoBehaviour)this).StartCoroutine(pTween.To(distance / avgSpeed, (float t) =>
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			((Component)this).transform.position = Vector3.Lerp(start, target2, t * t);
		}));
		Object.Instantiate((Object)(object)explosionPrefab, target, Quaternion.identity);
		RemoveCubes(target);
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void RemoveCubes(Vector3 target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		List<MVOverlapResult> list = MVElipsoidOverlapCheck.ElipsoidOverlapCheckSector(Vector3.one * 5f, target, Quaternion.identity);
		foreach (MVOverlapResult item in list)
		{
			IntVector[] localCubePos = item.localCubePos;
			foreach (IntVector pos in localCubePos)
			{
				((MVCubeModelBase)MVGameController.Instance.WOCM.GetWorldObjectClient(item.woId)).RemoveCube(pos);
			}
			((MVCubeModelBase)MVGameController.Instance.WOCM.GetWorldObjectClient(item.woId)).HandleDelta();
		}
	}
}
