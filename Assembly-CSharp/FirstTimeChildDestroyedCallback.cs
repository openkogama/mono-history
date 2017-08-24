using System;
using UnityEngine;

public class FirstTimeChildDestroyedCallback : MonoBehaviour
{
	private Action Destroyed;

	public void SubscribeToOnDestroyed(Action onDestroyed)
	{
		Destroyed = onDestroyed;
	}

	private void OnDestroy()
	{
		if (Destroyed != null)
		{
			Destroyed();
		}
	}
}
