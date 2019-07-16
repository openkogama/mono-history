using UnityEngine;

public class GameMeterHealth : GameMeterBase
{
	[SerializeField]
	private GameObject HealthMeter;

	[SerializeField]
	private ProgressBar progressBar;

	private int maxValue = 100;

	public override GameMeterType GameMeterType => GameMeterType.Health;

	public override void SetGameMeterVisibility()
	{
	}

	public override void UpdateValue()
	{
	}

	public override void SetShowGameMeter(bool show)
	{
		HealthMeter.SetActive(show);
	}

	public override void Initialize()
	{
		MVGameControllerBase.SpawnRoleDataMediatorLocal.Health.OnChange += OnProgressUpdate;
		MVGameControllerBase.SpawnRoleDataMediatorLocal.MaxHealth.OnChange += OnMaxValueUpdate;
		enabled = true;
		OnProgressUpdate(MVGameControllerBase.SpawnRoleDataMediatorLocal.Health.Value);
	}

	private void OnMaxValueUpdate(int maxValue)
	{
		this.maxValue = maxValue;
	}

	private void OnProgressUpdate(float newValue)
	{
		progressBar.Progress = newValue / (float)maxValue;
		for (int i = 0; i < gameMeterVisualEffects.Count; i++)
		{
			gameMeterVisualEffects[i].ExecuteEffect();
		}
	}
}
