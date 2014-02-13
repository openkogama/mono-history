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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		this.on = on;
		((Component)this).renderer.material.SetColor("_MainColor", initialColor * ((!on) ? 1f : 2f));
		if (OnToggle != null)
		{
			OnToggle(this);
		}
	}

	public override void Awake()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.Awake();
		UXMouseClickObject component = ((Component)this).GetComponent<UXMouseClickObject>();
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, new UXMouseClickObject.OnClickDelegate(OnClick));
		initialColor = ((Component)this).renderer.material.GetColor("_MainColor");
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
