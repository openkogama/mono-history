using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelProgressAccessoryShop : MonoBehaviour
{
	[SerializeField]
	private Text progressText;

	[SerializeField]
	private ProgressBar progressBar;

	[SerializeField]
	private RawImage badgeTexture;

	private Texture2D badgeTextureAsset;

	private int badgeLevel = 1;

	public void Start()
	{
		if (!LevelingManager.IsInitialized)
		{
			LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, new UnityAction(Initialize));
		}
		else
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Remove(LevelingManager.OnLevelingInitialized, new UnityAction(Initialize));
		badgeLevel = MVGameControllerBase.Game.LocalPlayer.Level;
		OnXPUpdate(MVGameControllerBase.Game.LocalPlayer.XPProgressData);
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		localPlayer.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnXPUpdate));
		BadgeManager.GetBadgeTexture(MVGameControllerBase.Game.LocalPlayer.Level, OnLevelingBadgeLoaded);
	}

	private void OnXPUpdate(XPProgressData xpProgressData)
	{
		int nextXP = xpProgressData.NextXP;
		int xP = xpProgressData.XP;
		int prevXP = xpProgressData.PrevXP;
		progressText.text = $"XP: {xP - prevXP} / {nextXP - prevXP}";
		progressBar.Progress = (float)(xP - prevXP) / (float)(nextXP - prevXP);
		if (MVGameControllerBase.Game.LocalPlayer.Level != badgeLevel)
		{
			badgeLevel = MVGameControllerBase.Game.LocalPlayer.Level;
			BadgeManager.GetBadgeTexture(MVGameControllerBase.Game.LocalPlayer.Level, OnLevelingBadgeLoaded);
		}
	}

	private void OnDestroy()
	{
		LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Remove(LevelingManager.OnLevelingInitialized, new UnityAction(Initialize));
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		localPlayer.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Remove(localPlayer.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnXPUpdate));
		BadgeManager.UnsubscribeGetBadgeRequest(OnLevelingBadgeLoaded);
		UnityEngine.Object.Destroy(badgeTextureAsset);
	}

	private void OnLevelingBadgeLoaded(WWW www)
	{
		if (www != null)
		{
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogWarning("Texture not loaded: " + www.error);
				return;
			}
			badgeTextureAsset = www.texture;
			badgeTexture.texture = badgeTextureAsset;
		}
	}
}
