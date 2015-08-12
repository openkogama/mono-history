using UnityEngine;

public class DragonTargetArea : MonoBehaviour
{
	private ParticleEmitter[] emitters;

	public bool isAvatarInside;

	public bool isFiring = true;

	public bool IsAvatarInside => isAvatarInside;

	private void Start()
	{
		emitters = gameObject.GetComponentsInChildren<ParticleEmitter>();
		StopFiring();
		GetComponentInChildren<TriggerBoxEvents>().TriggerEnter += DragonTargetArea_TriggerEnter;
		GetComponentInChildren<TriggerBoxEvents>().TriggerExit += DragonTargetArea_TriggerExit;
	}

	private void OnDestroy()
	{
		if (!(GetComponentInChildren<TriggerBoxEvents>() == null))
		{
			GetComponentInChildren<TriggerBoxEvents>().TriggerEnter -= DragonTargetArea_TriggerEnter;
			GetComponentInChildren<TriggerBoxEvents>().TriggerExit -= DragonTargetArea_TriggerExit;
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
			foreach (ParticleEmitter particleEmitter in array)
			{
				particleEmitter.emit = true;
			}
		}
	}

	public void StopFiring()
	{
		isFiring = false;
		if (emitters != null)
		{
			ParticleEmitter[] array = emitters;
			foreach (ParticleEmitter particleEmitter in array)
			{
				particleEmitter.emit = false;
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
