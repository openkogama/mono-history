using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelBadge : MonoBehaviour
{
	[SerializeField]
	private RawImage levelBadge;

	[SerializeField]
	private ProgressBarAndroid xpBar;

	private void Awake()
	{
		levelBadge.enabled = false;
		xpBar.gameObject.SetActive(value: false);
		if (LevelingManager.IsInitialized)
		{
			OnLevelingInitialized();
		}
		else
		{
			LevelingManager.OnLevelingInitialized = (LevelingManager.OnlevelingInitializedDelegate)Delegate.Combine(LevelingManager.OnLevelingInitialized, new LevelingManager.OnlevelingInitializedDelegate(OnLevelingInitialized));
		}
	}

	private void OnLevelingInitialized()
	{
		UpdateBadge(MVGameControllerBase.Game.LocalPlayer.Level);
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		localPlayer.OnLevelChanged = (MVPlayer.OnLevelChangedDelegate)Delegate.Combine(localPlayer.OnLevelChanged, new MVPlayer.OnLevelChangedDelegate(UpdateBadge));
		UpdateProgress(MVGameControllerBase.Game.LocalPlayer.XPProgressData);
		MVLocalPlayer localPlayer2 = MVGameControllerBase.Game.LocalPlayer;
		localPlayer2.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer2.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(UpdateProgress));
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
		xpBar.Progress = num;
	}

	private void UpdateBadge(int level)
	{
		BadgeManager.GetBadgeTexture(level, StreamingAssetCallback);
	}

	private void StreamingAssetCallback(WWW www)
	{
		if (www != null && www.texture != null)
		{
			levelBadge.enabled = true;
			xpBar.gameObject.SetActive(value: true);
			levelBadge.texture = www.texture;
		}
	}
}
