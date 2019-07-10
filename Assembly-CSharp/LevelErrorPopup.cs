using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelErrorPopup : MonoBehaviour
{
	[SerializeField]
	private RawImage requiredLevelImage;

	[SerializeField]
	private RawImage playerLevelImage;

	private UnityAction resultCallback;

	private Texture2D requiredLevelTextureAsset;

	private Texture2D playerLevelTextureAsset;

	public void Initialize(UnityAction resultCallback, int requiredLevel)
	{
		BadgeManager.GetBadgeTexture(requiredLevel, OnLevelRequirementLoaded);
		BadgeManager.GetBadgeTexture(MVGameControllerBase.Game.LocalPlayer.Level, OnPlayerLevelLoaded);
		this.resultCallback = resultCallback;
	}

	private void OnLevelRequirementLoaded(WWW www)
	{
		requiredLevelTextureAsset = www.texture;
		if (requiredLevelTextureAsset == null)
		{
			Debug.LogWarning("Badge not loaded for accessory level requirement");
		}
		else
		{
			requiredLevelImage.texture = requiredLevelTextureAsset;
		}
	}

	private void OnPlayerLevelLoaded(WWW www)
	{
		playerLevelTextureAsset = www.texture;
		if (playerLevelTextureAsset == null)
		{
			Debug.LogWarning("Badge not loaded for accessory level requirement");
		}
		else
		{
			playerLevelImage.texture = playerLevelTextureAsset;
		}
	}

	public void OnButtonPressed()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		Object.Destroy(requiredLevelTextureAsset);
		Object.Destroy(playerLevelTextureAsset);
		if (resultCallback != null)
		{
			resultCallback();
		}
	}
}
