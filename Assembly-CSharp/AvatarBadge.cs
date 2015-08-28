using System;
using UnityEngine;

public class AvatarBadge : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer badgeRenderer;

	[SerializeField]
	private TextMesh levelText;

	[SerializeField]
	private ScaleAnimations scaleAnimation;

	private Texture texture;

	private int ownerActorId;

	public void Initialize(int ownerActorId)
	{
		this.ownerActorId = ownerActorId;
		ScaleAnimations scaleAnimations = scaleAnimation;
		scaleAnimations.OnIntermediateScaleAnimationStopped = (ScaleAnimationBase.OnScaleAnimationStoppedDelegate)Delegate.Combine(scaleAnimations.OnIntermediateScaleAnimationStopped, new ScaleAnimationBase.OnScaleAnimationStoppedDelegate(ScaleAnimationIntermediateCallback));
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
		UpdateBadge(MVGameController.Game.Players[ownerActorId].Level);
		MVPlayer mVPlayer = MVGameController.Game.Players[ownerActorId];
		mVPlayer.OnLevelChanged = (MVPlayer.OnLevelChangedDelegate)Delegate.Combine(mVPlayer.OnLevelChanged, new MVPlayer.OnLevelChangedDelegate(UpdateBadge));
	}

	private void UpdateBadge(int level)
	{
		BadgeManager.GetBadgeTexture(level, StreamingAssetCallback);
		levelText.text = level.ToString();
	}

	private void OnDestroy()
	{
		if (MVGameController.Game.Players.ContainsKey(ownerActorId))
		{
			MVPlayer mVPlayer = MVGameController.Game.Players[ownerActorId];
			mVPlayer.OnLevelChanged = (MVPlayer.OnLevelChangedDelegate)Delegate.Remove(mVPlayer.OnLevelChanged, new MVPlayer.OnLevelChangedDelegate(UpdateBadge));
		}
		if (scaleAnimation != null)
		{
			ScaleAnimations scaleAnimations = scaleAnimation;
			scaleAnimations.OnIntermediateScaleAnimationStopped = (ScaleAnimationBase.OnScaleAnimationStoppedDelegate)Delegate.Remove(scaleAnimations.OnIntermediateScaleAnimationStopped, new ScaleAnimationBase.OnScaleAnimationStoppedDelegate(ScaleAnimationIntermediateCallback));
		}
	}

	private void StreamingAssetCallback(WWW www)
	{
		if (www != null && www.texture != null)
		{
			badgeRenderer.gameObject.layer = LayerMask.NameToLayer("Default");
			texture = www.texture;
			scaleAnimation.Play();
		}
		else
		{
			Debug.Log("Failed to get: " + www.url);
		}
	}

	private void ScaleAnimationIntermediateCallback(float extraTime)
	{
		if (Application.loadedLevelName != "GUIDevScene")
		{
			badgeRenderer.material.mainTexture = texture;
			texture = null;
		}
	}
}
