using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RequirementNotification : Notification
{
	[SerializeField]
	private Sprite OnSprite;

	[SerializeField]
	private Sprite OffSprite;

	[SerializeField]
	private RectTransform Layout;

	[SerializeField]
	private NotificationRequirementPanel LevelPanel;

	[SerializeField]
	private NotificationRequirementPanel GamecoinPanel;

	[SerializeField]
	private NotificationRequirementPanel StarPanel;

	private List<GameObject> PanelsToDestroy = new List<GameObject>();

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		PanelsToDestroy.ForEach((GameObject x) =>
		{
			Object.Destroy(x);
		});
		PanelsToDestroy.Clear();
		int id = (int)data[(byte)10];
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(id);
		if (worldObjectClient.Data.ContainsKey("levelAmount"))
		{
			int level = (int)worldObjectClient.Data["levelAmount"];
			ShowLevelRequirement(level);
		}
		if (worldObjectClient.Data.ContainsKey("gameCoinAmount"))
		{
			int gameCoins = (int)worldObjectClient.Data["gameCoinAmount"];
			ShowGameCoinRequirement(gameCoins);
		}
		if (worldObjectClient.Data.ContainsKey("starAmount"))
		{
			int stars = (int)worldObjectClient.Data["starAmount"];
			ShowStarRequirement(stars);
		}
	}

	private void ShowGameCoinRequirement(int gameCoins)
	{
		GameObject target = InstantiatePanel(GamecoinPanel);
		bool enabled = gameCoins <= MVGameControllerBase.Game.GameCoinManager.GameCoinAmount;
		Sprite checkMark = ((!enabled) ? OffSprite : OnSprite);
		ExecuteEvents.Execute(target, null, (INotificationRequirementPanel x, BaseEventData y) =>
		{
			x.OnToggleEnabled(gameCoins, checkMark, enabled);
		});
	}

	private void ShowStarRequirement(int stars)
	{
		GameObject target = InstantiatePanel(StarPanel);
		bool enabled = stars <= MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Collectible);
		Sprite checkMark = ((!enabled) ? OffSprite : OnSprite);
		ExecuteEvents.Execute(target, null, (INotificationRequirementPanel x, BaseEventData y) =>
		{
			x.OnToggleEnabled(stars, checkMark, enabled);
		});
	}

	private void ShowLevelRequirement(int level)
	{
		GameObject target = InstantiatePanel(LevelPanel);
		bool enabled = level <= MVGameControllerBase.Game.LocalPlayer.Level;
		Sprite checkMark = ((!enabled) ? OffSprite : OnSprite);
		ExecuteEvents.Execute(target, null, (INotificationRequirementPanel x, BaseEventData y) =>
		{
			x.OnToggleEnabled(level, checkMark, enabled);
		});
	}

	private GameObject InstantiatePanel(NotificationRequirementPanel panel)
	{
		GameObject gameObject = Object.Instantiate(panel.gameObject);
		gameObject.transform.SetParent(Layout.transform, worldPositionStays: false);
		PanelsToDestroy.Add(gameObject);
		return gameObject;
	}
}
