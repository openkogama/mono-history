using System;
using UnityEngine;

public class UXVisibleMeshRenderers : MonoBehaviour
{
	public void Awake()
	{
		UXVisible component = ((Component)this).GetComponent<UXVisible>();
		component.OnVisibleChange = (UXVisible.OnVisibleChangeDelegate)Delegate.Combine(component.OnVisibleChange, new UXVisible.OnVisibleChangeDelegate(HandleOnVisibleChange));
	}

	private void HandleOnVisibleChange(bool visible)
	{
		MeshRenderer[] componentsInChildren = ((Component)this).GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer val in componentsInChildren)
		{
			((Renderer)val).enabled = visible;
		}
	}
}
