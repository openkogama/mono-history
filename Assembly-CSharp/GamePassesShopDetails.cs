using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.GamePassSystem.GamePassEarnings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesShopDetails : MonoBehaviour
{
	[SerializeField]
	private Text totalEarningsAmount;

	[SerializeField]
	private Text tier1EarningPercentage;

	[SerializeField]
	private Text tier2EarningPercentage;

	[SerializeField]
	private Text tier3EarningPercentage;

	[SerializeField]
	private Image tier1CircularImage;

	[SerializeField]
	private Image tier2CircularImage;

	[SerializeField]
	private Image tier3CircularImage;

	[SerializeField]
	private Text displayedEarningsDescriptionText;

	[SerializeField]
	private Text displayedEarningsAmountText;

	[SerializeField]
	private GameObject displayedSmallGoldIcon;

	[SerializeField]
	private GameObject displayedLargeGoldIcon;

	[SerializeField]
	private GamePassesShop gamePassesShopPrefab;

	[SerializeField]
	private GamePassesHighScoreList highScoreListPrefab;

	[SerializeField]
	private List<RectTransform> transformsToExpand;

	[SerializeField]
	private List<CanvasGroup> tierOutlineList;

	[SerializeField]
	private List<RectTransform> pieChartsToEnlargen;

	[SerializeField]
	private float widthExpandAmount;

	[SerializeField]
	private float pieChartSizeIncreasePercentage;

	private List<float> originalWidthPositionsList;

	private List<float> originalSizeList;

	private float interpolationStartTime;

	private GamePassTier currentFocusedTier;

	private const float outlineInterpolationSpeed = 2f;

	private List<int> tierEarnings;

	private void Start()
	{
		originalWidthPositionsList = new List<float>();
		for (int i = 0; i < transformsToExpand.Count; i++)
		{
			originalWidthPositionsList.Add(transformsToExpand[i].rect.width);
		}
		originalSizeList = new List<float>();
		for (int j = 0; j < pieChartsToEnlargen.Count; j++)
		{
			originalSizeList.Add(pieChartsToEnlargen[j].rect.width);
		}
		ProjectEarningsReport projectEarningReport = GamePassesProjectEarningsManager.ProjectEarningReport;
		if (projectEarningReport != null)
		{
			UpdateEarningsData(projectEarningReport);
		}
		else
		{
			SetUpWaitingForProjectEarningsReportUI();
		}
	}

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
		for (int i = 0; i < transformsToExpand.Count; i++)
		{
			float num2 = originalWidthPositionsList[i];
			if (i == (int)(currentFocusedTier - 1))
			{
				num2 += widthExpandAmount;
			}
			float x = Mathf.Lerp(transformsToExpand[i].rect.width, num2, num);
			transformsToExpand[i].sizeDelta = new Vector2(x, transformsToExpand[i].sizeDelta.y);
		}
		for (int j = 0; j < tierOutlineList.Count; j++)
		{
			if (j == (int)(currentFocusedTier - 1))
			{
				tierOutlineList[(int)(currentFocusedTier - 1)].alpha = Mathf.Lerp(0f, 1f, num * 2f);
				continue;
			}
			float num3 = Mathf.Lerp(tierOutlineList[j].alpha, 0f, num * 2f);
			if (num3 < tierOutlineList[j].alpha)
			{
				tierOutlineList[j].alpha = num3;
			}
		}
		for (int k = 0; k < pieChartsToEnlargen.Count; k++)
		{
			float num4 = originalSizeList[k];
			if (k == (int)(currentFocusedTier - 1))
			{
				num4 *= pieChartSizeIncreasePercentage;
			}
			float num5 = Mathf.Lerp(pieChartsToEnlargen[k].rect.width, num4, num);
			pieChartsToEnlargen[k].sizeDelta = new Vector2(num5, num5);
		}
	}

	private void SetUpWaitingForProjectEarningsReportUI()
	{
		GamePassesProjectEarningsManager.OnEarningsDataUpdated = (Action<ProjectEarningsReport>)Delegate.Combine(GamePassesProjectEarningsManager.OnEarningsDataUpdated, new Action<ProjectEarningsReport>(OnProjectEarningsUpdatedCallback));
		totalEarningsAmount.text = "??";
		tier1EarningPercentage.text = "Game Tier 1 (??%)";
		tier2EarningPercentage.text = "Game Tier 2 (??%)";
		tier3EarningPercentage.text = "Game Tier 3 (??%)";
		displayedEarningsDescriptionText.text = string.Empty;
		displayedEarningsAmountText.text = string.Empty;
		float num = 0.33f;
		tier1CircularImage.fillAmount = num;
		float num2 = 0.33f;
		tier2CircularImage.fillAmount = num2;
		tier3CircularImage.fillAmount = 0.34f;
		float angle = -360f * num;
		tier2CircularImage.transform.Rotate(Vector3.forward, angle);
		float angle2 = -360f * (num + num2);
		tier3CircularImage.transform.Rotate(Vector3.forward, angle2);
	}

	private void OnProjectEarningsUpdatedCallback(ProjectEarningsReport projectEarningsReport)
	{
		GamePassesProjectEarningsManager.OnEarningsDataUpdated = (Action<ProjectEarningsReport>)Delegate.Remove(GamePassesProjectEarningsManager.OnEarningsDataUpdated, new Action<ProjectEarningsReport>(OnProjectEarningsUpdatedCallback));
		UpdateEarningsData(projectEarningsReport);
	}

	private void UpdateEarningsData(ProjectEarningsReport projectEarningsReport)
	{
		tierEarnings = new List<int>();
		tierEarnings.Add(GetTotalEarnings(projectEarningsReport));
		tierEarnings.Add(GetTierEarnings(projectEarningsReport, GamePassTier.Tier1));
		tierEarnings.Add(GetTierEarnings(projectEarningsReport, GamePassTier.Tier2));
		tierEarnings.Add(GetTierEarnings(projectEarningsReport, GamePassTier.Tier3));
		int tier1Earnings = tierEarnings[1];
		int tier2Earnings = tierEarnings[2];
		int tier3Earnings = tierEarnings[3];
		int totalEarnings = tierEarnings[0];
		UpdateEarningsText(tier1Earnings, tier2Earnings, tier3Earnings, totalEarnings);
		UpdateEarningPieChart(tier1Earnings, tier2Earnings, tier3Earnings, totalEarnings);
		UpdateDisplayedText();
	}

	private int GetTotalEarnings(ProjectEarningsReport projectEarningsReport)
	{
		int profileID = MVGameControllerBase.Game.LocalPlayer.ProfileID;
		if (!projectEarningsReport.projectMemberEarningsReports.ContainsKey(profileID))
		{
			return 0;
		}
		return projectEarningsReport.projectMemberEarningsReports[profileID].earningsReport.TotalEarningsGold;
	}

	private int GetTierEarnings(ProjectEarningsReport projectEarningsReport, GamePassTier tier)
	{
		int profileID = MVGameControllerBase.Game.LocalPlayer.ProfileID;
		if (!projectEarningsReport.projectMemberEarningsReports.ContainsKey(profileID))
		{
			return 0;
		}
		if (!projectEarningsReport.projectMemberEarningsReports[profileID].earningsReport.gamePassTierEarningsGold.ContainsKey(tier))
		{
			return 0;
		}
		return projectEarningsReport.projectMemberEarningsReports[profileID].earningsReport.gamePassTierEarningsGold[tier];
	}

	private void UpdateEarningsText(int tier1Earnings, int tier2Earnings, int tier3Earnings, int totalEarnings)
	{
		totalEarningsAmount.text = totalEarnings.ToString();
		if ((float)totalEarnings <= 0f)
		{
			tier1EarningPercentage.text = "Game Tier 1";
			tier2EarningPercentage.text = "Game Tier 2";
			tier3EarningPercentage.text = "Game Tier 3";
			return;
		}
		int num = GetPercentage(tier1Earnings, totalEarnings);
		int num2 = GetPercentage(tier2Earnings, totalEarnings);
		int num3 = GetPercentage(tier3Earnings, totalEarnings);
		int num4 = num + num2 + num3;
		if (num4 < 100)
		{
			int num5 = 0;
			while (num4 < 100)
			{
				num4++;
				if (num5 <= 0 && !IsPercentageWhole(tier1Earnings, totalEarnings))
				{
					num++;
				}
				else if (num5 <= 1 && !IsPercentageWhole(tier2Earnings, totalEarnings))
				{
					num2++;
				}
				else
				{
					num3++;
				}
				num5++;
			}
		}
		tier1EarningPercentage.text = "Game Tier 1 (" + num + "%)";
		tier2EarningPercentage.text = "Game Tier 2 (" + num2 + "%)";
		tier3EarningPercentage.text = "Game Tier 3 (" + num3 + "%)";
	}

	private void UpdateEarningPieChart(int tier1Earnings, int tier2Earnings, int tier3Earnings, int totalEarnings)
	{
		float num;
		float num2;
		if (totalEarnings <= 0)
		{
			num = 0.34f;
			tier1CircularImage.fillAmount = num;
			num2 = 0.33f;
			tier2CircularImage.fillAmount = num2;
			tier3CircularImage.fillAmount = 0.33f;
		}
		else
		{
			num = (float)tier1Earnings / (float)totalEarnings;
			tier1CircularImage.fillAmount = num;
			num2 = (float)tier2Earnings / (float)totalEarnings;
			tier2CircularImage.fillAmount = num2;
			tier3CircularImage.fillAmount = (float)tier3Earnings / (float)totalEarnings;
		}
		float angle = -360f * num;
		tier2CircularImage.transform.Rotate(Vector3.forward, angle);
		float angle2 = -360f * (num + num2);
		tier3CircularImage.transform.Rotate(Vector3.forward, angle2);
	}

	private int GetPercentage(int tierEarnings, int totalEarnings)
	{
		if (totalEarnings <= 0)
		{
			return 33;
		}
		float f = (float)tierEarnings / (float)totalEarnings * 100f;
		return Mathf.FloorToInt(f);
	}

	private bool IsPercentageWhole(int tierEarnings, int totalEarnings)
	{
		if (totalEarnings <= 0)
		{
			return false;
		}
		float num = (float)tierEarnings / (float)totalEarnings * 100f;
		int num2 = Mathf.FloorToInt(num);
		if (num != (float)num2)
		{
			return false;
		}
		return true;
	}

	private void UpdateDisplayedText()
	{
		if (currentFocusedTier == GamePassTier.Tier0)
		{
			displayedSmallGoldIcon.SetActive(value: false);
			displayedLargeGoldIcon.SetActive(value: true);
			displayedEarningsDescriptionText.text = string.Empty;
			displayedEarningsAmountText.text = string.Empty;
		}
		else
		{
			displayedSmallGoldIcon.SetActive(value: true);
			displayedLargeGoldIcon.SetActive(value: false);
			displayedEarningsDescriptionText.text = "Game Tier " + (int)currentFocusedTier;
			displayedEarningsAmountText.text = tierEarnings[(int)currentFocusedTier].ToString();
		}
	}

	private void ResetHighlightEffects()
	{
		currentFocusedTier = GamePassTier.Tier0;
		interpolationStartTime = 0f;
		for (int i = 0; i < transformsToExpand.Count; i++)
		{
			float x = originalWidthPositionsList[i];
			transformsToExpand[i].sizeDelta = new Vector2(x, transformsToExpand[i].sizeDelta.y);
		}
		for (int j = 0; j < tierOutlineList.Count; j++)
		{
			tierOutlineList[j].alpha = 0f;
		}
		for (int k = 0; k < pieChartsToEnlargen.Count; k++)
		{
			float num = originalSizeList[k];
			pieChartsToEnlargen[k].sizeDelta = new Vector2(num, num);
		}
	}

	private void InstantiateGamePassesShop(GamePassTier tierToShow)
	{
		GamePassesShop gamePassesShop = UnityEngine.Object.Instantiate(gamePassesShopPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gamePassesShop.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		gamePassesShop.Initialize(tierToShow, new List<int>());
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
		UpdateDisplayedText();
	}

	public void OnTierDetailExit(GamePassTier tierExited)
	{
		currentFocusedTier = GamePassTier.Tier0;
		interpolationStartTime = Time.time;
		UpdateDisplayedText();
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
		GamePassesHighScoreList highScoreList = UnityEngine.Object.Instantiate(highScoreListPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(highScoreList.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}
}
