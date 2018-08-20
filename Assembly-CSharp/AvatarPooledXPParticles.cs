using UnityEngine;

public class AvatarPooledXPParticles : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem xpParticleSystem;

	private void Start()
	{
		xpParticleSystem.Play();
	}

	private void Update()
	{
		if (!xpParticleSystem.isPlaying)
		{
			PrefabPool.Instance.EnumPoolManager.Return(this, PoolEnums.XP);
		}
	}

	public void Initialize(int xpDelta)
	{
		ParticleSystem.EmissionModule emission = xpParticleSystem.emission;
		ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
		rateOverTime.constant = xpDelta;
		emission.rateOverTime = rateOverTime;
	}

	public void Play()
	{
		GetComponent<ParticleSystem>().Play();
	}
}
