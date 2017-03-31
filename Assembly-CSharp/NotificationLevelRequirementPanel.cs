using UnityEngine;
using UnityEngine.UI;

public class NotificationLevelRequirementPanel : NotificationRequirementPanel
{
	[SerializeField]
	private Image LevelImage;

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
	}

	private void BadgeCallback(WWW www)
	{
		if (www != null && www.texture != null)
		{
			LevelImage.sprite = GetBadgeSprite(www.texture);
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
