using System;
using UnityEngine;
using UnityEngine.Events;

public class GameMeterXP : GameMeterBase
{
	[SerializeField]
	private GameObject XPMeter;

	private float interpolateTowardsXPProgress;

	private float previousXPProgress;

	private float elapsedInterpolationTime = 1f;

	public override GameMeterType GameMeterType => GameMeterType.XP;

	public override void Initialize()
	{
		if (LevelingManager.IsInitialized && MVGameControllerBase.Game.LocalPlayer.CanGetXPProgressData)
		{
			Init();
		}
		else
		{
			LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, new UnityAction(Init));
		}
	}

	private void Init()
	{
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		localPlayer.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnProgressUpdate));
		enabled = true;
		LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Remove(LevelingManager.OnLevelingInitialized, new UnityAction(Init));
		OnProgressUpdate(MVGameControllerBase.Game.LocalPlayer.XPProgressData);
	}

	public override void SetGameMeterVisibility()
	{
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
}
