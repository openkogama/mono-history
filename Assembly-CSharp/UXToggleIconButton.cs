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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected Obj, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		materialOn = new Material(materialOn);
		materialOff = new Material(materialOff);
		base.Initialize();
		UpdateMaterial();
		ColorIcon(color);
		OnClick = (OnClickDelegate)Delegate.Combine(OnClick, new OnClickDelegate(OnBaseClick));
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Color val = materialOn.GetColor(materialProperty);
		Color val2 = materialOff.GetColor(materialProperty);
		val.a = alpha;
		val2.a = alpha;
		materialOn.SetColor(materialProperty, val);
		materialOff.SetColor(materialProperty, val2);
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
		((Component)this).renderer.material = ((!_toggleState) ? materialOff : materialOn);
		UXMouseOverColorFade component = ((Component)this).GetComponent<UXMouseOverColorFade>();
		if ((Object)(object)component != (Object)null)
		{
			component.materials.Clear();
			component.materials.Add(((Component)this).renderer.material);
			component.UpdateMaterials();
		}
	}
}
