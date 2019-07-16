using System;
using System.Collections.Generic;
using GameMeterVisuals;
using UnityEngine;
using UnityEngine.UI;

public class BoostHUDController : MonoBehaviour
{
	[SerializeField]
	private HorizontalLayoutGroup content;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private SlideOnClick slideOnClick;

	[SerializeField]
	private BoostImageController boostImageController;

	[SerializeField]
	private int boosterWidth = 100;

	private List<GameObject> currentBoosts = new List<GameObject>();

	private Vector2 startPos;

	private RectTransform rectTransform;

	private void Start()
	{
		BoostController boostController = MVGameControllerBase.LocalPlayer.BoostController;
		boostController.BoostCountChanged = (Action)Delegate.Combine(boostController.BoostCountChanged, new Action(SetupHUD));
		rectTransform = (RectTransform)transform;
		startPos = rectTransform.anchoredPosition;
		SetupHUD();
	}

	private void SetupHUD()
	{
		CreateActiveBoosts();
		SetupTransform();
	}

	private void SetupTransform()
	{
		int count = MVGameControllerBase.LocalPlayer.BoostController.GetActiveBoosts().Count;
		RectTransform rectTransform = (RectTransform)transform;
		slideOnClick.SetNewStartPosition(new Vector3((float)(count * boosterWidth) + startPos.x, startPos.y, 0f));
		canvasGroup.alpha = 0f;
		if (count > 0)
		{
			canvasGroup.alpha = 1f;
		}
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
			BoostRadialUpdate boostRadialUpdate = UnityEngine.Object.Instantiate(boostImageController.GetBoostVisualization(item.Type));
			boostRadialUpdate.Initialize(item);
			currentBoosts.Add(boostRadialUpdate.gameObject);
			boostRadialUpdate.transform.SetParent(content.transform, worldPositionStays: false);
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
