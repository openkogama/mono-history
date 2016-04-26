using System;
using UnityEngine;
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
			LevelingManager.OnLevelingInitialized = (LevelingManager.OnlevelingInitializedDelegate)Delegate.Combine(LevelingManager.OnLevelingInitialized, new LevelingManager.OnlevelingInitializedDelegate(OnLevelingIsReady));
		}
		else
		{
			SetLevelBadge();
		}
	}

	private void OnLevelingIsReady()
	{
		LevelingManager.OnLevelingInitialized = (LevelingManager.OnlevelingInitializedDelegate)Delegate.Remove(LevelingManager.OnLevelingInitialized, new LevelingManager.OnlevelingInitializedDelegate(OnLevelingIsReady));
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
