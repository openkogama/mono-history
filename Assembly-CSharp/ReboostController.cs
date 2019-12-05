using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReboostController : MonoBehaviour
{
	[SerializeField]
	private GameObject ReboostUI;

	[SerializeField]
	private Image reboostBackground;

	[SerializeField]
	private Transform boostIconContainer;

	[SerializeField]
	private BoostIconManager boostIconManager;

	private List<BoostType> boostsToReboost = new List<BoostType>();

	private List<GameObject> boostIcons = new List<GameObject>();

	private void Start()
	{
		UpdateReboostUI();
	}

	private void OnEnable()
	{
		UpdateReboostUI();
	}

	private void UpdateReboostUI()
	{
		boostsToReboost.Clear();
		for (int i = 0; i < boostIcons.Count; i++)
		{
			Object.Destroy(boostIcons[i]);
		}
		boostIcons.Clear();
		bool flag = MVGameControllerBase.Game.LocalPlayer.BoostController.HasExpiredBoosts();
		bool flag2 = false;
		flag2 = MVClientSettings.BoostersEnabled;
		ReboostUI.SetActive(flag && flag2);
		if (flag && flag2)
		{
			boostsToReboost = MVGameControllerBase.Game.LocalPlayer.BoostController.GetCurrentAndExpiredBoosts();
			for (int j = 0; j < boostsToReboost.Count; j++)
			{
				GameObject gameObject = boostIconManager.CreateBoosterIcon(boostsToReboost[j]);
				gameObject.transform.SetParent(boostIconContainer, worldPositionStays: false);
				boostIcons.Add(gameObject);
			}
		}
	}

	public void OnReboostWithAdClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IBoostAdController x, BaseEventData y) =>
		{
			x.TryShowAdForReboost(boostsToReboost[0], ReboostResponse);
		});
	}

	public void ReboostResponse(bool finishedAd)
	{
		if (finishedAd)
		{
			for (int i = 0; i < boostsToReboost.Count; i++)
			{
				MVGameControllerBase.Game.LocalPlayer.BoostController.ActivateOrRenewBoost(boostsToReboost[i]);
			}
			UpdateReboostUI();
		}
	}

	public void CancelReboost()
	{
		MVGameControllerBase.Game.LocalPlayer.BoostController.RemoveAllExpiredBoosts();
		UpdateReboostUI();
	}

	public void ChangeBackgroundAlpha(float newAlpha)
	{
		Color color = reboostBackground.color;
		color.a = newAlpha;
		reboostBackground.color = color;
	}
}
