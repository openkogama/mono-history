using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Networking;
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

	private void OnLevelRequirementLoaded(UnityWebRequest www)
	{
		requiredLevelTextureAsset = DownloadHandlerTexture.GetContent(www);
		if (requiredLevelTextureAsset == null)
		{
			Debug.LogWarning("Badge not loaded for accessory level requirement");
		}
		else
		{
			requiredLevelImage.texture = requiredLevelTextureAsset;
		}
	}

	private void OnPlayerLevelLoaded(UnityWebRequest www)
	{
		playerLevelTextureAsset = DownloadHandlerTexture.GetContent(www);
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
		requiredLevelTextureAsset = null;
		playerLevelTextureAsset = null;
		if (resultCallback != null)
		{
			resultCallback();
		}
	}

	private void OnDestroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(OnLevelRequirementLoaded);
		BadgeManager.UnsubscribeGetBadgeRequest(OnPlayerLevelLoaded);
	}
}
