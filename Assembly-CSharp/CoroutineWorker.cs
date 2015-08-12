using System.Collections;
using UnityEngine;

internal class CoroutineWorker : MonoBehaviour
{
	public void RunCoroutineAndDestroy(IEnumerator coroutine)
	{
		StartCoroutine(coroutine);
	}

	public IEnumerator Run(IEnumerator coroutine)
	{
		yield return StartCoroutine(coroutine);
		Object.Destroy(gameObject);
		Debug.Log("Destroyed coroutine worker.");
	}
}
