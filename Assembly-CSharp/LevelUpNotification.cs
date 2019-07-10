using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpNotification : Notification
{
	[SerializeField]
	private Text label;

	[SerializeField]
	private Image Icon;

	private Texture2D textureAsset;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		if (LevelingManager.IsInitialized)
		{
			int level = (int)data[(byte)4];
			BadgeManager.GetBadgeTexture(level, BadgeCallback);
		}
		label.text = TM._("Level Up!");
	}

	private void OnDestroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(BadgeCallback);
		Object.Destroy(textureAsset);
	}

	private void BadgeCallback(WWW www)
	{
		textureAsset = www.texture;
		if (textureAsset != null)
		{
			Icon.sprite = Sprite.Create(textureAsset, new Rect(Vector2.zero, new Vector2(textureAsset.width, textureAsset.height)), Vector2.one / 2f);
		}
		else
		{
			Debug.Log("Failed to get: " + www.url);
		}
	}
}
