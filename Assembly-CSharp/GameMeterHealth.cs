using UnityEngine;

public class GameMeterHealth : GameMeterBase
{
	[SerializeField]
	private GameObject HealthMeter;

	[SerializeField]
	private ProgressBar progressBar;

	private float healthMultiplier = 1f;

	private bool initialized;

	public override GameMeterType GameMeterType => GameMeterType.Health;

	public override void Initialize()
	{
		MVGameControllerBase.SpawnRoleDataMediatorLocal.Health.OnChange += OnProgressUpdate;
		MVGameControllerBase.Game.LocalPlayer.BoostController.SubscribeToBoostChanged(BoostType.ExtraHealthFloatMultiplier, SetupBoostedHealthMultiplier);
		SetupBoostedHealthMultiplier();
		enabled = true;
		initialized = true;
		OnProgressUpdate(MVGameControllerBase.SpawnRoleDataMediatorLocal.Health.Value);
	}

	public override void SetGameMeterVisibility()
	{
	}

	public override void UpdateValue()
	{
	}

	private void SetupBoostedHealthMultiplier()
	{
		healthMultiplier = 1f;
		if (MVGameControllerBase.Game.LocalPlayer.BoostController.TryGetActiveBoost(BoostType.ExtraHealthFloatMultiplier, out var boost))
		{
			healthMultiplier = (float)boost.Value;
		}
	}

	private void OnProgressUpdate(float newValue)
	{
		progressBar.Progress = newValue / GetBoostedHealth(100f);
		for (int i = 0; i < gameMeterVisualEffects.Count; i++)
		{
			gameMeterVisualEffects[i].ExecuteEffect();
		}
	}

	protected float GetBoostedHealth(float defaultHealth)
	{
		return defaultHealth * healthMultiplier;
	}

	public override void SetShowGameMeter(bool show)
	{
		HealthMeter.SetActive(show);
	}
}
