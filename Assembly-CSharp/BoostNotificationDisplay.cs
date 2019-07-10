using System;
using UnityEngine;
using UnityEngine.UI;

public class BoostNotificationDisplay : MonoBehaviour
{
	[SerializeField]
	private Text boostsActive;

	[SerializeField]
	private Image backgroundColor;

	[SerializeField]
	private Color boostActiveColor;

	[SerializeField]
	private Color boostInactiveColor;

	private void Start()
	{
		BoostController boostController = MVGameControllerBase.Game.LocalPlayer.BoostController;
		boostController.BoostCountChanged = (Action<int>)Delegate.Combine(boostController.BoostCountChanged, new Action<int>(OnBoostCountChanged));
	}

	private void OnBoostCountChanged(int activeBoostsCount)
	{
		boostsActive.text = activeBoostsCount.ToString();
		backgroundColor.color = ((activeBoostsCount <= 0) ? boostInactiveColor : boostActiveColor);
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			BoostController boostController = MVGameControllerBase.Game.LocalPlayer.BoostController;
			boostController.BoostCountChanged = (Action<int>)Delegate.Remove(boostController.BoostCountChanged, new Action<int>(OnBoostCountChanged));
		}
	}
}
