using UnityEngine;

public class SentryGunFireBeam : SentryGunBeam
{
	public ParticleSystem fireParticles;

	protected override void OnUpdate()
	{
		Vector3 vector = EndPosition - StartPosition;
		fireParticles.startSpeed = vector.magnitude;
	}
}
