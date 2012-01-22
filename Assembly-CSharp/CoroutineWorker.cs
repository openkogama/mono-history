using System.Collections;
using UnityEngine;

internal class CoroutineWorker : MonoBehaviour
{
	public void RunCoroutineAndDestroy(IEnumerator coroutine)
	{
		((MonoBehaviour)this).StartCoroutine(coroutine);
	}

	public IEnumerator Run(IEnumerator coroutine)
	{
		yield return ((MonoBehaviour)this).StartCoroutine(coroutine);
		Object.Destroy((Object)(object)((Component)this).gameObject);
		Debug.Log((object)"Destroyed coroutine worker.");
	}
}
