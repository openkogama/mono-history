using System;
using UnityEngine;

public class DropBox : MonoBehaviour
{
	private Color initialColor;

	public void Awake()
	{
		UXDropObject component = ((Component)this).GetComponent<UXDropObject>();
		component.AcceptDrop = (UXDropObject.AcceptDropDelegate)Delegate.Combine(component.AcceptDrop, new UXDropObject.AcceptDropDelegate(AcceptDrop));
		component.OnDragOverEnter = (UXDropObject.OnDragOverEnterDelegate)Delegate.Combine(component.OnDragOverEnter, new UXDropObject.OnDragOverEnterDelegate(OnDragOverEnter));
		component.OnDragOverExit = (UXDropObject.OnDragOverExitDelegate)Delegate.Combine(component.OnDragOverExit, new UXDropObject.OnDragOverExitDelegate(OnDragOverExit));
		component.OnDrop = (UXDropObject.OnDropDelegate)Delegate.Combine(component.OnDrop, new UXDropObject.OnDropDelegate(OnDrop));
	}

	private void Start()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		initialColor = ((Component)this).renderer.material.GetColor("_MainColor");
	}

	private bool AcceptDrop(GameObject gameObject)
	{
		return true;
	}

	private void OnDragOverEnter(GameObject gameObject)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).renderer.material.SetColor("_MainColor", initialColor * 1.5f);
	}

	private void OnDragOverExit(GameObject gameObject)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).renderer.material.SetColor("_MainColor", initialColor);
	}

	private void OnDrop(GameObject gameObject)
	{
		Debug.Log((object)(((Object)gameObject).name + " dropped."));
	}
}
