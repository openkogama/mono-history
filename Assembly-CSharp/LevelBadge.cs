using System;
using MV.WorldObject.Subscription;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelBadge : MonoBehaviour
{
	[SerializeField]
	private RawImage levelBadge;

	[SerializeField]
	private ProgressBarAndroid xpBar;

	[SerializeField]
	private ProgressBarAndroid subscriberXPBar;

	private Texture2D badgeTextureAsset;

	private void Awake()
	{
		levelBadge.enabled = false;
		xpBar.gameObject.SetActive(value: false);
		subscriberXPBar.gameObject.SetActive(value: false);
		if (LevelingManager.IsInitialized)
		{
			OnLevelingInitialized();
		}
		else
		{
			LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, new UnityAction(OnLevelingInitialized));
		}
	}

	private void OnLevelingInitialized()
	{
		UpdateBadge(MVGameControllerBase.Game.LocalPlayer.Level);
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		localPlayer.OnLevelChanged = (UnityAction<int>)Delegate.Combine(localPlayer.OnLevelChanged, new UnityAction<int>(UpdateBadge));
		UpdateProgress(MVGameControllerBase.Game.LocalPlayer.XPProgressData);
		MVLocalPlayer localPlayer2 = MVGameControllerBase.Game.LocalPlayer;
		localPlayer2.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer2.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(UpdateProgress));
	}

	private void UpdateProgress(XPProgressData xpProgress)
	{
		if (xpProgress.XpNextRel < 0)
		{
			Debug.LogError("Can't calculate update progress as xpNextRel <= 0");
			return;
		}
		float num = (float)xpProgress.XpRel / (float)xpProgress.XpNextRel;
		if (num < 0f)
		{
			Debug.Log("ProgressPercentage: " + num);
			Debug.LogError("processPercentage invalid.");
		}
		xpBar.Progress = num;
		subscriberXPBar.Progress = num;
	}

	private void UpdateBadge(int level)
	{
		BadgeManager.GetBadgeTexture(level, StreamingAssetCallback);
	}

	private void OnDestroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(StreamingAssetCallback);
		UnityEngine.Object.Destroy(badgeTextureAsset);
	}

	private void StreamingAssetCallback(WWW www)
	{
		badgeTextureAsset = www.texture;
		if (badgeTextureAsset != null)
		{
			levelBadge.enabled = true;
			levelBadge.texture = badgeTextureAsset;
			bool flag = MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.HasBenefit(SubscriptionBenefit.XPBoost);
			if (xpBar.gameObject.activeSelf == flag)
			{
				xpBar.gameObject.SetActive(!flag);
			}
			if (subscriberXPBar.gameObject.activeSelf != flag)
			{
				subscriberXPBar.gameObject.SetActive(flag);
			}
		}
	}
}
