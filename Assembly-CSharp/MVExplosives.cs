using UnityEngine;

public class MVExplosives : MVLogicObject
{
	private float damageRadius = 10f;

	private float damageValue = 150f;

	private float shockwaveAcceleration = 3500f;

	private GameObject particleGO;

	private GameObject audioGO;

	private AudioLogicCube audioLC;

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
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ExplosivesObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
		particleGO = (GameObject)Object.Instantiate(Resources.Load("ParticleFX/Explosion"), Vector3.zero, Quaternion.identity);
		particleGO.transform.parent = gameObject.transform;
	}

	public override void OnInputStateChanged()
	{
		if (InputState)
		{
			Explode();
		}
	}

	private void ApplyProximityDamage()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		MVPlayer localPlayer = MVGameController.Instance.WOCM.LocalPlayer;
		float num = Vector3.Distance(((Component)localPlayer.Avatar.Avatar).gameObject.transform.position, gameObject.transform.position);
		if (num <= damageRadius)
		{
			float num2 = 1f - num / damageRadius;
			float damage = damageValue * num2;
			localPlayer.Avatar.AvatarController.ApplyProximityDamage(damage);
			AvatarController avatarController = localPlayer.Avatar.AvatarController;
			Vector3 val = ((Component)localPlayer.Avatar.Avatar).gameObject.transform.position - gameObject.transform.position;
			avatarController.ApplyImpulse(val.normalized * num2 * shockwaveAcceleration, suspendImpactDamage: true);
		}
	}

	public void Explode()
	{
		ParticleEmitter[] componentsInChildren = particleGO.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter val in componentsInChildren)
		{
			val.Emit();
		}
		ApplyProximityDamage();
		gameObject.audio.Play();
	}
}
