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

	private List<AdvancedGhostTriggerBase> ghostTriggers = new List<AdvancedGhostTriggerBase>();

	public void SetAttackValueFactor(float factor)
	{
		this.factor = factor;
	}

	public void Init(AudioSource weaponHitSound, MVCubeModelBase body)
	{
		this.weaponHitSound = weaponHitSound;
		body.Changed += body_Changed;
		SetupWeaponCollision();
	}

	private void body_Changed(object sender, CubeModelChangedEventArgs e)
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
			foreach (int id in attackTargets)
			{
				MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(id);
				if (worldObjectClient == null)
				{
					continue;
				}
				InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
				if (timeoutMap.Contains(worldObjectClient.Id))
				{
					continue;
				}
				if (component == null)
				{
					Debug.LogError("WorldObject does not have interactionHandler");
					continue;
				}
				Vector3 vector = (worldObjectClient.GetTargetPosition() - gameObject.transform.position).normalized * impulseStrength;
				InteractionData interaction = AdvancedGhostBodyRotateWeaponPackage.Create(damage * factor, vector * factor);
				if (component.HandleInteraction(interaction, interactionIsLocal: true))
				{
					timeoutMap.Add(worldObjectClient.Id);
					weaponHitSound.Play();
				}
			}
		}
	}
}
