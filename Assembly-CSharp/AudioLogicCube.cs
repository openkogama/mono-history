using UnityEngine;

public class AudioLogicCube : MonoBehaviour
{
	public bool testKeys;

	private AudioOnOffComponent[] audioOnOffComponents;

	private void Awake()
	{
		audioOnOffComponents = ((Component)this).GetComponents<AudioOnOffComponent>();
	}

	private void Update()
	{
		if (testKeys)
		{
			if (Input.GetKeyDown("z"))
			{
				Play(on: true);
			}
			if (Input.GetKeyDown("x"))
			{
				Play(on: false);
			}
		}
	}

	public void Play(bool on)
	{
		if (on)
		{
			AudioOnOffComponent[] array = audioOnOffComponents;
			foreach (AudioOnOffComponent audioOnOffComponent in array)
			{
				audioOnOffComponent.TurnOn();
			}
		}
		else
		{
			AudioOnOffComponent[] array2 = audioOnOffComponents;
			foreach (AudioOnOffComponent audioOnOffComponent2 in array2)
			{
				audioOnOffComponent2.TurnOff();
			}
		}
	}
}
