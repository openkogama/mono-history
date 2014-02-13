using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVSmoke : MVLogicObject
{
	private const string prefabPath = "Prefabs/SmokeObject";

	private GameObject particleGO;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVSmoke(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/SmokeObject", worldObjects)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected Obj, but got Unknown
		particleGO = (GameObject)Object.Instantiate(Resources.Load("ParticleFX/FluffySmoke"), gameObject.transform.position, Quaternion.identity);
		particleGO.transform.parent = gameObject.transform;
		ToggleEmitter(toggle: false);
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
		foreach (ParticleEmitter val in componentsInChildren)
		{
			val.emit = toggle;
		}
	}
}
