using UnityEngine;

public class MVSmoke : MVLogicObject
{
	private GameObject particleGO;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	protected override void CreateMVWOC(bool local)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected Obj, but got Unknown
		interactionFlags = InteractionFlags.Selectable;
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/SmokeObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
		particleGO = (GameObject)Object.Instantiate(Resources.Load("ParticleFX/FluffySmoke"), Vector3.zero, Quaternion.identity);
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
		else
		{
			ToggleEmitter(toggle: false);
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
