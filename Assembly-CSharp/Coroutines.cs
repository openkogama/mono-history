using System.Collections;
using UnityEngine;

internal class Coroutines
{
	public static void StartCoroutine(IEnumerator coroutine)
	{
		GameObject gameObject = new GameObject("Coroutine Worker");
		CoroutineWorker coroutineWorker = gameObject.AddComponent<CoroutineWorker>();
		coroutineWorker.RunCoroutineAndDestroy(coroutine);
	}
}
