using UnityEngine;

public class PlayControllerTourist : PlayControllerBase
{
	private MVGUILogo guiLogo;

	private MVGUITouristRegister touristRegister;

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		GameObject gameObject = UXUtils.FindGUIObjectOfType<MVGUIPlayModeTourist>().gameObject;
		bottomCenterToggles = AIngameController.FindGUIObjectOfType<MVGUIBottomCenterToggles>(gameObject);
		guiLogo = AIngameController.FindGUIObjectOfType<MVGUILogo>(gameObject);
		touristRegister = AIngameController.FindGUIObjectOfType<MVGUITouristRegister>(gameObject);
	}

	protected override void HideLostFocusGUI()
	{
		base.HideLostFocusGUI();
		guiLogo.View.Show();
		touristRegister.View.Hide();
	}

	protected override void ShowLostFocusGUI()
	{
		base.ShowLostFocusGUI();
		guiLogo.View.Hide();
		touristRegister.View.Show();
	}
}
