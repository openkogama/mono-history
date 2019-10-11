using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesXpRewardInfo : MonoBehaviour, IGamePassShopContent
{
	[SerializeField]
	private Image TeamRequirementImage;

	[SerializeField]
	private GameObject optionsButton;

	[SerializeField]
	private GameObject backgroundTier1;

	[SerializeField]
	private GameObject backgroundTier2;

	[SerializeField]
	private GameObject backgroundTier3;

	[SerializeField]
	private GamePassesXPRewardOption xPRewardOption;

	[SerializeField]
	private Text xPAmountText;

	[SerializeField]
	private Text bonusXPAmountText;

	[SerializeField]
	private GameObject subscriberUI;

	[SerializeField]
	private GameObject claimedText;

	private GamePassTier tier;

	public void Initialize(GamePassTier tier)
	{
		this.tier = tier;
		ChangeBackground(tier);
		TeamRequirementImage.color = Styles.GetTeamColor(MVTeam.None, darkTeam: true);
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit || MVGameControllerBase.EditModeUI.IsInPlayInEditMode)
		{
			optionsButton.SetActive(value: false);
		}
		UpdateXPText();
		UpdateXPTextVisibility();
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Combine(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(OnGameProgressionDataUpdate));
	}

	public void OnOptionsButtonPress()
	{
		GamePassesXPRewardOption popUp = UnityEngine.Object.Instantiate(xPRewardOption);
		popUp.Initialize(this, tier);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popUp.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
	}

	public void OnGameProgressionDataUpdate()
	{
		UpdateXPText();
		UpdateXPTextVisibility();
	}

	public void Activate()
	{
	}

	public void Deactivate()
	{
	}

	private void ChangeBackground(GamePassTier tier)
	{
		bool flag = tier == GamePassTier.Tier1;
		bool flag2 = tier == GamePassTier.Tier2;
		bool flag3 = tier == GamePassTier.Tier3;
		if (backgroundTier1.activeSelf != flag)
		{
			backgroundTier1.SetActive(flag);
		}
		if (backgroundTier2.activeSelf != flag2)
		{
			backgroundTier2.SetActive(flag2);
		}
		if (backgroundTier3.activeSelf != flag3)
		{
			backgroundTier3.SetActive(flag3);
		}
	}

	private void UpdateXPText()
	{
		int xPReward = GamePassProgressionController.GetXPReward(tier);
		xPAmountText.text = xPReward + " XP";
		float num = SubscriberRewardDataManager.GetBaseXpAmount();
		bonusXPAmountText.text = Mathf.FloorToInt((float)xPReward * (1f + num / 100f)) + " XP for Elites";
	}

	private void UpdateXPTextVisibility()
	{
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit)
		{
			bool flag = (int)tier <= (int)GamePassesManager.PlayerPlanetData.gamePassTier;
			if (xPAmountText.gameObject.activeSelf == flag)
			{
				xPAmountText.gameObject.SetActive(!flag);
			}
			if (subscriberUI.activeSelf == flag)
			{
				subscriberUI.SetActive(!flag);
			}
			if (claimedText.activeSelf != flag)
			{
				claimedText.SetActive(flag);
			}
		}
	}

	private void OnDestroy()
	{
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Remove(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(OnGameProgressionDataUpdate));
	}
}
