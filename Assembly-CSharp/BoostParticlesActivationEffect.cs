using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class BoostParticlesActivationEffect : MonoBehaviour
{
	[Serializable]
	private struct BoosterColors
	{
		public BoostType type;

		public Color rayColor;

		public Color bubbleColor;
	}

	[SerializeField]
	private ParticleSystem rayParticles;

	[SerializeField]
	private ParticleSystem bubbleParticles;

	[SerializeField]
	private List<BoosterColors> boostColors;

	[SerializeField]
	private float activationCooldown;

	private SpawnRoleModeType previousMode;

	private float activationStartTime;

	private List<BoostType> boostsToActivate = new List<BoostType>();

	private void Start()
	{
		MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.SpawnRoleMode.OnChange += OnAvatarModeChange;
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.SpawnRoleMode.OnChange -= OnAvatarModeChange;
		}
	}

	private void Update()
	{
		if (activationStartTime + activationCooldown <= Time.time && boostsToActivate.Count > 0)
		{
			ActivateParticles(boostsToActivate[0]);
			boostsToActivate.RemoveAt(0);
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
		boostsToActivate.Clear();
		BoostController boostController = MVGameControllerBase.Game.LocalPlayer.BoostController;
		Dictionary<BoostType, Boost>.ValueCollection allBoosts = boostController.GetAllBoosts();
		foreach (Boost item in allBoosts)
		{
			if (MVGameControllerBase.LocalPlayer.BoostController.IsBoostActive(item.Type))
			{
				boostsToActivate.Add(item.Type);
			}
		}
	}

	private void ActivateParticles(BoostType typeToActivate)
	{
		for (int i = 0; i < boostColors.Count; i++)
		{
			if (boostColors[i].type == typeToActivate)
			{
				ParticleSystem.MainModule main = rayParticles.main;
				main.startColor = boostColors[i].rayColor;
				rayParticles.Play();
				ParticleSystem.MainModule main2 = bubbleParticles.main;
				main2.startColor = boostColors[i].bubbleColor;
				bubbleParticles.Play();
			}
		}
		activationStartTime = Time.time;
	}
}
