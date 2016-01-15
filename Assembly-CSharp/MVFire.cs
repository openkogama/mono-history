using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVFire : MVLogicObject
{
	private GameObject particleGO;

	private GameObject audioGO;

	private AudioSource audioSource;

	private float ignoreDistanceSqr = 100f;

	private float damageRadius = 2.5f;

	private float damageValue = 100f;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVFire(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVFirePrefab, worldObjects)
	{
		particleGO = (GameObject)Object.Instantiate(PrefabPool.Instance.ParticleFire1, gameObject.transform.position, Quaternion.identity);
		particleGO.transform.parent = gameObject.transform;
		ParticleEmitter[] componentsInChildren = particleGO.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			particleEmitter.emit = false;
		}
		audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.pitch = 1f + Random.Range(-0.2f, 0.2f);
	}

	protected override void OnUpdate()
	{
		if (!InputState && InputLinkRefs.Count != 0)
		{
			return;
		}
		HashSet<int> localControlledWorldObjects = MVGameControllerBase.Game.PlayerController.LocalControlledWorldObjects;
		Vector3 vector = transform.position;
		foreach (int item in localControlledWorldObjects)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(item);
			if (worldObjectClient == null)
			{
				continue;
			}
			InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
			if (!(interactionDataHandlerBase == null) && !((worldObjectClient.WorldPosition - vector).sqrMagnitude > ignoreDistanceSqr))
			{
				float num = damageRadius;
				if (worldObjectClient.Collider != null)
				{
					Vector3 a = worldObjectClient.Collider.ClosestPointOnBounds(vector);
					num = Vector3.Distance(a, vector);
				}
				else
				{
					Vector3.Distance(worldObjectClient.GameObject.transform.position, vector);
				}
				if (num <= damageRadius)
				{
					float damage = Time.deltaTime * damageValue * (1f - num / damageRadius);
					interactionDataHandlerBase.HandleInteraction(ProximityDamageAndImpulse.Create(damage, Vector3.zero, PlayerKilledByType.Fire), interactionIsLocal: true);
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
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			particleEmitter.emit = toggle;
		}
		if (toggle)
		{
			audioSource.Play();
		}
		else
		{
			audioSource.Stop();
		}
	}
}
