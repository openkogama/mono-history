using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpNotification : Notification
{
	[SerializeField]
	private Text label;

	[SerializeField]
	private Image Icon;

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
	}

	private void BadgeCallback(WWW www)
	{
		if (www != null && www.texture != null)
		{
			Icon.sprite = GetBadgeSprite(www.texture);
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
