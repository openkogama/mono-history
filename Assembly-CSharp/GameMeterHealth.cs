using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterHealth : GameMeterBase
{
	[SerializeField]
	private Image HealthMeter;

	[SerializeField]
	private ProgressBarAndroid progressBar;

	private MVAvatar avatarLocal;

	private bool initialized;

	public override GameMeterType GameMeterType => GameMeterType.Health;

	public override void SetGameMeterVisibility()
	{
		if (!initialized)
		{
			avatarLocal = MVGameControllerBase.Game.LocalPlayer.Avatar;
			MVRuntimeDataVariableClampedFloat health = avatarLocal.Health;
			health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnProgressUpdate));
			enabled = true;
			initialized = true;
			OnProgressUpdate(avatarLocal.Health.Value);
		}
	}

	public override void UpdateValue()
	{
	}

	private void OnProgressUpdate(object newValue)
	{
		progressBar.Progress = (float)newValue / 100f;
		for (int i = 0; i < gameMeterVisualEffects.Count; i++)
		{
			gameMeterVisualEffects[i].ExecuteEffect();
		}
	}

	public override void SetShowGameMeter(bool show)
	{
		HealthMeter.enabled = show;
		progressBar.enabled = show;
	}
}
