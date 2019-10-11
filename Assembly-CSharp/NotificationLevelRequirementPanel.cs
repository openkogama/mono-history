using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NotificationLevelRequirementPanel : NotificationRequirementPanel
{
	[SerializeField]
	private Image LevelImage;

	private Texture2D badgeTextureAsset;

	public override void OnToggleEnabled(object text, Sprite checkmarkSprite, bool enabled)
	{
		base.OnToggleEnabled(text, checkmarkSprite, enabled);
		if (LevelingManager.IsInitialized)
		{
			BadgeManager.GetBadgeTexture((int)text, BadgeCallback);
		}
	}

	private void OnDestroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(BadgeCallback);
		Object.Destroy(badgeTextureAsset);
	}

	private void BadgeCallback(UnityWebRequest www)
	{
		badgeTextureAsset = DownloadHandlerTexture.GetContent(www);
		if (badgeTextureAsset != null)
		{
			LevelImage.sprite = GetBadgeSprite(badgeTextureAsset);
		}
		else
		{
			Debug.Log("Failed to get: " + www.url);
		}
	}

	private Sprite GetBadgeSprite(Texture2D source)
	{
		return Sprite.Create(source, new Rect(Vector2.zero, new Vector2(source.width, source.height)), Vector2.one / 2f);
	}
}
