using UnityEngine;

public class SentryGunFireBeam : SentryGunBeam
{
	public ParticleSystem fireParticles;

	protected override void OnUpdate()
	{
		Vector3 vector = EndPosition - StartPosition;
		ParticleSystem.MainModule main = fireParticles.main;
		main.startSpeedMultiplier = vector.magnitude;
	}
}
