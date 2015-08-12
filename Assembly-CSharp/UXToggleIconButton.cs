using System;
using UnityEngine;

public class UXToggleIconButton : UXIconButton
{
	public delegate void OnToggleDelegate(bool state);

	public OnToggleDelegate OnToggle;

	public Material materialOn;

	public Material materialOff;

	[SerializeField]
	private bool _toggleState;

	public bool ToggleState
	{
		get
		{
			return _toggleState;
		}
		set
		{
			if (value != _toggleState)
			{
				_toggleState = value;
				UpdateMaterial();
				NotifyOnToggle();
			}
		}
	}

	protected override void Initialize()
	{
		materialOn = new Material(materialOn);
		materialOff = new Material(materialOff);
		base.Initialize();
		UpdateMaterial();
		ColorIcon(color);
		OnClick = (OnClickDelegate)Delegate.Combine(OnClick, new OnClickDelegate(OnBaseClick));
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		Color color = materialOn.GetColor(materialProperty);
		Color color2 = materialOff.GetColor(materialProperty);
		color.a = alpha;
		color2.a = alpha;
		materialOn.SetColor(materialProperty, color);
		materialOff.SetColor(materialProperty, color2);
		UpdateMaterial();
	}

	private void OnBaseClick()
	{
		Toggle();
	}

	public void Toggle()
	{
		_toggleState = !_toggleState;
		UpdateMaterial();
		NotifyOnToggle();
	}

	public void SetToggleState(bool toggle)
	{
		_toggleState = toggle;
		UpdateMaterial();
	}

	private void NotifyOnToggle()
	{
		if (OnToggle != null)
		{
			OnToggle(_toggleState);
		}
	}

	private void UpdateMaterial()
	{
		GetComponent<Renderer>().material = ((!_toggleState) ? materialOff : materialOn);
		UXMouseOverColorFade component = GetComponent<UXMouseOverColorFade>();
		if (component != null)
		{
			component.materials.Clear();
			component.materials.Add(GetComponent<Renderer>().material);
			component.UpdateMaterials();
		}
	}
}
