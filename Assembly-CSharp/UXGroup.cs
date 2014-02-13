using System.Collections.Generic;
using UnityEngine;

public class UXGroup : UXGUIElement
{
	public delegate void OnGroupEventDelegate();

	public OnGroupEventDelegate OnShowGroup;

	public OnGroupEventDelegate OnHideGroup;

	public void Start()
	{
		if (Visible)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	public override void SetVisible(bool visible)
	{
		Visible = visible;
		foreach (UXGUIElement immediateChild in GetImmediateChildren())
		{
			immediateChild.SetVisible(visible);
		}
		if (Visible && OnShowGroup != null)
		{
			OnShowGroup();
		}
		if (!Visible && OnHideGroup != null)
		{
			OnHideGroup();
		}
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		foreach (UXGUIElement immediateChild in GetImmediateChildren())
		{
			immediateChild.SetAlpha(alpha, materialProperty);
		}
	}

	private List<UXGUIElement> GetImmediateChildren()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected Obj, but got Unknown
		List<UXGUIElement> list = new List<UXGUIElement>();
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val = item;
			UXGUIElement component = ((Component)val).gameObject.GetComponent<UXGUIElement>();
			if ((Object)(object)component != (Object)null)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public void Hide()
	{
		SetVisible(visible: false);
	}

	public void Show()
	{
		SetVisible(visible: true);
	}
}
