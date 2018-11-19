using System;
using UnityEngine;

public class GameMeterHealth : GameMeterBase
{
	[SerializeField]
	private GameObject HealthMeter;

	[SerializeField]
	private ProgressBar progressBar;

	private MVAvatar avatarLocal;

	private bool initialized;

	public override GameMeterType GameMeterType => GameMeterType.Health;

	public override void SetGameMeterVisibility()
	{
		if (!initialized)
		{
			if (MVGameControllerBase.WOCM.AvatarLocal == null)
			{
				MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
				mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerReady, new Action(LateInitialize));
			}
			else
			{
				Initialize();
			}
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
		HealthMeter.SetActive(show);
	}

	private void Initialize()
	{
		avatarLocal = MVGameControllerBase.Game.LocalPlayer.Avatar;
		MVRuntimeDataVariableClampedFloat health = avatarLocal.Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnProgressUpdate));
		enabled = true;
		initialized = true;
		OnProgressUpdate(avatarLocal.Health.Value);
	}

	private void LateInitialize()
	{
		Initialize();
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(Initialize));
	}
}
