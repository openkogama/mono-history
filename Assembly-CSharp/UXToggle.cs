using System;
using UnityEngine;

[AddComponentMenu("UX/Elements/Toggle")]
public class UXToggle : UXGUIElement
{
	public delegate void OnToggleDelegate(UXToggle toggle);

	public bool on;

	public OnToggleDelegate OnToggle;

	private Color initialColor;

	public bool On
	{
		get
		{
			return on;
		}
		set
		{
			if (on != value)
			{
				ChangeOn(value);
			}
		}
	}

	private void ChangeOn(bool on)
	{
		this.on = on;
		GetComponent<Renderer>().material.SetColor("_MainColor", initialColor * ((!on) ? 1f : 2f));
		if (OnToggle != null)
		{
			OnToggle(this);
		}
	}

	public override void Awake()
	{
		base.Awake();
		UXMouseClickObject component = GetComponent<UXMouseClickObject>();
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, new UXMouseClickObject.OnClickDelegate(OnClick));
		initialColor = GetComponent<Renderer>().material.GetColor("_MainColor");
	}

	public void Start()
	{
		ChangeOn(on);
	}

	private void OnClick(UXMouseClickObject clickObject, Vector3 mousePositionWorld)
	{
		On = !On;
	}
}
