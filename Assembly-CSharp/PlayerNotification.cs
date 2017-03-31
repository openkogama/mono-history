using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNotification : Notification
{
	private const int PrestigiousLevelRequirement = 25;

	[SerializeField]
	protected Text NameLabel;

	[SerializeField]
	private Image BadgeImage;

	[SerializeField]
	private RectTransform PrestigiousPlayerFrame;

	[SerializeField]
	private RectTransform FriendPlayerFrame;

	private MVPlayer player;

	protected NotificationLifetime lifeTime = NotificationLifetime.Low;

	protected override NotificationLifetime Lifetime => lifeTime;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		player = MVGameControllerBase.Game.Players[(int)data[(byte)9]];
		if (player.Level >= 25)
		{
			PrestigiousPlayerFrame.gameObject.SetActive(value: true);
			lifeTime = NotificationLifetime.High;
		}
		else
		{
			PrestigiousPlayerFrame.gameObject.SetActive(value: false);
			lifeTime = NotificationLifetime.Low;
		}
		Friend friendByProfileID = MVGameControllerBase.Game.Friends.GetFriendByProfileID(player.ProfileID);
		if (friendByProfileID != null && friendByProfileID.status == FriendStatus.Accepted)
		{
			PrestigiousPlayerFrame.gameObject.SetActive(value: false);
			FriendPlayerFrame.gameObject.SetActive(value: true);
			lifeTime = NotificationLifetime.High;
		}
		if (LevelingManager.IsInitialized)
		{
			BadgeManager.GetBadgeTexture(player.Level, BadgeCallback);
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
			BadgeImage.sprite = GetBadgeSprite(www.texture);
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
