using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVFire : MVLogicObject
{
	private const string prefabPath = "Prefabs/FireObject";

	private GameObject particleGO;

	private GameObject audioGO;

	private float ignoreDistanceSqr = 100f;

	private float damageRadius = 2.5f;

	private float damageValue = 100f;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVFire(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/FireObject", worldObjects)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected Obj, but got Unknown
		particleGO = (GameObject)Object.Instantiate(Resources.Load("ParticleFX/Fire1"), gameObject.transform.position, Quaternion.identity);
		particleGO.transform.parent = gameObject.transform;
		ParticleEmitter[] componentsInChildren = particleGO.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter val in componentsInChildren)
		{
			val.emit = false;
		}
		gameObject.audio.pitch = 1f + Random.Range(-0.2f, 0.2f);
	}

	protected override void OnUpdate()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (!InputState && InputLinkRefs.Count != 0)
		{
			return;
		}
		HashSet<int> localControlledWorldObjects = MVGameController.Instance.Game.PlayerController.LocalControlledWorldObjects;
		Vector3 val = transform.position;
		foreach (int item in localControlledWorldObjects)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(item);
			if (worldObjectClient == null)
			{
				continue;
			}
			InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
			if ((Object)(object)component == (Object)null)
			{
				continue;
			}
			Vector3 val2 = worldObjectClient.WorldPosition - val;
			if (!(val2.sqrMagnitude > ignoreDistanceSqr))
			{
				float num = damageRadius;
				if ((Object)(object)worldObjectClient.GameObject.collider != (Object)null)
				{
					Vector3 val3 = worldObjectClient.GameObject.collider.ClosestPointOnBounds(val);
					num = Vector3.Distance(val3, val);
				}
				else
				{
					Vector3.Distance(worldObjectClient.GameObject.transform.position, val);
				}
				if (num <= damageRadius)
				{
					float damage = Time.deltaTime * damageValue * (1f - num / damageRadius);
					component.HandleInteraction(ProximityDamageAndImpulse.Create(damage, Vector3.zero, PlayerKilledByType.Fire), interactionIsLocal: true);
				}
			}
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		if (InputLinkRefs.Count == 0)
		{
			ToggleEmitter(toggle: true);
		}
	}

	public override void OnInputLinkChanged()
	{
		if (InputLinkRefs.Count == 0)
		{
			ToggleEmitter(toggle: true);
		}
		else
		{
			OnInputStateChanged();
		}
	}

	public override void OnInputStateChanged()
	{
		Debug.Log((object)("InputState " + InputState));
		if (InputState)
		{
			ToggleEmitter(toggle: true);
		}
		else
		{
			ToggleEmitter(toggle: false);
		}
	}

	private void ToggleEmitter(bool toggle)
	{
		ParticleEmitter[] componentsInChildren = particleGO.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter val in componentsInChildren)
		{
			val.emit = toggle;
		}
		if (toggle)
		{
			gameObject.audio.Play();
		}
		else
		{
			gameObject.audio.Stop();
		}
	}
}
