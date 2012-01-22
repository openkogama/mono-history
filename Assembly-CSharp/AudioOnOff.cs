using UnityEngine;

public class AudioOnOff : AudioOnOffComponent
{
	public AudioClip offClip;

	public float offMinPitch = 0.8f;

	public float offMaxPitch = 1.1f;

	public float offMinVol = 0.7f;

	public float offMaxVol = 0.9f;

	public override void TurnOff()
	{
		if ((Object)(object)offClip != (Object)null)
		{
			PlayClip(onOffAudioSource, offClip, loop, offMinPitch, offMaxPitch, offMinVol, offMaxVol);
		}
	}
}
