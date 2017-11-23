using UnityEngine;

public class HitParticle : MonoBehaviour
{
	[SerializeField]
	private ParticleEmitter pEmitter;

	[SerializeField]
	private PoolEnums type;

	private float timer;

	public void Initialize()
	{
		pEmitter.emit = true;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= pEmitter.maxEnergy)
		{
			pEmitter.emit = false;
			pEmitter.ClearParticles();
			timer = 0f;
			PrefabPool.Instance.EnumPoolManager.Return(this, type);
		}
	}
}
