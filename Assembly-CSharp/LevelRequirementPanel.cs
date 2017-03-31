using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelRequirementPanel : MonoBehaviour
{
	[SerializeField]
	private RawImage levelRequirementImage;

	private int prevLevel = -1;

	private int desiredLevel;

	public void SetLevelSpriteFromCallback(int level)
	{
		desiredLevel = level;
		if (!LevelingManager.IsInitialized)
		{
			LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, new UnityAction(OnLevelingIsReady));
		}
		else
		{
			SetLevelBadge();
		}
	}

	private void OnLevelingIsReady()
	{
		LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Remove(LevelingManager.OnLevelingInitialized, new UnityAction(OnLevelingIsReady));
		SetLevelBadge();
	}

	private void SetLevelBadge()
	{
		if (desiredLevel != prevLevel)
		{
			prevLevel = desiredLevel;
			BadgeManager.GetBadgeTexture(desiredLevel, StreamingAssetCallback);
		}
	}

	private void OnDestroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(StreamingAssetCallback);
	}

	private void StreamingAssetCallback(WWW www)
	{
		if (www != null && www.texture != null)
		{
			Debug.Log("www.texture.width " + www.texture.width);
			levelRequirementImage.enabled = true;
			Debug.Log(desiredLevel);
			levelRequirementImage.texture = www.texture;
		}
	}
}
