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
		if (freeButtons.Count == 0)
		{
			BuildButton();
		}
		GameObject gameObject = freeButtons.First();
		freeButtons.Remove(gameObject);
		gameObject.SetActive(value: true);
		gameObject.transform.localPosition = new Vector3(0f, 0f - ((float)buttons * buttonHeight + (float)buttons * 2f), 0f);
		gameObject.GetComponentsInChildren<UXText>(includeInactive: true)[0].Text = text;
		UXMouseClickObject component = gameObject.GetComponent<UXMouseClickObject>();
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
		GameObject gameObject = Object.Instantiate(rightClickButtonPrefab);
		UXPlane component = gameObject.GetComponent<UXPlane>();
		if (buttonHeight == 0f)
		{
			buttonHeight = component.Height;
		}
		if (buttonWidth == 0f)
		{
			buttonWidth = component.Width;
		}
		gameObject.GetComponent<UXMouseOverHighlight>().AddMaterial(gameObject.GetComponent<Renderer>().material);
		gameObject.transform.parent = buttonRoot;
		gameObject.transform.localScale = Vector3.one;
		freeButtons.Add(gameObject);
	}

	private void AlignButtonRoot()
	{
		float num = buttonHeight * (float)buttons + 2f * (float)buttons;
		buttonRoot.localPosition = new Vector3(0f, num / 2f, 0f);
	}

	private void CreateButtonRoot()
	{
		Transform transform = new GameObject("ButtonRoot").transform;
		transform.parent = base.transform;
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
		buttonRoot = transform;
	}

	protected override void UpdateVisibility()
	{
		base.gameObject.SetActive(Visible);
		if (!Visible)
		{
			buttons = 0;
			freeButtons.Clear();
			for (int i = 0; i < buttonRoot.childCount; i++)
			{
				GameObject gameObject = buttonRoot.GetChild(i).gameObject;
				gameObject.SetActive(value: false);
				freeButtons.Add(gameObject);
			}
		}
	}
}
