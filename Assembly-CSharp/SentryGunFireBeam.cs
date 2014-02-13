using UnityEngine;

public class SentryGunFireBeam : SentryGunBeam
{
	public ParticleSystem fireParticles;

	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = EndPosition - StartPosition;
		fireParticles.startSpeed = val.magnitude;
	}
}
