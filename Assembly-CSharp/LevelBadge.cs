using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelBadge : MonoBehaviour
{
	[SerializeField]
	private RawImage levelBadge;

	[SerializeField]
	private ProgressBarAndroid xpBar;

	[SerializeField]
	private PlayerStatusPopup playerStatusPopup;

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
			LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, new UnityAction(OnLevelingInitialized));
		}
	}

	public void OnClick()
	{
		if (!MVGameControllerBase.IsTouristSession)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.Effect);
			});
			PlayerStatusPopup playerStatus = UnityEngine.Object.Instantiate(playerStatusPopup);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(playerStatus.gameObject, UIPushOption.None, null, UIGroupFlags.Effect);
			});
			playerStatus.Initialize();
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
