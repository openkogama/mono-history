using UnityEngine;

public class SoundEmitterActiveCheck : MonoBehaviour
{
	private MVGlobalSoundEmitter globalSoundEmitter;

	private MVSoundEmitter soundEmitter;

	private bool initialized;

	public void Initialize(MVSoundEmitter SoundEmitter)
	{
		soundEmitter = SoundEmitter;
		initialized = true;
	}

	public void Initialize(MVGlobalSoundEmitter SoundEmitter)
	{
		globalSoundEmitter = SoundEmitter;
		initialized = true;
	}

	private void Update()
	{
		if (!initialized)
		{
			return;
		}
		if (soundEmitter == null && globalSoundEmitter == null)
		{
			Object.Destroy(this);
		}
		if (soundEmitter != null)
		{
			if (soundEmitter.GameObject.activeInHierarchy)
			{
				soundEmitter.UpdateSound();
				Object.Destroy(this);
			}
		}
		else if (globalSoundEmitter.GameObject.activeInHierarchy)
		{
			globalSoundEmitter.UpdateSound();
			Object.Destroy(this);
		}
	}
}
