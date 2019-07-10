using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostMenuItem : MonoBehaviour
{
	[Serializable]
	private struct BoosterDef
	{
		public BoostType type;

		public GameObject iconPrefab;
	}

	[SerializeField]
	private GameObject boostUnlockedGlow;

	[SerializeField]
	private GameObject boostActiveIcon;

	[SerializeField]
	private RectTransform boostTypeImageParent;

	[SerializeField]
	private Button getWithAd;

	[SerializeField]
	private Text boostDescription;

	[SerializeField]
	private List<BoosterDef> boosterList;

	private BoostType boostType;

	public void Initialize(Boost boost, bool boostUnlocked)
	{
		boostType = boost.Type;
		boostDescription.text = boost.Description;
		SetBoostUIUnlocked(boostUnlocked);
		for (int i = 0; i < boosterList.Count; i++)
		{
			if (boosterList[i].type == boost.Type)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(boosterList[i].iconPrefab);
				gameObject.transform.SetParent(boostTypeImageParent, worldPositionStays: false);
				break;
			}
		}
	}

	private void SetBoostUIUnlocked(bool boostUnlocked)
	{
		boostUnlockedGlow.SetActive(boostUnlocked);
		boostActiveIcon.SetActive(boostUnlocked);
		getWithAd.gameObject.SetActive(!boostUnlocked);
	}

	public void OnUnlockBoostWithAdClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IBoostAdController x, BaseEventData y) =>
		{
			x.TryShowAd(boostType, BoostUnlockedResponse);
		});
	}

	private void BoostUnlockedResponse(bool boostUnlocked)
	{
		SetBoostUIUnlocked(boostUnlocked);
	}
}
