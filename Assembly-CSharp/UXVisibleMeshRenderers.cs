using System;
using UnityEngine;

public class UXVisibleMeshRenderers : MonoBehaviour
{
	public void Awake()
	{
		UXVisible component = GetComponent<UXVisible>();
		component.OnVisibleChange = (UXVisible.OnVisibleChangeDelegate)Delegate.Combine(component.OnVisibleChange, new UXVisible.OnVisibleChangeDelegate(HandleOnVisibleChange));
	}

	private void HandleOnVisibleChange(bool visible)
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.enabled = visible;
		}
	}
}
