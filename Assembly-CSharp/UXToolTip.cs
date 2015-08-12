using System;
using UnityEngine;

[RequireComponent(typeof(UXMouseOverObject))]
public class UXToolTip : MonoBehaviour
{
	private UXToolTipObject toolTipObject;

	public string toolTipText;

	public bool HitOnDragOver;

	private void Start()
	{
		UXMouseOverObject component = GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverEnter));
		component.OnMouseOver = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOver, new UXMouseOverObject.OnMouseOverDelegate(HandleOnMouseOver));
		toolTipObject = UXUtils.FindGUIObjectOfType<UXToolTipObject>();
		UXDropObject uXDropObject = GetComponent<UXDropObject>();
		if (uXDropObject == null)
		{
			uXDropObject = gameObject.AddComponent<UXDropObject>();
		}
		UXDropObject uXDropObject2 = uXDropObject;
		uXDropObject2.OnDragOverEnter = (UXDropObject.OnDragOverEnterDelegate)Delegate.Combine(uXDropObject2.OnDragOverEnter, (UXDropObject.OnDragOverEnterDelegate)((GameObject g) =>
		{
			OnMouseOverEnter(null);
		}));
		UXDropObject uXDropObject3 = uXDropObject;
		uXDropObject3.OnDragOver = (UXDropObject.OnDragOverDelegate)Delegate.Combine(uXDropObject3.OnDragOver, (UXDropObject.OnDragOverDelegate)((GameObject g) =>
		{
			HandleOnMouseOver(null);
		}));
	}

	public void OnMouseOverEnter(UXMouseOverObject mouseOverObject)
	{
		toolTipObject.ReadyToolTip(TM._(toolTipText), MVInputWrapper.GetPointerPosition());
	}

	public void HandleOnMouseOver(UXMouseOverObject mouseOverObject)
	{
		toolTipObject.HandleOnMouseOver();
	}
}
