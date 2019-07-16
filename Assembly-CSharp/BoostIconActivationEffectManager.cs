using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class BoostIconActivationEffectManager : MonoBehaviour
{
	[SerializeField]
	private BoostIconActivationEffectController boostIconEffectPrefab;

	private List<BoostIconActivationEffectController> boostIconEffects = new List<BoostIconActivationEffectController>();

	private int nextActiveBoostEffect;

	private SpawnRoleModeType previousMode;

	private void Start()
	{
		MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.SpawnRoleMode.OnChange += OnAvatarModeChange;
		BoostController boostController = MVGameControllerBase.Game.LocalPlayer.BoostController;
		Dictionary<BoostType, Boost>.ValueCollection allBoosts = boostController.GetAllBoosts();
		foreach (Boost item in allBoosts)
		{
			BoostIconActivationEffectController boostIconActivationEffectController = Object.Instantiate(boostIconEffectPrefab);
			boostIconActivationEffectController.transform.SetParent(transform, worldPositionStays: false);
			boostIconActivationEffectController.Initialize(item.Type, StartBoostIconEffect);
			boostIconEffects.Add(boostIconActivationEffectController);
		}
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.SpawnRoleMode.OnChange -= OnAvatarModeChange;
		}
	}

	private void OnAvatarModeChange(SpawnRoleModeType newMode)
	{
		if ((previousMode == SpawnRoleModeType.Hidden || previousMode == SpawnRoleModeType.Dead) && newMode == SpawnRoleModeType.Playing)
		{
			OnAvatarSpawn();
		}
		previousMode = newMode;
	}

	private void OnAvatarSpawn()
	{
		nextActiveBoostEffect = 0;
		StartBoostIconEffect();
	}

	private void StartBoostIconEffect()
	{
		if (boostIconEffects.Count > nextActiveBoostEffect)
		{
			nextActiveBoostEffect++;
			boostIconEffects[nextActiveBoostEffect - 1].Activate();
		}
	}
}
