using UnityEngine;

public class PlayControllerTourist : PlayControllerBase
{
	private MVGUILogo guiLogo;

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		GameObject gameObject = UXUtils.FindGUIObjectOfType<MVGUIPlayModeTourist>().gameObject;
		bottomCenterToggles = AIngameController.FindGUIObjectOfType<MVGUIBottomCenterToggles>(gameObject);
		guiLogo = AIngameController.FindGUIObjectOfType<MVGUILogo>(gameObject);
	}

	protected override void HideLostFocusGUI()
	{
		base.HideLostFocusGUI();
		guiLogo.View.Show();
	}

	protected override void ShowLostFocusGUI()
	{
		base.ShowLostFocusGUI();
		guiLogo.View.Hide();
	}
}
