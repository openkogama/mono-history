using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePassesShopDetails : MonoBehaviour
{
	[SerializeField]
	private GamePassesShop gamePassesShopPrefab;

	[SerializeField]
	private GamePassesHighScoreList highScoreListPrefab;

	[SerializeField]
	private List<CanvasGroup> tierOutlineList;

	private float interpolationStartTime;

	private GamePassTier currentFocusedTier;

	private const float outlineInterpolationSpeed = 2f;

	private void OnDisable()
	{
		ResetHighlightEffects();
	}

	private void Update()
	{
		float num = Time.time - interpolationStartTime;
		if (num > 1f + Time.deltaTime)
		{
			return;
		}
		for (int i = 0; i < tierOutlineList.Count; i++)
		{
			if (i == (int)(currentFocusedTier - 1))
			{
				tierOutlineList[(int)(currentFocusedTier - 1)].alpha = Mathf.Lerp(0f, 1f, num * 2f);
				continue;
			}
			float num2 = Mathf.Lerp(tierOutlineList[i].alpha, 0f, num * 2f);
			if (num2 < tierOutlineList[i].alpha)
			{
				tierOutlineList[i].alpha = num2;
			}
		}
	}

	private void ResetHighlightEffects()
	{
		currentFocusedTier = GamePassTier.Tier0;
		interpolationStartTime = 0f;
		for (int i = 0; i < tierOutlineList.Count; i++)
		{
			tierOutlineList[i].alpha = 0f;
		}
	}

	private void InstantiateGamePassesShop(GamePassTier tierToShow)
	{
		GamePassesShop gamePassesShop = Object.Instantiate(gamePassesShopPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gamePassesShop.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		gamePassesShop.Initialize(tierToShow);
	}

	public void Exit()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	public void OnTierDetailEnter(GamePassTier tierEntered)
	{
		currentFocusedTier = tierEntered;
		interpolationStartTime = Time.time;
	}

	public void OnTierDetailExit(GamePassTier tierExited)
	{
		currentFocusedTier = GamePassTier.Tier0;
		interpolationStartTime = Time.time;
	}

	public void OnTier1ShopPressed()
	{
		InstantiateGamePassesShop(GamePassTier.Tier1);
	}

	public void OnTier2ShopPressed()
	{
		InstantiateGamePassesShop(GamePassTier.Tier2);
	}

	public void OnTier3ShopPressed()
	{
		InstantiateGamePassesShop(GamePassTier.Tier3);
	}

	public void ShowHighScore()
	{
		GamePassesHighScoreList highScoreList = Object.Instantiate(highScoreListPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(highScoreList.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}
}
