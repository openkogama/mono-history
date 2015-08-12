using System;
using UnityEngine;

public class MVGUISelectionGizmo : MVGUIGizmoBase
{
	public GizmoClickDelegate OnRotate;

	public GizmoClickDelegate OnXZtranslate;

	public GizmoClickDelegate OnYtranslate;

	public UXIconButton rotate;

	public UXIconButton xzTranslate;

	public UXIconButton yTranslateUp;

	public UXIconButton yTranslateDown;

	protected override void InitializeGizmo()
	{
		base.InitializeGizmo();
		UXMouseClickObject component = rotate.GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) => FireGizmoDelegate(OnRotate)));
		UXMouseClickObject component2 = xzTranslate.GetComponent<UXMouseClickObject>();
		component2.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component2.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) => FireGizmoDelegate(OnXZtranslate)));
		UXMouseClickObject component3 = yTranslateUp.GetComponent<UXMouseClickObject>();
		component3.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component3.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) => FireGizmoDelegate(OnYtranslate)));
		UXMouseClickObject component4 = yTranslateDown.GetComponent<UXMouseClickObject>();
		component4.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component4.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) => FireGizmoDelegate(OnYtranslate)));
		InitializeMouseOverListeners();
	}

	private bool FireGizmoDelegate(GizmoClickDelegate gizmoClick)
	{
		gizmoClick?.Invoke();
		return false;
	}

	protected override void UpdateVisibility()
	{
		rotate.SetVisible(Visible && rotate.buttonEnabled);
		xzTranslate.SetVisible(Visible && xzTranslate.buttonEnabled);
		yTranslateUp.SetVisible(Visible && yTranslateUp.buttonEnabled);
		yTranslateDown.SetVisible(Visible && yTranslateDown.buttonEnabled);
	}

	private void InitializeMouseOverListeners()
	{
		UXMouseOverObject component = rotate.GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverEnter, (UXMouseOverObject.OnMouseOverDelegate)((UXMouseOverObject mouseOverObject) =>
		{
			xzTranslate.GetComponent<UXMouseOverColorFade>().OnMouseOverExit(mouseOverObject);
			yTranslateUp.GetComponent<UXMouseOverColorFade>().OnMouseOverExit(mouseOverObject);
		}));
		UXMouseOverObject component2 = xzTranslate.GetComponent<UXMouseOverObject>();
		component2.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component2.OnMouseOverEnter, (UXMouseOverObject.OnMouseOverDelegate)((UXMouseOverObject mouseOverObject) =>
		{
			rotate.GetComponent<UXMouseOverColorFade>().OnMouseOverExit(mouseOverObject);
			yTranslateUp.GetComponent<UXMouseOverColorFade>().OnMouseOverExit(mouseOverObject);
		}));
		UXMouseOverObject component3 = xzTranslate.GetComponent<UXMouseOverObject>();
		component3.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component3.OnMouseOverExit, (UXMouseOverObject.OnMouseOverDelegate)((UXMouseOverObject mouseOverObject) =>
		{
			rotate.GetComponent<UXMouseOverColorFade>().OnMouseOverEnter(mouseOverObject);
		}));
		UXMouseOverObject component4 = yTranslateUp.GetComponent<UXMouseOverObject>();
		component4.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component4.OnMouseOverEnter, (UXMouseOverObject.OnMouseOverDelegate)((UXMouseOverObject mouseOverObject) =>
		{
			yTranslateDown.GetComponent<UXMouseOverColorFade>().OnMouseOverEnter(mouseOverObject);
			rotate.GetComponent<UXMouseOverColorFade>().OnMouseOverExit(mouseOverObject);
			xzTranslate.GetComponent<UXMouseOverColorFade>().OnMouseOverExit(mouseOverObject);
		}));
		UXMouseOverObject component5 = yTranslateUp.GetComponent<UXMouseOverObject>();
		component5.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component5.OnMouseOverExit, (UXMouseOverObject.OnMouseOverDelegate)((UXMouseOverObject mouseOverObject) =>
		{
			yTranslateDown.GetComponent<UXMouseOverColorFade>().OnMouseOverExit(mouseOverObject);
		}));
		UXMouseOverObject component6 = yTranslateDown.GetComponent<UXMouseOverObject>();
		component6.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component6.OnMouseOverEnter, (UXMouseOverObject.OnMouseOverDelegate)((UXMouseOverObject mouseOverObject) =>
		{
			yTranslateUp.GetComponent<UXMouseOverColorFade>().OnMouseOverEnter(mouseOverObject);
		}));
		UXMouseOverObject component7 = yTranslateDown.GetComponent<UXMouseOverObject>();
		component7.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component7.OnMouseOverExit, (UXMouseOverObject.OnMouseOverDelegate)((UXMouseOverObject mouseOverObject) =>
		{
			yTranslateUp.GetComponent<UXMouseOverColorFade>().OnMouseOverExit(mouseOverObject);
		}));
	}
}
