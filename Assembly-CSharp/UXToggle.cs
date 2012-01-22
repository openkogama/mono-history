using System;
using UnityEngine;

[AddComponentMenu("UX/Elements/Toggle")]
public class UXToggle : MonoBehaviour
{
	public delegate void OnToggleDelegate(UXToggle toggle);

	public bool on;

	public OnToggleDelegate OnToggle;

	private Color initialColor;

	private bool isInitialized;

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
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!isInitialized)
		{
			Initialize();
		}
		this.on = on;
		((Component)this).renderer.material.SetColor("_MainColor", initialColor * ((!on) ? 1f : 2f));
		if (OnToggle != null)
		{
			OnToggle(this);
		}
	}

	public void Awake()
	{
		if (!isInitialized)
		{
			Initialize();
		}
	}

	public void Initialize()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		UXMouseClickObject component = ((Component)this).GetComponent<UXMouseClickObject>();
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, new UXMouseClickObject.OnClickDelegate(OnClick));
		initialColor = ((Component)this).renderer.material.GetColor("_MainColor");
		isInitialized = true;
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
