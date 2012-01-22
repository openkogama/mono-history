using System;
using System.Collections;
using UnityEngine;

public class MVGUILightSettingsDialog : UXViewScript
{
	public MeshRenderer colorCube;

	public UXSlider redSlider;

	public UXSlider greenSlider;

	public UXSlider blueSlider;

	public UXSlider rangeSlider;

	public UXSlider intensitySlider;

	public UXButton okButton;

	public UXButton cancelButton;

	private MVWorldObjectClient wo;

	private float r;

	private float g;

	private float b;

	public static MVGUILightSettingsDialog New(MVWorldObjectClient wo)
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/Box Settings Dialogs/LightSettingsDialog"));
		MVGUILightSettingsDialog dialog = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUILightSettingsDialog>();
		dialog.wo = wo;
		UXView uXView = dialog.View;
		uXView.OnHide = (UXView.OnHideDelegate)Delegate.Combine(uXView.OnHide, (UXView.OnHideDelegate)(() =>
		{
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(dialog);
			Object.Destroy((Object)(object)((Component)dialog).gameObject);
			MVGameController.Instance.EditorController.LeaveContextMenuState();
		}));
		UXFullscreenColliderBox.Instance.AddBlockingObject(dialog);
		return dialog;
	}

	public void Start()
	{
		okButton.OnClick = OKButtonOnClick;
		cancelButton.OnClick = CancelButtonOnClick;
		View.Initialize();
		float[] array = wo.Data["color"] as float[];
		r = array[0];
		g = array[1];
		b = array[2];
		redSlider.Value = r;
		greenSlider.Value = g;
		blueSlider.Value = b;
		UpdateColor();
		rangeSlider.Value = (float)wo.Data["range"];
		intensitySlider.Value = (float)wo.Data["intensity"];
		UXSlider uXSlider = redSlider;
		uXSlider.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			r = v;
			UpdateColor();
		}));
		UXSlider uXSlider2 = greenSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			g = v;
			UpdateColor();
		}));
		UXSlider uXSlider3 = blueSlider;
		uXSlider3.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider3.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			b = v;
			UpdateColor();
		}));
		UXSlider uXSlider4 = redSlider;
		uXSlider4.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider4.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			r = s.Value;
			UpdateColor();
		}));
		UXSlider uXSlider5 = greenSlider;
		uXSlider5.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider5.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			g = s.Value;
			UpdateColor();
		}));
		UXSlider uXSlider6 = blueSlider;
		uXSlider6.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider6.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			b = s.Value;
			UpdateColor();
		}));
	}

	private void UpdateColor()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		((Renderer)colorCube).sharedMaterial.color = new Color(r, g, b);
	}

	public void OKButtonOnClick()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["color"] = new float[3] { r, g, b };
		hashtable["range"] = rangeSlider.Value;
		hashtable["intensity"] = intensitySlider.Value;
		MVGameController.Instance.Game.UpdateWorldObjectData(wo.Id, hashtable);
		View.Hide();
	}

	public void CancelButtonOnClick()
	{
		View.Hide();
	}
}
