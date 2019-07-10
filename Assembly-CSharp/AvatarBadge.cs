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

	private Texture badgeTextureAsset;

	private int ownerActorId = -1;

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
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(ownerActorId);
		UpdateBadge(playerUnsafe.Level);
		playerUnsafe.OnLevelChanged = (UnityAction<int>)Delegate.Combine(playerUnsafe.OnLevelChanged, new UnityAction<int>(UpdateBadge));
	}

	private void UpdateBadge(int level)
	{
		BadgeManager.GetBadgeTexture(level, OnBadgeTextureReceived);
		levelText.text = level.ToString();
	}

	private void OnDestroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(OnBadgeTextureReceived);
		LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Remove(LevelingManager.OnLevelingInitialized, new UnityAction(OnLevelingInitialized));
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
		{
			if (ownerActorId != -1 && MVGameControllerBase.Game.MVPlayerContainer.ContainsKey(ownerActorId))
			{
				MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(ownerActorId);
				playerUnsafe.OnLevelChanged = (UnityAction<int>)Delegate.Remove(playerUnsafe.OnLevelChanged, new UnityAction<int>(UpdateBadge));
			}
			if (scaleAnimation != null)
			{
				ScaleAnimations scaleAnimations = scaleAnimation;
				scaleAnimations.OnIntermediateScaleAnimationStopped = (ScaleAnimationBase.OnScaleAnimationStoppedDelegate)Delegate.Remove(scaleAnimations.OnIntermediateScaleAnimationStopped, new ScaleAnimationBase.OnScaleAnimationStoppedDelegate(ScaleAnimationIntermediateCallback));
			}
			UnityEngine.Object.Destroy(badgeRenderer.material);
		}
	}

	private void OnBadgeTextureReceived(WWW www)
	{
		Texture2D texture = www.texture;
		if (texture != null)
		{
			badgeRenderer.gameObject.layer = LayerMask.NameToLayer("UIItems");
			badgeTextureAsset = texture;
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
			badgeRenderer.material.mainTexture = badgeTextureAsset;
			badgeTextureAsset = null;
		}
	}
}
