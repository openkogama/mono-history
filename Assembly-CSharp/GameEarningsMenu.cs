using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.GamePassSystem.GamePassEarnings;
using UnityEngine;
using UnityEngine.UI;

public class GameEarningsMenu : MonoBehaviour
{
	[SerializeField]
	private Text totalEarningsAmount;

	[SerializeField]
	private Text boostEarningPercentage;

	[SerializeField]
	private Text tier1EarningPercentage;

	[SerializeField]
	private Text tier2EarningPercentage;

	[SerializeField]
	private Text tier3EarningPercentage;

	[SerializeField]
	private Image boostCircularImage;

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
	private List<RectTransform> transformsToExpand;

	[SerializeField]
	private List<RectTransform> pieChartsToEnlargen;

	[SerializeField]
	private float widthExpandAmount;

	[SerializeField]
	private float pieChartSizeIncreasePercentage;

	private List<int> tierEarnings;

	private List<float> originalWidthPositionsList;

	private List<float> originalSizeList;

	private float interpolationStartTime;

	private int currentFocusedEarning = -1;

	private const float outlineInterpolationSpeed = 2f;

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
			if (i == currentFocusedEarning)
			{
				num2 += widthExpandAmount;
			}
			float x = Mathf.Lerp(transformsToExpand[i].rect.width, num2, num);
			transformsToExpand[i].sizeDelta = new Vector2(x, transformsToExpand[i].sizeDelta.y);
		}
		for (int j = 0; j < pieChartsToEnlargen.Count; j++)
		{
			float num3 = originalSizeList[j];
			if (j == currentFocusedEarning)
			{
				num3 *= pieChartSizeIncreasePercentage;
			}
			float num4 = Mathf.Lerp(pieChartsToEnlargen[j].rect.width, num3, num);
			pieChartsToEnlargen[j].sizeDelta = new Vector2(num4, num4);
		}
	}

	private void SetUpWaitingForProjectEarningsReportUI()
	{
		GamePassesProjectEarningsManager.OnEarningsDataUpdated = (Action<ProjectEarningsReport>)Delegate.Combine(GamePassesProjectEarningsManager.OnEarningsDataUpdated, new Action<ProjectEarningsReport>(OnProjectEarningsUpdatedCallback));
		totalEarningsAmount.text = "??";
		boostEarningPercentage.text = "Boosters (??%)";
		tier1EarningPercentage.text = "Game Tier 1 (??%)";
		tier2EarningPercentage.text = "Game Tier 2 (??%)";
		tier3EarningPercentage.text = "Game Tier 3 (??%)";
		displayedEarningsDescriptionText.text = string.Empty;
		displayedEarningsAmountText.text = string.Empty;
		float num = 0.25f;
		boostCircularImage.fillAmount = num;
		float num2 = 0.25f;
		tier1CircularImage.fillAmount = num2;
		float num3 = 0.25f;
		tier2CircularImage.fillAmount = num3;
		tier3CircularImage.fillAmount = 0.25f;
		float angle = -360f * num;
		tier2CircularImage.transform.Rotate(Vector3.forward, angle);
		float angle2 = -360f * (num + num2);
		tier2CircularImage.transform.Rotate(Vector3.forward, angle2);
		float angle3 = -360f * (num + num2 + num3);
		tier3CircularImage.transform.Rotate(Vector3.forward, angle3);
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
		tierEarnings.Add(GetTotalBoostEarnings(projectEarningsReport));
		tierEarnings.Add(GetTierEarnings(projectEarningsReport, GamePassTier.Tier1));
		tierEarnings.Add(GetTierEarnings(projectEarningsReport, GamePassTier.Tier2));
		tierEarnings.Add(GetTierEarnings(projectEarningsReport, GamePassTier.Tier3));
		int boostersEarnings = tierEarnings[1];
		int tier1Earnings = tierEarnings[2];
		int tier2Earnings = tierEarnings[3];
		int tier3Earnings = tierEarnings[4];
		int totalEarnings = tierEarnings[0];
		UpdateEarningsText(boostersEarnings, tier1Earnings, tier2Earnings, tier3Earnings, totalEarnings);
		UpdateEarningPieChart(boostersEarnings, tier1Earnings, tier2Earnings, tier3Earnings, totalEarnings);
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

	private int GetTotalBoostEarnings(ProjectEarningsReport projectEarningsReport)
	{
		int profileID = MVGameControllerBase.Game.LocalPlayer.ProfileID;
		if (!projectEarningsReport.projectMemberEarningsReports.ContainsKey(profileID))
		{
			return 0;
		}
		int num = 0;
		foreach (int value in projectEarningsReport.projectMemberEarningsReports[profileID].earningsReport.gameBoosterEarningsGold.Values)
		{
			num += value;
		}
		return num;
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

	private void UpdateEarningsText(int boostersEarnings, int tier1Earnings, int tier2Earnings, int tier3Earnings, int totalEarnings)
	{
		totalEarningsAmount.text = totalEarnings.ToString("N0").Replace(",", ".");
		if ((float)totalEarnings <= 0f)
		{
			boostEarningPercentage.text = "Boosters (0%)";
			tier1EarningPercentage.text = "Game Tier 1 (0%)";
			tier2EarningPercentage.text = "Game Tier 2 (0%)";
			tier3EarningPercentage.text = "Game Tier 3 (0%)";
			return;
		}
		int num = GetPercentage(boostersEarnings, totalEarnings);
		int num2 = GetPercentage(tier1Earnings, totalEarnings);
		int num3 = GetPercentage(tier2Earnings, totalEarnings);
		int num4 = GetPercentage(tier3Earnings, totalEarnings);
		int num5 = num + num2 + num3 + num4;
		if (num5 < 100)
		{
			int num6 = 0;
			while (num5 < 100)
			{
				num5++;
				if (num6 <= 0 && !IsPercentageWhole(boostersEarnings, totalEarnings))
				{
					num++;
				}
				else if (num6 <= 1 && !IsPercentageWhole(tier1Earnings, totalEarnings))
				{
					num2++;
				}
				else if (num6 <= 2 && !IsPercentageWhole(tier2Earnings, totalEarnings))
				{
					num3++;
				}
				else
				{
					num4++;
				}
				num6++;
			}
		}
		boostEarningPercentage.text = "Boosters (" + num + "%)";
		tier1EarningPercentage.text = "Game Tier 1 (" + num2 + "%)";
		tier2EarningPercentage.text = "Game Tier 2 (" + num3 + "%)";
		tier3EarningPercentage.text = "Game Tier 3 (" + num4 + "%)";
	}

	private int GetPercentage(int earnings, int totalEarnings)
	{
		if (totalEarnings <= 0)
		{
			return 25;
		}
		float f = (float)earnings / (float)totalEarnings * 100f;
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

	private void UpdateEarningPieChart(int boostersEarnings, int tier1Earnings, int tier2Earnings, int tier3Earnings, int totalEarnings)
	{
		float num;
		float num2;
		float num3;
		if (totalEarnings <= 0)
		{
			num = 0.25f;
			boostCircularImage.fillAmount = num;
			num2 = 0.25f;
			tier1CircularImage.fillAmount = num2;
			num3 = 0.25f;
			tier2CircularImage.fillAmount = num3;
			tier3CircularImage.fillAmount = 0.25f;
		}
		else
		{
			num = (float)boostersEarnings / (float)totalEarnings;
			boostCircularImage.fillAmount = num;
			num2 = (float)tier1Earnings / (float)totalEarnings;
			tier1CircularImage.fillAmount = num2;
			num3 = (float)tier2Earnings / (float)totalEarnings;
			tier2CircularImage.fillAmount = num3;
			tier3CircularImage.fillAmount = (float)tier3Earnings / (float)totalEarnings;
		}
		float angle = -360f * num;
		tier1CircularImage.transform.Rotate(Vector3.forward, angle);
		float angle2 = -360f * (num + num2);
		tier2CircularImage.transform.Rotate(Vector3.forward, angle2);
		float angle3 = -360f * (num + num2 + num3);
		tier3CircularImage.transform.Rotate(Vector3.forward, angle3);
	}

	private void UpdateDisplayedText()
	{
		if (currentFocusedEarning == -1)
		{
			displayedSmallGoldIcon.SetActive(value: false);
			displayedLargeGoldIcon.SetActive(value: true);
			displayedEarningsDescriptionText.text = string.Empty;
			displayedEarningsAmountText.text = string.Empty;
		}
		else if (currentFocusedEarning == 0)
		{
			displayedSmallGoldIcon.SetActive(value: true);
			displayedLargeGoldIcon.SetActive(value: false);
			displayedEarningsDescriptionText.text = "Boosters ";
			displayedEarningsAmountText.text = tierEarnings[currentFocusedEarning + 1].ToString();
		}
		else
		{
			displayedSmallGoldIcon.SetActive(value: true);
			displayedLargeGoldIcon.SetActive(value: false);
			displayedEarningsDescriptionText.text = "Game Tier " + currentFocusedEarning;
			displayedEarningsAmountText.text = tierEarnings[currentFocusedEarning + 1].ToString();
		}
	}

	private void ResetHighlightEffects()
	{
		currentFocusedEarning = -1;
		interpolationStartTime = 0f;
		for (int i = 0; i < transformsToExpand.Count; i++)
		{
			float x = originalWidthPositionsList[i];
			transformsToExpand[i].sizeDelta = new Vector2(x, transformsToExpand[i].sizeDelta.y);
		}
		for (int j = 0; j < pieChartsToEnlargen.Count; j++)
		{
			float num = originalSizeList[j];
			pieChartsToEnlargen[j].sizeDelta = new Vector2(num, num);
		}
	}

	public void OnHighlightEarning(int earningEntered)
	{
		currentFocusedEarning = earningEntered;
		interpolationStartTime = Time.time;
		UpdateDisplayedText();
	}

	public void OnStopHighlightEarning(int earningExited)
	{
		currentFocusedEarning = -1;
		interpolationStartTime = Time.time;
		UpdateDisplayedText();
	}
}
