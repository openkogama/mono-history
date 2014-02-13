using UnityEngine;

public class ObjectParticleEmitterScript : MonoBehaviour
{
	public ParticleSystem particleSystemPrefab;

	private ParticleSystem particleSystemInstance;

	public void Play()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected Obj, but got Unknown
		if ((Object)(object)particleSystemPrefab != (Object)null)
		{
			particleSystemInstance = (ParticleSystem)Object.Instantiate((Object)(object)particleSystemPrefab, ((Component)this).transform.position, ((Component)this).transform.rotation);
			if ((Object)(object)particleSystemInstance != (Object)null)
			{
				particleSystemInstance.Play();
			}
			else
			{
				Debug.LogError((object)"ParticleSystemInstance is null");
			}
		}
		else
		{
			Debug.LogError((object)"ParticleSystemPrefab is null");
		}
	}
}
