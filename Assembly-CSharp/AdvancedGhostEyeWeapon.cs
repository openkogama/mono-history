using UnityEngine;

public class AdvancedGhostEyeWeapon : AdvancedGhostTriggerBase
{
	private float impulseStrength = 2000f;

	private TimeoutMap timeoutMap = new TimeoutMap(0.5f);

	public AudioSource doDamage;

	private void Update()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		timeoutMap.Update();
		foreach (int attackTarget in attackTargets)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(attackTarget);
			if (worldObjectClient != null && !timeoutMap.Contains(worldObjectClient.Id))
			{
				MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
				if ((Object)(object)component == (Object)null)
				{
					Debug.LogWarning((object)"WorldObject does not have rigidBody");
					continue;
				}
				Vector3 val = worldObjectClient.GetTargetPosition() - (((Component)this).gameObject.transform.position - Vector3.up);
				Vector3 impulse = val.normalized * impulseStrength;
				component.AddImpulse(impulse);
				timeoutMap.Add(worldObjectClient.Id);
				doDamage.Play();
			}
		}
	}
}
