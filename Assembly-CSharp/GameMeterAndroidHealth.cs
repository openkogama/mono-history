using UnityEngine;
using UnityEngine.UI;

public class GameMeterAndroidHealth : GameMeterAndroidBase
{
	[SerializeField]
	private Image HealthMeter;

	[SerializeField]
	private ProgressBarAndroid progressBar;

	public override GameMeterType GameMeterType => GameMeterType.Health;

	public override void UpdateShowGameMeter()
	{
		MVAvatar avatar = MVGameControllerBase.Game.LocalPlayer.Avatar;
		if (avatar != null)
		{
			progressBar.Progress = avatar.Health.Value / 100f;
		}
	}

	public override void SetShowGameMeter(bool show)
	{
		HealthMeter.enabled = show;
		progressBar.enabled = show;
	}
}
