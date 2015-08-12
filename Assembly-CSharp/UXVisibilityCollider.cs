using System;
using UnityEngine;

[RequireComponent(typeof(UXVisibility))]
public class UXVisibilityCollider : MonoBehaviour
{
	private void Awake()
	{
		UXVisibility component = GetComponent<UXVisibility>();
		component.OnVisibilityChange = (UXVisibility.VisibilityChangeDelegate)Delegate.Combine(component.OnVisibilityChange, new UXVisibility.VisibilityChangeDelegate(HandleVisibilityChange));
	}

	private void HandleVisibilityChange(float visibility)
	{
		GetComponent<Collider>().enabled = visibility > 0f;
	}
}
