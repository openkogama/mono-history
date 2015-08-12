using UnityEngine;

public class GameMeterHealth : GameMeterBase
{
	[SerializeField]
	private MVGUIProgressBar progressBar;

	public override GameMeterType GameMeterType => GameMeterType.Health;

	private void Update()
	{
		MVAvatar avatar = MVGameController.Game.LocalPlayer.Avatar;
		if (avatar != null)
		{
			MeterActive = true;
			progressBar.Percentage = avatar.Health.Value / 100f;
		}
		else
		{
			MeterActive = false;
		}
	}
}
