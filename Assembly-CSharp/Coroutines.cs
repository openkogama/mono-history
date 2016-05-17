using System.Collections;
using UnityEngine;

public class Coroutines : MonoBehaviour
{
	private static Coroutines instance;

	private void Awake()
	{
		instance = this;
	}

	public static void Start(IEnumerator coroutine)
	{
		instance.StartCoroutine(coroutine);
	}
}
