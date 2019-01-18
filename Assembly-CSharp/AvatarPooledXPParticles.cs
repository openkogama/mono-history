using UnityEngine;

public class AvatarPooledXPParticles : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem xpParticleSystem;

	private bool hasStarted;

	private float initStartTime;

	private const float waitBeforeStartDuration = 1.2f;

	private const float particleMultipier = 1f;

	private void Update()
	{
		if (!hasStarted && Time.time > initStartTime + 1.2f)
		{
			xpParticleSystem.Play();
			hasStarted = true;
		}
		if (!xpParticleSystem.isPlaying && hasStarted)
		{
			PrefabPool.Instance.EnumPoolManager.Return(this, PoolEnums.XP);
		}
	}

	public void Initialize(int xpDelta)
	{
		hasStarted = false;
		xpParticleSystem.Stop();
		ParticleSystem.EmissionModule emission = xpParticleSystem.emission;
		ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
		ParticleSystem.MainModule main = xpParticleSystem.main;
		float constant = rateOverTime.constant;
		rateOverTime.constant = (float)xpDelta * 1f;
		float constant2 = rateOverTime.constant;
		emission.rateOverTime = rateOverTime;
		initStartTime = Time.time;
	}
}
