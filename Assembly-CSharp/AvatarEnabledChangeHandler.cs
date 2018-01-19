using System;
using UnityEngine;

public class AvatarEnabledChangeHandler : MonoBehaviour
{
	public Action OnEnabled;

	public Action OnDisabled;

	private void OnEnable()
	{
		if (OnEnabled != null)
		{
			OnEnabled();
		}
	}

	private void OnDisable()
	{
		if (OnDisabled != null)
		{
			OnDisabled();
		}
	}
}
