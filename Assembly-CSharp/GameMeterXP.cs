using System;
using UnityEngine;
using UnityEngine.Events;

public class GameMeterXP : GameMeterBase
{
	[SerializeField]
	private GameObject XPMeter;

	private bool initialized;

	private float interpolateTowardsXPProgress;

	private float previousXPProgress;

	private float elapsedInterpolationTime = 1f;

	public override GameMeterType GameMeterType => GameMeterType.XP;

	public override void SetGameMeterVisibility()
	{
		if (!initialized && MVGameControllerBase.Game.LocalPlayer.CanGetXPProgressData)
		{
			Initialize();
		}
		else if (!initialized)
		{
			LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, new UnityAction(LateInitialize));
		}
	}

	public override void UpdateValue()
	{
	}

	private void Update()
	{
		if (elapsedInterpolationTime < 1f + Time.deltaTime)
		{
			elapsedInterpolationTime += Time.deltaTime;
			float num = Mathf.Lerp(previousXPProgress, interpolateTowardsXPProgress, elapsedInterpolationTime);
			previousXPProgress = num;
			for (int i = 0; i < gameMeterVisualEffects.Count; i++)
			{
				gameMeterVisualEffects[i].ExecuteEffect();
			}
		}
	}

	private void OnProgressUpdate(XPProgressData xpProgress)
	{
		for (int i = 0; i < gameMeterVisualEffects.Count; i++)
		{
			gameMeterVisualEffects[i].ExecuteEffect();
		}
		interpolateTowardsXPProgress = xpProgress.XP / xpProgress.NextXP;
		elapsedInterpolationTime = 0f;
	}

	public override void SetShowGameMeter(bool show)
	{
		XPMeter.SetActive(show);
	}

	private void Initialize()
	{
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		localPlayer.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnProgressUpdate));
		enabled = true;
		initialized = true;
		OnProgressUpdate(MVGameControllerBase.Game.LocalPlayer.XPProgressData);
	}

	private void LateInitialize()
	{
		Initialize();
		LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Remove(LevelingManager.OnLevelingInitialized, new UnityAction(LateInitialize));
	}
}
