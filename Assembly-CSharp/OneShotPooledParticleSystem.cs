using UnityEngine;

public class OneShotPooledParticleSystem : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particles;

	private PoolEnums type;

	public static ParticleSystem Instantiate(PoolEnums type)
	{
		OneShotPooledParticleSystem oneShotPooledParticleSystem = PrefabPool.Instance.EnumPoolManager.Instantiate<OneShotPooledParticleSystem>(type);
		oneShotPooledParticleSystem.type = type;
		return oneShotPooledParticleSystem.particles;
	}

	public static ParticleSystem Instantiate(PoolEnums type, Vector3 position, Quaternion rotation)
	{
		OneShotPooledParticleSystem oneShotPooledParticleSystem = PrefabPool.Instance.EnumPoolManager.Instantiate<OneShotPooledParticleSystem>(type);
		oneShotPooledParticleSystem.type = type;
		oneShotPooledParticleSystem.transform.localPosition = position;
		oneShotPooledParticleSystem.transform.localRotation = rotation;
		return oneShotPooledParticleSystem.particles;
	}

	private void OnValidate()
	{
		if (particles == null)
		{
			particles = GetComponent<ParticleSystem>();
		}
	}

	private void Update()
	{
		if (particles.time >= particles.main.duration)
		{
			PrefabPool.Instance.EnumPoolManager.Return(this, type);
		}
	}
}
