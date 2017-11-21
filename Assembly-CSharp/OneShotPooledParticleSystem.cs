using UnityEngine;

public class OneShotPooledParticleSystem : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particleSystem;

	private PoolEnums type;

	public static ParticleSystem Instantiate(PoolEnums type)
	{
		OneShotPooledParticleSystem oneShotPooledParticleSystem = PrefabPool.Instance.EnumPoolManager.Instantiate<OneShotPooledParticleSystem>(type);
		oneShotPooledParticleSystem.type = type;
		return oneShotPooledParticleSystem.particleSystem;
	}

	public static ParticleSystem Instantiate(PoolEnums type, Vector3 position, Quaternion rotation)
	{
		OneShotPooledParticleSystem oneShotPooledParticleSystem = PrefabPool.Instance.EnumPoolManager.Instantiate<OneShotPooledParticleSystem>(type);
		oneShotPooledParticleSystem.type = type;
		oneShotPooledParticleSystem.transform.localPosition = position;
		oneShotPooledParticleSystem.transform.localRotation = rotation;
		return oneShotPooledParticleSystem.particleSystem;
	}

	private void OnValidate()
	{
		if (particleSystem == null)
		{
			particleSystem = GetComponent<ParticleSystem>();
		}
	}

	private void Update()
	{
		if (particleSystem.time >= particleSystem.duration)
		{
			PrefabPool.Instance.EnumPoolManager.Return(this, type);
		}
	}
}
