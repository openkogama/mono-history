using System;
using UnityEngine;

[RequireComponent(typeof(UXMouseOverObject))]
[RequireComponent(typeof(UXMouseClickObject))]
[AddComponentMenu("UX/Collections/Collection view slot")]
public class UXCollectionViewSlot : MonoBehaviour
{
	public delegate void SlotEventDelegate(int slotIndex);

	public SlotEventDelegate OnSlotMouseOver;

	public SlotEventDelegate OnSlotMouseDown;

	private int slotIndex;

	public int SlotIndex
	{
		get
		{
			return slotIndex;
		}
		set
		{
			slotIndex = value;
		}
	}

	public void Start()
	{
		UXMouseOverObject component = ((Component)this).GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverEnterDelegate)Delegate.Combine(component.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverEnterDelegate(HandleMouseOver));
		UXMouseClickObject component2 = ((Component)this).GetComponent<UXMouseClickObject>();
		component2.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component2.OnClick, new UXMouseClickObject.OnClickDelegate(HandleMouseClick));
	}

	public void HandleMouseOver(UXMouseOverObject mouseOverObject)
	{
		if (OnSlotMouseOver != null)
		{
			OnSlotMouseOver(slotIndex);
		}
	}

	public void HandleMouseClick(UXMouseClickObject mouseClickObject, Vector3 mousePositionWorld)
	{
		if (OnSlotMouseDown != null)
		{
			OnSlotMouseDown(slotIndex);
		}
	}
}
