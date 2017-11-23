using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class AdvancedGhostBodyRotateWeapon : MonoBehaviour
{
	private float damage = 55f;

	private float impulseStrength = 2500f;

	private float factor = 1f;

	private TimeoutMap timeoutMap = new TimeoutMap(0.5f);

	private AudioSource weaponHitSound;

	public MVTeam alliedTeam = MVTeam.Server;

	private List<AdvancedGhostTriggerBase> ghostTriggers = new List<AdvancedGhostTriggerBase>();

	public MVTeam AlliedTeam
	{
		get
		{
			return alliedTeam;
		}
		set
		{
			alliedTeam = value;
		}
	}

	public void SetAttackValueFactor(float factor)
	{
		this.factor = factor;
	}

	public void Init(AudioSource weaponHitSound, MVCubeModelBase body)
	{
		this.weaponHitSound = weaponHitSound;
		body.Changed = (Action<CubeModelChangedEventArgs>)Delegate.Combine(body.Changed, new Action<CubeModelChangedEventArgs>(body_Changed));
		SetupWeaponCollision();
	}

	private void body_Changed(CubeModelChangedEventArgs e)
	{
		SetupWeaponCollision();
	}

	private void SetupWeaponCollision()
	{
		ghostTriggers.Clear();
		foreach (Transform item in base.transform)
		{
			if (item.GetComponent<Collider>() != null)
			{
				item.GetComponent<Collider>().isTrigger = true;
				AdvancedGhostTriggerBase advancedGhostTriggerBase = item.gameObject.GetComponent<AdvancedGhostTriggerBase>();
				if (advancedGhostTriggerBase == null)
				{
					advancedGhostTriggerBase = item.gameObject.AddComponent<AdvancedGhostTriggerBase>();
				}
				ghostTriggers.Add(advancedGhostTriggerBase);
			}
		}
	}

	private void Update()
	{
		if (factor <= 0f)
		{
			return;
		}
		timeoutMap.Update();
		foreach (AdvancedGhostTriggerBase ghostTrigger in ghostTriggers)
		{
			int[] attackTargets = ghostTrigger.AttackTargets;
			foreach (int woid in attackTargets)
			{
				Attack(woid);
			}
		}
	}

	private void Attack(int woid)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woid);
		if (worldObjectClient == null || timeoutMap.Contains(worldObjectClient.Id))
		{
			return;
		}
		InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
		if (interactionDataHandlerBase != null)
		{
			float num = ((MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(worldObjectClient.OwnerActorNr) != AlliedTeam || MVGameControllerBase.Game.TeamManager.TeamCount() <= 1) ? (damage * factor) : 0f);
			Vector3 vector = (worldObjectClient.GetTargetPosition() - gameObject.transform.position).normalized * impulseStrength;
			InteractionData interaction = AdvancedGhostBodyRotateWeaponPackage.Create(num, vector * factor);
			if (interactionDataHandlerBase.HandleInteraction(interaction, interactionIsLocal: true))
			{
				timeoutMap.Add(worldObjectClient.Id);
				weaponHitSound.Play();
			}
		}
		else
		{
			Debug.LogError("WorldObject does not have interactionHandler");
		}
	}
}
