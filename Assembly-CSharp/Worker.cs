using System;
using System.Collections;
using UnityEngine;

public class Worker : MonoBehaviour
{
	public void To(float duration, float startValue, float endValue, Action<float> callback)
	{
		StartCoroutine(DoTo(duration, startValue, endValue, callback));
	}

	public IEnumerator DoTo(float duration, float startValue, float endValue, Action<float> callback)
	{
		yield return StartCoroutine(pTween.To(duration, startValue, endValue, callback));
		UnityEngine.Object.Destroy(gameObject);
	}
}
