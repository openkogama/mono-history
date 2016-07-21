using System.Collections.Generic;
using UnityEngine;

public class MVSmoke : MVLogicObject
{
	private ParticleSystem particleSystem;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVSmoke(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSmokePrefab, worldObjects)
	{
		particleSystem = Object.Instantiate(PrefabPool.Instance.ParticleFluffySmoke, gameObject.transform.position, Quaternion.identity) as ParticleSystem;
		particleSystem.transform.parent = gameObject.transform;
		ToggleEmitter(toggle: false);
	}

	public override void Initialize()
	{
		base.Initialize();
		if (InputLinkRefs.Count == 0)
		{
			ToggleEmitter(toggle: true);
		}
		SetupCulling(particleSystem.gameObject);
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
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.enabled = toggle;
	}
}
