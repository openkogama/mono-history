using System;
using UnityEngine;

public class MVGUIProgressXP : MonoBehaviour
{
	[SerializeField]
	private UXText currentXpText;

	[SerializeField]
	private MVGUIProgressBar progressBar;

	private void Awake()
	{
		if (Application.loadedLevelName != "GUIDevScene")
		{
			if (LevelingManager.IsInitialized)
			{
				OnLevelingInitialized();
			}
			else
			{
				LevelingManager.OnLevelingInitialized = (LevelingManager.OnlevelingInitializedDelegate)Delegate.Combine(LevelingManager.OnLevelingInitialized, new LevelingManager.OnlevelingInitializedDelegate(OnLevelingInitialized));
			}
		}
	}

	private void OnLevelingInitialized()
	{
		MVLocalPlayer localPlayer = MVGameController.Game.LocalPlayer;
		localPlayer.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(UpdateProgress));
		UpdateProgress(MVGameController.Game.LocalPlayer.XPProgressData);
	}

	private void UpdateProgress(XPProgressData xpProgress)
	{
		if (xpProgress.XpNextRel <= 0)
		{
			Debug.LogError("Can't calculate update progress as xpNextRel <= 0");
			return;
		}
		float num = (float)xpProgress.XpRel / (float)xpProgress.XpNextRel;
		if (num < 0f)
		{
			Debug.LogError("processPercentage invalid. " + num);
		}
		progressBar.Percentage = Mathf.Clamp01(num);
		currentXpText.Text = xpProgress.XP.ToString();
	}
}
