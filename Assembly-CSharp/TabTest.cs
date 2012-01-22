using System;
using UnityEngine;

public class TabTest : UXViewScript
{
	public UXTabWindow tabWindow;

	public override void OnInitialize()
	{
		UXTabWindow uXTabWindow = tabWindow;
		uXTabWindow.OnTabSelect = (UXTabWindow.OnTabSelectedDelegate)Delegate.Combine(uXTabWindow.OnTabSelect, (UXTabWindow.OnTabSelectedDelegate)((int tabID) =>
		{
			Debug.Log((object)("Tab selected: " + tabID));
		}));
	}
}
