using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
			LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, new UnityAction(OnLevelingInitialized));
		}
	}

	private void OnLevelingInitialized()
	{
		UpdateBadge(MVGameControllerBase.Game.Players[ownerActorId].Level);
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[ownerActorId];
		mVPlayer.OnLevelChanged = (UnityAction<int>)Delegate.Combine(mVPlayer.OnLevelChanged, new UnityAction<int>(UpdateBadge));
	}

	private void UpdateBadge(int level)
	{
		BadgeManager.GetBadgeTexture(level, StreamingAssetCallback);
		levelText.text = level.ToString();
	}

	private void OnDestroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(StreamingAssetCallback);
		if (!LevelingManager.IsInitialized)
		{
			LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Remove(LevelingManager.OnLevelingInitialized, new UnityAction(OnLevelingInitialized));
		}
		if (MVGameControllerBase.Game != null)
		{
			if (MVGameControllerBase.Game.Players.ContainsKey(ownerActorId))
			{
				MVPlayer mVPlayer = MVGameControllerBase.Game.Players[ownerActorId];
				mVPlayer.OnLevelChanged = (UnityAction<int>)Delegate.Remove(mVPlayer.OnLevelChanged, new UnityAction<int>(UpdateBadge));
			}
			if (scaleAnimation != null)
			{
				ScaleAnimations scaleAnimations = scaleAnimation;
				scaleAnimations.OnIntermediateScaleAnimationStopped = (ScaleAnimationBase.OnScaleAnimationStoppedDelegate)Delegate.Remove(scaleAnimations.OnIntermediateScaleAnimationStopped, new ScaleAnimationBase.OnScaleAnimationStoppedDelegate(ScaleAnimationIntermediateCallback));
			}
			UnityEngine.Object.Destroy(badgeRenderer.material);
		}
	}

	private void StreamingAssetCallback(WWW www)
	{
		if (www != null && www.texture != null)
		{
			badgeRenderer.gameObject.layer = LayerMask.NameToLayer("UIItems");
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
		if (SceneManager.GetActiveScene().name != "GUIDevScene")
		{
			badgeRenderer.material.mainTexture = texture;
			texture = null;
		}
	}
}
