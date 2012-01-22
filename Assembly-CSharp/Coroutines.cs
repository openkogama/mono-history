using System.Collections;
using UnityEngine;

internal class Coroutines
{
	public static void StartCoroutine(IEnumerator coroutine)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		GameObject val = new GameObject("Coroutine Worker");
		CoroutineWorker coroutineWorker = val.AddComponent<CoroutineWorker>();
		coroutineWorker.RunCoroutineAndDestroy(coroutine);
	}
}
