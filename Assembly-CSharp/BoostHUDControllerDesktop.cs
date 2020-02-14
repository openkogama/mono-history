using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostHUDControllerDesktop : MonoBehaviour
{
	[SerializeField]
	private HorizontalLayoutGroup content;

	[SerializeField]
	private BoostImageController boostImageController;

	private List<GameObject> currentBoosts = new List<GameObject>();

	private void Start()
	{
		BoostController boostController = MVGameControllerBase.LocalPlayer.BoostController;
		boostController.BoostCountChanged = (Action)Delegate.Combine(boostController.BoostCountChanged, new Action(CreateActiveBoosts));
	}

	private void CreateActiveBoosts()
	{
		for (int num = currentBoosts.Count - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(currentBoosts[num]);
		}
		currentBoosts.Clear();
		BoostController boostController = MVGameControllerBase.LocalPlayer.BoostController;
		Dictionary<BoostType, Boost>.ValueCollection activeBoosts = boostController.GetActiveBoosts();
		foreach (Boost item in activeBoosts)
		{
			Image image = UnityEngine.Object.Instantiate(boostImageController.GetBoostVisualization(item.Type));
			currentBoosts.Add(image.gameObject);
			image.transform.SetParent(content.transform, worldPositionStays: false);
		}
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			BoostController boostController = MVGameControllerBase.LocalPlayer.BoostController;
			boostController.BoostCountChanged = (Action)Delegate.Remove(boostController.BoostCountChanged, new Action(CreateActiveBoosts));
		}
	}
}
