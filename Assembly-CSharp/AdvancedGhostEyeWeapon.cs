using UnityEngine;

public class AdvancedGhostEyeWeapon : AdvancedGhostTriggerBase
{
	private float impulseStrength = 2000f;

	private TimeoutMap timeoutMap = new TimeoutMap(0.5f);

	public AudioSource doDamage;

	private void Update()
	{
		timeoutMap.Update();
		foreach (int attackTarget in attackTargets)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(attackTarget);
			if (worldObjectClient != null && !timeoutMap.Contains(worldObjectClient.Id))
			{
				MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
				if (!(component == null))
				{
					Vector3 impulse = (worldObjectClient.GetTargetPosition() - (gameObject.transform.position - Vector3.up)).normalized * impulseStrength;
					component.AddImpulse(impulse);
					timeoutMap.Add(worldObjectClient.Id);
					doDamage.Play();
				}
			}
		}
	}
}
