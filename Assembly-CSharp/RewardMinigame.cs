using System;
using System.Collections.Generic;
using MV.Common;
using RewardGeneration;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class RewardMinigame : MonoBehaviour
{
	[SerializeField]
	private GridLayoutGroup rewardContentGroup;

	[SerializeField]
	private RewardObject rewardObjectPrefab;

	[SerializeField]
	private RectMask2D mask;

	[SerializeField]
	private RewardAllPrizes winnableRewards;

	[SerializeField]
	private Button spinButton;

	[SerializeField]
	private RectTransform buyMoreSpins;

	[SerializeField]
	private Button continueButton;

	[SerializeField]
	private PurchaseSpins purchaseSpins;

	[SerializeField]
	private RewardSpinVisualization rewardSpinVisualization;

	[SerializeField]
	private RectTransform winningRewardObjectRoot;

	[SerializeField]
	private RectTransform targetPreviewTransform;

	[SerializeField]
	private CanvasGroup backgroundElementsCanvasGroup;

	[SerializeField]
	private float fadeSpeed;

	[SerializeField]
	private int baseWinningIndex;

	[SerializeField]
	private int amountOfSpinLoops;

	[SerializeField]
	private float purchaseButtonAnimationSpeed;

	[SerializeField]
	private List<RewardDef> rewardTypeList;

	[SerializeField]
	private List<RewardRarityGroupDef> rewardGroupList;

	private int winningIndex;

	private static readonly int extraItemsAfterWinningIndex = 15;

	private List<RewardObject> rewardObjects = new List<RewardObject>();

	private static bool mappedDictionaries = false;

	private static readonly Dictionary<RewardRarity, RewardRarityGroupDef> rarityGroupMap = new Dictionary<RewardRarity, RewardRarityGroupDef>();

	private static readonly Dictionary<RewardType, RewardDef> rarityDefMap = new Dictionary<RewardType, RewardDef>();

	private readonly List<RewardRarityGroupDef> sortedRarityGroupList = new List<RewardRarityGroupDef>();

	private bool spinReady;

	private bool waitingForSpinFromServer;

	private bool instantiatedContent;

	private readonly List<int> indicesToRemove = new List<int>();

	private List<IActorRewardClient> rewards;

	private float targetAlphaForCanvasGroup = 1f;

	private float maxPercent;

	public void Start()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ICanvasController x, BaseEventData y) =>
		{
			x.SetPixelPerfect(pixelPerfect: false);
		});
		RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Combine(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(UpdateRewardCount));
		PrepareForSpin();
	}

	private void UpdateRewardCount()
	{
		if (waitingForSpinFromServer)
		{
			buyMoreSpins.gameObject.SetActive(value: false);
			RewardManager.GetReward(WinningIndexReady);
			waitingForSpinFromServer = false;
		}
	}

	private void PrepareForSpin()
	{
		if (!spinReady)
		{
			winningIndex = baseWinningIndex;
			if (!RewardManager.IsInitialized)
			{
				RewardManager.RequestInitialization(OnRewardsReady);
			}
			else
			{
				OnRewardsReady();
			}
		}
	}

	private void OnRewardsReady()
	{
		rewards = RewardManager.GetPossibleRewards();
		if (!instantiatedContent)
		{
			MapSerializedLists();
			NormalizeProbability();
		}
		CreateLayoutGroupRewards();
		if (RewardManager.NumberOfPendingRewards == 0)
		{
			waitingForSpinFromServer = true;
		}
		else
		{
			RewardManager.GetReward(WinningIndexReady);
		}
	}

	private void WinningIndexReady()
	{
		SetWinningItem();
		if (instantiatedContent)
		{
			rewardSpinVisualization.AnimateShowing(OnSpinReady);
		}
		else
		{
			rewardSpinVisualization.Initialize(OnSpinReady);
		}
		mask.enabled = true;
		instantiatedContent = true;
	}

	private void OnSpinReady()
	{
		spinReady = true;
		spinButton.interactable = true;
		continueButton.interactable = true;
	}

	private void SetWinningItem()
	{
		IActorRewardClient currentReward = RewardManager.CurrentReward;
		winningIndex = baseWinningIndex + amountOfSpinLoops * (extraItemsAfterWinningIndex + baseWinningIndex);
		RewardObject rewardObject = UnityEngine.Object.Instantiate(rarityDefMap[currentReward.RewardType].rewardPrefabType);
		rewardObject.transform.SetParent(rewardContentGroup.transform, worldPositionStays: false);
		rewardObject.transform.SetSiblingIndex(winningIndex);
		RewardObject rewardObject2 = rewardObjects[winningIndex];
		rewardObjects[winningIndex] = rewardObject;
		UnityEngine.Object.Destroy(rewardObject2.gameObject);
		RewardRarityGroupDef rewardRarity = rarityGroupMap[currentReward.RewardRarity];
		rewardRarity.actorReward = currentReward;
		rewardRarity.rewardDef = rarityDefMap[currentReward.RewardType];
		rewardObject.Initialize(rewardRarity);
	}

	private void MapSerializedLists()
	{
		if (!mappedDictionaries)
		{
			mappedDictionaries = true;
			for (int i = 0; i < rewardGroupList.Count; i++)
			{
				RewardRarityGroupDef value = rewardGroupList[i];
				rarityGroupMap[value.rarity] = value;
			}
			for (int j = 0; j < rewardTypeList.Count; j++)
			{
				RewardDef value2 = rewardTypeList[j];
				rarityDefMap[value2.rewardType] = value2;
			}
		}
	}

	private void NormalizeProbability()
	{
		float num = 0f;
		for (int i = 0; i < rewards.Count; i++)
		{
			IActorRewardClient actorRewardClient = rewards[i];
			RewardRarityGroupDef item = rarityGroupMap[actorRewardClient.RewardRarity];
			item.actorReward = actorRewardClient;
			item.normalizedProbability = num + item.showProbability;
			item.rewardDef = rarityDefMap[actorRewardClient.RewardType];
			num = item.normalizedProbability;
			sortedRarityGroupList.Add(item);
		}
		maxPercent = num;
	}

	private void CreateLayoutGroupRewards()
	{
		int num = winningIndex + extraItemsAfterWinningIndex;
		for (int i = 0; i < num; i++)
		{
			int num2 = UnityEngine.Random.Range(0, 3);
			for (int j = 0; j < num2; j++)
			{
				UnityEngine.Random.Range(0f, maxPercent);
			}
			CreateRewardObjects(i);
		}
		for (int k = 0; k < amountOfSpinLoops; k++)
		{
			for (int l = 0; l < num; l++)
			{
				if (instantiatedContent)
				{
					rewardObjects[num + l].Initialize(rewardObjects[l].RewardRarity);
					continue;
				}
				RewardObject rewardObject = UnityEngine.Object.Instantiate(rewardObjects[l]);
				rewardObject.transform.SetParent(rewardContentGroup.transform, worldPositionStays: false);
				rewardObjects.Add(rewardObject);
			}
		}
		RemoveExtraIndices();
	}

	private void CreateRewardObjects(int index)
	{
		float num = UnityEngine.Random.Range(0f, maxPercent);
		for (int i = 0; i < sortedRarityGroupList.Count; i++)
		{
			RewardRarityGroupDef rewardRarity = sortedRarityGroupList[i];
			if (!(num <= rewardRarity.normalizedProbability))
			{
				continue;
			}
			if (instantiatedContent && rewardObjects[index].GetType() == rewardRarity.rewardDef.rewardPrefabType.GetType())
			{
				rewardObjects[index].Initialize(rewardRarity);
				break;
			}
			if (instantiatedContent)
			{
				UnityEngine.Object.Destroy(rewardObjects[index].gameObject);
				indicesToRemove.Add(index);
			}
			RewardObject rewardObject = UnityEngine.Object.Instantiate(rewardRarity.rewardDef.rewardPrefabType);
			rewardObject.transform.SetParent(rewardContentGroup.transform, worldPositionStays: false);
			rewardObject.transform.SetSiblingIndex(index);
			rewardObjects.Add(rewardObject);
			rewardObject.Initialize(rewardRarity);
			break;
		}
	}

	private void RemoveExtraIndices()
	{
		for (int i = 0; i < indicesToRemove.Count; i++)
		{
			rewardObjects.RemoveAt(i);
		}
		indicesToRemove.Clear();
	}

	public void StartContentRotation()
	{
		if (spinReady)
		{
			spinReady = false;
			buyMoreSpins.gameObject.SetActive(value: false);
			spinButton.interactable = false;
			continueButton.interactable = false;
			float num = rewardContentGroup.cellSize.x / 2f;
			float num2 = UnityEngine.Random.Range(0f - num + 8f, num - 8f);
			float targetPosition = 0f - rewardObjects[winningIndex].GetPosX() - ((RectTransform)rewardContentGroup.transform).anchoredPosition.x + num + num2;
			rewardSpinVisualization.StartSpin(targetPosition, SpinningFinished);
		}
	}

	private void Update()
	{
		if (buyMoreSpins.gameObject.activeInHierarchy)
		{
			buyMoreSpins.transform.localScale = Vector3.Lerp(buyMoreSpins.transform.localScale, Vector3.one, purchaseButtonAnimationSpeed * Time.deltaTime);
		}
		if (backgroundElementsCanvasGroup.alpha != targetAlphaForCanvasGroup)
		{
			backgroundElementsCanvasGroup.alpha = Mathf.Lerp(backgroundElementsCanvasGroup.alpha, targetAlphaForCanvasGroup, fadeSpeed * Time.deltaTime);
		}
	}

	public void OnViewPossibleRewardsClicked()
	{
		RewardAllPrizes allRewards = UnityEngine.Object.Instantiate(winnableRewards);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(allRewards.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
		allRewards.Initialize(sortedRarityGroupList);
	}

	private void SpinningFinished()
	{
		RewardManager.ClaimReward(PreviewReward);
	}

	private void PreviewReward()
	{
		targetAlphaForCanvasGroup = 0f;
		winningRewardObjectRoot.gameObject.SetActive(value: true);
		RewardObject rewardObject = UnityEngine.Object.Instantiate(rewardObjects[winningIndex]);
		rewardObject.CachedTransform.sizeDelta = rewardObjects[winningIndex].CachedTransform.sizeDelta;
		rewardObject.transform.SetParent(winningRewardObjectRoot, worldPositionStays: false);
		rewardObject.transform.position = rewardObjects[winningIndex].transform.position;
		rewardObject.SelectReward(targetPreviewTransform, OnFinishedPreviewingReward, Reset);
		if ((int)RewardManager.CurrentReward.RewardRarity >= 2)
		{
			PublishRareRewardNotification();
		}
	}

	private void PublishRareRewardNotification()
	{
		MVGameControllerBase.OperationRequests.WonRareReward(RewardManager.CurrentReward, GetRewardAmount(RewardManager.CurrentReward));
	}

	private int GetRewardAmount(IActorRewardClient reward)
	{
		return reward.RewardType switch
		{
			RewardType.GoldReward => ((ActorGoldRewardClient)reward).goldAmount, 
			RewardType.XPReward => ((ActorXPRewardClient)reward).xpAmount, 
			_ => 0, 
		};
	}

	private void OnFinishedPreviewingReward()
	{
		continueButton.interactable = true;
		winningRewardObjectRoot.gameObject.SetActive(value: false);
		targetAlphaForCanvasGroup = 1f;
		UnityEngine.Object.Destroy(winningRewardObjectRoot.GetChild(0).gameObject);
		rewardSpinVisualization.AnimateHiding(PrepareForSpin);
		if (RewardManager.NumberOfPendingRewards == 0)
		{
			spinReady = false;
			waitingForSpinFromServer = true;
			buyMoreSpins.gameObject.SetActive(value: true);
			buyMoreSpins.transform.localScale = Vector3.zero;
		}
	}

	public void BuyMoreSpins()
	{
		PurchaseSpins purchaseSpinsPopup = UnityEngine.Object.Instantiate(purchaseSpins);
		purchaseSpinsPopup.Initialize(OnPurchaseSpinsPop);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(purchaseSpinsPopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
	}

	private void OnPurchaseSpinsPop()
	{
		buyMoreSpins.gameObject.SetActive(value: false);
	}

	private void Reset()
	{
		spinReady = false;
	}

	private void ClearObjects()
	{
		rewardObjects.Clear();
		sortedRarityGroupList.Clear();
		rewardSpinVisualization.Clear();
	}

	private void OnDestroy()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ICanvasController x, BaseEventData y) =>
		{
			x.SetPixelPerfect(pixelPerfect: true);
		});
		RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Remove(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(UpdateRewardCount));
		RewardManager.UnsubscribeFromGetReward();
		ClearObjects();
		rewardObjects.Clear();
		rewardSpinVisualization.Clear();
	}
}
