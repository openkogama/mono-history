using System;
using UnityEngine;

[RequireComponent(typeof(UXMouseOverColorFade))]
[RequireComponent(typeof(UXDropObject))]
public class UXBucket : UXGUIElement
{
	public delegate void OnDropDelegate(GameObject droppedObject);

	public OnDropDelegate OnDropInto;

	public override void Awake()
	{
		base.Awake();
		BuildMesh(UXUtils.AddComponentIfNotExists<MeshFilter>(gameObject).mesh);
		UXUtils.AddComponentIfNotExists<BoxCollider>(gameObject);
		UXMouseOverColorFade component = gameObject.GetComponent<UXMouseOverColorFade>();
		component.materials.Add(GetComponent<Renderer>().material);
		UXDropObject component2 = gameObject.GetComponent<UXDropObject>();
		component2.AcceptDrop = (UXDropObject.AcceptDropDelegate)Delegate.Combine(component2.AcceptDrop, (UXDropObject.AcceptDropDelegate)((GameObject drop) => true));
		component2.OnDrop = (UXDropObject.OnDropDelegate)Delegate.Combine(component2.OnDrop, new UXDropObject.OnDropDelegate(OnObjectDrop));
	}

	private void OnObjectDrop(GameObject droppedObject)
	{
		NotifyOnDrop(droppedObject);
	}

	private void NotifyOnDrop(GameObject droppedObject)
	{
		if (OnDropInto != null && droppedObject != null)
		{
			OnDropInto(droppedObject);
		}
	}
}
