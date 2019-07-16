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
		boostController.BoostCountChanged = (Action)Delegate.Combine(boostController.BoostCountChanged, new Action(OnBoostCountChanged));
		OnBoostCountChanged();
	}

	private void OnBoostCountChanged()
	{
		int count = MVGameControllerBase.Game.LocalPlayer.BoostController.GetActiveBoosts().Count;
		boostsActive.text = count.ToString();
		backgroundColor.color = ((count <= 0) ? boostInactiveColor : boostActiveColor);
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			BoostController boostController = MVGameControllerBase.Game.LocalPlayer.BoostController;
			boostController.BoostCountChanged = (Action)Delegate.Remove(boostController.BoostCountChanged, new Action(OnBoostCountChanged));
		}
	}
}
