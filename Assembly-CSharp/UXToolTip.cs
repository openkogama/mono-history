using System;
using Localize;
using UnityEngine;

[RequireComponent(typeof(UXMouseOverObject))]
public class UXToolTip : MonoBehaviour
{
	private UXToolTipObject toolTipObject;

	public TextSlotIndex toolTipTextID;

	public string toolTipText;

	public bool HitOnDragOver;

	private void Awake()
	{
		UXMouseOverObject component = ((Component)this).GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverEnter));
		component.OnMouseOver = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOver, new UXMouseOverObject.OnMouseOverDelegate(HandleOnMouseOver));
		toolTipObject = UXUtils.FindGUIObjectOfType<UXToolTipObject>();
		UXDropObject uXDropObject = ((Component)this).GetComponent<UXDropObject>();
		if ((Object)(object)uXDropObject == (Object)null)
		{
			uXDropObject = ((Component)this).gameObject.AddComponent<UXDropObject>();
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
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (toolTipText == null || toolTipText == string.Empty)
		{
			toolTipText = ToolTipText.Instance.GetToolTipText(toolTipTextID);
		}
		toolTipObject.ReadyToolTip(toolTipText, Input.mousePosition);
	}

	public void HandleOnMouseOver(UXMouseOverObject mouseOverObject)
	{
		toolTipObject.HandleOnMouseOver();
	}
}
