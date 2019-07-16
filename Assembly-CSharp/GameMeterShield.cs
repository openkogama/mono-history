using UnityEngine;
using UnityEngine.UI;

public class GameMeterShield : GameMeterBase
{
	[SerializeField]
	private Image ShieldMeter;

	[SerializeField]
	private ProgressBar progressBar;

	private const float maxShieldValue = 100f;

	private float interpolateTowardsShieldProgress;

	private float previousShieldProgress;

	private float elapsedInterpolationTime;

	public override GameMeterType GameMeterType => GameMeterType.Shield;

	public override void Initialize()
	{
		MVGameControllerBase.SpawnRoleDataMediatorLocal.Shield.OnChange += OnProgressUpdate;
		enabled = true;
		OnProgressUpdate(MVGameControllerBase.SpawnRoleDataMediatorLocal.Shield.Value);
	}

	public override void SetGameMeterVisibility()
	{
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

	private void OnProgressUpdate(float newValue)
	{
		interpolateTowardsShieldProgress = newValue / 100f;
		elapsedInterpolationTime = 0f;
	}

	public override void SetShowGameMeter(bool show)
	{
		ShieldMeter.enabled = show;
		progressBar.enabled = show;
	}
}
