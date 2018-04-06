using System;
using UnityEngine;

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
			MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
			localPlayer.OnInitializeLeveling = (Action)Delegate.Combine(localPlayer.OnInitializeLeveling, new Action(LateInitialize));
			progress.Progress = 0f;
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
			float num = (previousXPProgress = Mathf.Lerp(previousXPProgress, interpolateTowardsXPProgress, elapsedInterpolationTime));
			progress.Progress = num;
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
		progress.enabled = show;
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
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		localPlayer.OnInitializeLeveling = (Action)Delegate.Remove(localPlayer.OnInitializeLeveling, new Action(LateInitialize));
	}
}
