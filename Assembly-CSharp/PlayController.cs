using UnityEngine;

public class PlayController : AIngameController
{
	public MVGUISpeedometer speedometer;

	public MVGUIReward reward;

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		GameObject playGUI = UXUtils.GetGUIHandler().playGUI;
		speedometer = playGUI.GetComponentInChildren<MVGUISpeedometer>();
		reward = playGUI.GetComponentInChildren<MVGUIReward>();
	}

	public override void ToggleShowUI()
	{
		base.ToggleShowUI();
		if (uiShown)
		{
			speedometer.View.Show();
		}
		else
		{
			speedometer.View.Hide();
		}
		MVGameController.Instance.WOCM.AvatarLocal.ShowHealth = uiShown;
	}

	public override void RemoveUI()
	{
		base.RemoveUI();
		speedometer.View.Hide();
		reward.View.Hide();
	}
}
