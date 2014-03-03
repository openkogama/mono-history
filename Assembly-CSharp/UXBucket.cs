using System;
using UnityEngine;

[RequireComponent(typeof(UXDropObject))]
[RequireComponent(typeof(UXMouseOverColorFade))]
public class UXBucket : UXGUIElement
{
	public delegate void OnDropDelegate(GameObject droppedObject);

	public OnDropDelegate OnDropInto;

	public override void Awake()
	{
		base.Awake();
		UXUtils.AddComponentIfNotExists<MeshFilter>(((Component)this).gameObject).mesh = BuildMesh();
		UXUtils.AddComponentIfNotExists<BoxCollider>(((Component)this).gameObject);
		UXMouseOverColorFade component = ((Component)this).gameObject.GetComponent<UXMouseOverColorFade>();
		component.materials.Add(((Component)this).renderer.material);
		UXDropObject component2 = ((Component)this).gameObject.GetComponent<UXDropObject>();
		component2.AcceptDrop = (UXDropObject.AcceptDropDelegate)Delegate.Combine(component2.AcceptDrop, (UXDropObject.AcceptDropDelegate)((GameObject drop) => true));
		component2.OnDrop = (UXDropObject.OnDropDelegate)Delegate.Combine(component2.OnDrop, new UXDropObject.OnDropDelegate(OnObjectDrop));
	}

	private void OnObjectDrop(GameObject droppedObject)
	{
		NotifyOnDrop(droppedObject);
	}

	private void NotifyOnDrop(GameObject droppedObject)
	{
		if (OnDropInto != null && (Object)(object)droppedObject != (Object)null)
		{
			OnDropInto(droppedObject);
		}
	}
}
