using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MVGUIRightClickGizmo : MVGUIGizmoBase
{
	public delegate void OnGizmoMouseDownDelegate();

	private const float BUTTON_SPACE = 2f;

	public OnGizmoMouseDownDelegate OnGizmoMouseDown;

	public GameObject rightClickButtonPrefab;

	private List<GameObject> freeButtons = new List<GameObject>();

	private Transform buttonRoot;

	private int buttons;

	private float buttonWidth;

	private float buttonHeight;

	protected override void InitializeGizmo()
	{
		base.InitializeGizmo();
		CreateButtonRoot();
	}

	public void AddButton(string text, GizmoClickDelegate onGizmoClick)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (freeButtons.Count == 0)
		{
			BuildButton();
		}
		GameObject val = freeButtons.First();
		freeButtons.Remove(val);
		val.SetActiveRecursively(true);
		val.transform.localPosition = new Vector3(0f, 0f - ((float)buttons * buttonHeight + (float)buttons * 2f), 0f);
		val.GetComponentInChildren<UXText>().Text = text;
		UXMouseClickObject component = val.GetComponent<UXMouseClickObject>();
		component.OnMouseUp = (UXMouseClickObject clickObject, Vector3 mousePos) =>
		{
			onGizmoClick();
			Visible = false;
		};
		component.OnMouseDown = (UXMouseClickObject clickObject, Vector3 mousePos) =>
		{
			if (OnGizmoMouseDown != null)
			{
				OnGizmoMouseDown();
			}
			return false;
		};
		AlignButtonRoot();
		buttons++;
	}

	private void BuildButton()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)rightClickButtonPrefab);
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		UXPlane component = val2.GetComponent<UXPlane>();
		if (buttonHeight == 0f)
		{
			buttonHeight = component.Height;
		}
		if (buttonWidth == 0f)
		{
			buttonWidth = component.Width;
		}
		val2.GetComponent<UXMouseOverHighlight>().AddMaterial(val2.renderer.material);
		val2.transform.parent = buttonRoot;
		val2.transform.localScale = Vector3.one;
		freeButtons.Add(val2);
	}

	private void AlignButtonRoot()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		float num = buttonHeight * (float)buttons + 2f * (float)buttons;
		buttonRoot.localPosition = new Vector3(0f, num / 2f, 0f);
	}

	private void CreateButtonRoot()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = new GameObject("ButtonRoot").transform;
		transform.parent = ((Component)this).transform;
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
		buttonRoot = transform;
	}

	protected override void UpdateVisibility()
	{
		((Component)this).gameObject.active = Visible;
		if (!Visible)
		{
			buttons = 0;
			freeButtons.Clear();
			for (int i = 0; i < buttonRoot.childCount; i++)
			{
				GameObject gameObject = ((Component)buttonRoot.GetChild(i)).gameObject;
				gameObject.SetActiveRecursively(false);
				freeButtons.Add(gameObject);
			}
		}
	}
}
