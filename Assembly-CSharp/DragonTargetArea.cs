using UnityEngine;

public class DragonTargetArea : MonoBehaviour
{
	private ParticleEmitter[] emitters;

	public bool isAvatarInside;

	public bool isFiring = true;

	public bool IsAvatarInside => isAvatarInside;

	private void Start()
	{
		emitters = ((Component)this).gameObject.GetComponentsInChildren<ParticleEmitter>();
		StopFiring();
		((Component)this).GetComponentInChildren<TriggerBoxEvents>().TriggerEnter += DragonTargetArea_TriggerEnter;
		((Component)this).GetComponentInChildren<TriggerBoxEvents>().TriggerExit += DragonTargetArea_TriggerExit;
	}

	private void OnDestroy()
	{
		if (!((Object)(object)((Component)this).GetComponentInChildren<TriggerBoxEvents>() == (Object)null))
		{
			((Component)this).GetComponentInChildren<TriggerBoxEvents>().TriggerEnter -= DragonTargetArea_TriggerEnter;
			((Component)this).GetComponentInChildren<TriggerBoxEvents>().TriggerExit -= DragonTargetArea_TriggerExit;
		}
	}

	private void Update()
	{
	}

	public void StartFiring()
	{
		isFiring = true;
		if (emitters != null)
		{
			ParticleEmitter[] array = emitters;
			foreach (ParticleEmitter val in array)
			{
				val.emit = true;
			}
		}
	}

	public void StopFiring()
	{
		isFiring = false;
		if (emitters != null)
		{
			ParticleEmitter[] array = emitters;
			foreach (ParticleEmitter val in array)
			{
				val.emit = false;
			}
		}
	}

	private void DragonTargetArea_TriggerEnter(object sender, TriggerEventArgs e)
	{
		isAvatarInside = true;
	}

	private void DragonTargetArea_TriggerExit(object sender, TriggerEventArgs e)
	{
		isAvatarInside = false;
	}
}
