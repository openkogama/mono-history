using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVExplosives(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects) : MVLogicObject(data, "Prefabs/ExplosivesObject", worldObjects)
{
	private const string prefabPath = "Prefabs/ExplosivesObject";

	private float damageRadius = 10f;

	private float damageValue = 150f;

	private float shockwaveAcceleration = 3500f;

	private GameObject audioGO;

	private AudioLogicCube audioLC;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public override void OnInputStateChanged()
	{
		if (InputState)
		{
			Explode();
		}
	}

	private void ApplyProximityDamage()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		Collider[] array = Physics.OverlapSphere(gameObject.transform.position, damageRadius, 1 << LayerMask.NameToLayer("Player"));
		Collider[] array2 = array;
		foreach (Collider val in array2)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)val).transform);
			if (mVObject == null)
			{
				continue;
			}
			float num = Vector3.Distance(((Component)val).collider.ClosestPointOnBounds(gameObject.transform.position), gameObject.transform.position);
			if (num <= damageRadius)
			{
				InteractionDataHandlerBase component = mVObject.GameObject.GetComponent<InteractionDataHandlerBase>();
				if (!((Object)(object)component == (Object)null))
				{
					float num2 = 1f - num / damageRadius;
					float damage = damageValue * num2;
					Vector3 val2 = mVObject.GameObject.transform.position - gameObject.transform.position;
					Vector3 impulse = val2.normalized * num2 * shockwaveAcceleration;
					InteractionData interaction = ProximityDamageAndImpulse.Create(damage, impulse, PlayerKilledByType.Explosive);
					component.HandleInteraction(interaction, interactionIsLocal: true);
				}
			}
		}
	}

	public void Explode()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Object.Instantiate(Resources.Load("ParticleFX/Explosion"), gameObject.transform.position, Quaternion.identity);
		ApplyProximityDamage();
	}
}
