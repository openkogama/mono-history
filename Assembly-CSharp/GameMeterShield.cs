using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterShield : GameMeterBase
{
	[SerializeField]
	private Image ShieldMeter;

	[SerializeField]
	private ProgressBar progressBar;

	private MVAvatar avatarLocal;

	private bool initialized;

	private const float maxShieldValue = 100f;

	private float interpolateTowardsShieldProgress;

	private float previousShieldProgress;

	private float elapsedInterpolationTime;

	public override GameMeterType GameMeterType => GameMeterType.Shield;

	public override void SetGameMeterVisibility()
	{
		if (!initialized)
		{
			avatarLocal = MVGameControllerBase.Game.LocalPlayer.Avatar;
			MVRuntimeDataVariableClampedFloat shield = avatarLocal.Shield;
			shield.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(shield.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnProgressUpdate));
			enabled = true;
			initialized = true;
			OnProgressUpdate(avatarLocal.Shield.Value);
		}
	}

	public override void UpdateValue()
	{
	}

	private void Update()
	{
		elapsedInterpolationTime += Time.deltaTime;
		float progress = (previousShieldProgress = Mathf.Lerp(previousShieldProgress, interpolateTowardsShieldProgress, elapsedInterpolationTime));
		progressBar.Progress = progress;
		for (int i = 0; i < gameMeterVisualEffects.Count; i++)
		{
			gameMeterVisualEffects[i].ExecuteEffect();
		}
	}

	private void OnProgressUpdate(object newValue)
	{
		interpolateTowardsShieldProgress = avatarLocal.Shield.Value / 100f;
		elapsedInterpolationTime = 0f;
	}

	public override void SetShowGameMeter(bool show)
	{
		ShieldMeter.enabled = show;
		progressBar.enabled = show;
	}
}
