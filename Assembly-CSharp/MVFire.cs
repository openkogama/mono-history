using UnityEngine;

public class MVFire : MVLogicObject
{
	private GameObject particleGO;

	private GameObject audioGO;

	private float damageRadius = 2.5f;

	private float damageValue = 100f;

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
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/FireObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
		particleGO = (GameObject)Object.Instantiate(Resources.Load("ParticleFX/Fire1"), Vector3.zero, Quaternion.identity);
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
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (InputState || InputLinkRefs.Count == 0)
		{
			MVPlayer localPlayer = MVGameController.Instance.WOCM.LocalPlayer;
			float num = Vector3.Distance(((Component)localPlayer.Avatar.Avatar).gameObject.transform.position, gameObject.transform.position);
			if (num <= damageRadius)
			{
				float damage = Time.deltaTime * damageValue * (1f - num / damageRadius);
				localPlayer.Avatar.AvatarController.ApplyProximityDamage(damage);
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
