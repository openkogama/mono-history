using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterAndroidHealth : GameMeterAndroidBase
{
	[SerializeField]
	private Image HealthMeter;

	[SerializeField]
	private ProgressBarAndroid progressBar;

	private MVAvatar avatarLocal;

	public override GameMeterType GameMeterType => GameMeterType.Health;

	public override void SetGameMeterVisibility()
	{
		avatarLocal = MVGameControllerBase.Game.LocalPlayer.Avatar;
		MVRuntimeDataVariableClampedFloat health = avatarLocal.Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnProgressUpdate));
		enabled = true;
	}

	public override void UpdateValue()
	{
	}

	private void OnProgressUpdate(object newValue)
	{
		progressBar.Progress = (float)newValue / 100f;
	}

	public override void SetShowGameMeter(bool show)
	{
		HealthMeter.enabled = show;
		progressBar.enabled = show;
	}
}
