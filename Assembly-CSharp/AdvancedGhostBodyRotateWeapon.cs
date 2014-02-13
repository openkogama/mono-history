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
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected Obj, but got Unknown
		ghostTriggers.Clear();
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val = item;
			if ((Object)(object)((Component)val).collider != (Object)null)
			{
				((Component)val).collider.isTrigger = true;
				AdvancedGhostTriggerBase advancedGhostTriggerBase = ((Component)val).gameObject.GetComponent<AdvancedGhostTriggerBase>();
				if ((Object)(object)advancedGhostTriggerBase == (Object)null)
				{
					advancedGhostTriggerBase = ((Component)val).gameObject.AddComponent<AdvancedGhostTriggerBase>();
				}
				ghostTriggers.Add(advancedGhostTriggerBase);
			}
		}
	}

	private void Update()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
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
				MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(id);
				if (worldObjectClient == null)
				{
					continue;
				}
				InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
				if (timeoutMap.Contains(worldObjectClient.Id))
				{
					continue;
				}
				if ((Object)(object)component == (Object)null)
				{
					Debug.LogError((object)"WorldObject does not have interactionHandler");
					continue;
				}
				Vector3 val = worldObjectClient.GetTargetPosition() - ((Component)this).gameObject.transform.position;
				Vector3 val2 = val.normalized * impulseStrength;
				InteractionData interaction = AdvancedGhostBodyRotateWeaponPackage.Create(damage * factor, val2 * factor);
				if (component.HandleInteraction(interaction, interactionIsLocal: true))
				{
					timeoutMap.Add(worldObjectClient.Id);
					weaponHitSound.Play();
				}
			}
		}
	}
}
