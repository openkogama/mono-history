using UnityEngine;

public class KogamaAudioScript_Feet : MonoBehaviour
{
	private AudioSource[] leftFootAudioSources;

	private AudioSource[] rightFootAudioSources;

	public FeetClipsAndLevels[] feetClipsAndLevels;

	private bool leftFeet;

	private void Awake()
	{
		leftFootAudioSources = new AudioSource[feetClipsAndLevels.Length];
		rightFootAudioSources = new AudioSource[feetClipsAndLevels.Length];
		for (int i = 0; i < feetClipsAndLevels.Length; i++)
		{
			leftFootAudioSources[i] = gameObject.AddComponent<AudioSource>();
			leftFootAudioSources[i].clip = feetClipsAndLevels[i].leftClip;
			leftFootAudioSources[i].playOnAwake = false;
			rightFootAudioSources[i] = gameObject.AddComponent<AudioSource>();
			if (feetClipsAndLevels[i].rightClip == null)
			{
				rightFootAudioSources[i].clip = feetClipsAndLevels[i].leftClip;
			}
			else
			{
				rightFootAudioSources[i].clip = feetClipsAndLevels[i].rightClip;
			}
			rightFootAudioSources[i].playOnAwake = false;
		}
	}

	private void Update()
	{
		if (MVInputWrapper.DebugGetKeyDown("1"))
		{
			PlayFeetSound();
		}
	}

	private void PlayFeetSound()
	{
		for (int i = 0; i < feetClipsAndLevels.Length; i++)
		{
			if (leftFeet)
			{
				leftFootAudioSources[i].pitch = Random.Range(feetClipsAndLevels[i].minPitchLeft, feetClipsAndLevels[i].maxPitchLeft);
				leftFootAudioSources[i].volume = Random.Range(feetClipsAndLevels[i].minVolumeLeft, feetClipsAndLevels[i].maxVolumeLeft);
				leftFootAudioSources[i].Play();
			}
			else
			{
				rightFootAudioSources[i].pitch = Random.Range(feetClipsAndLevels[i].minPitchRight, feetClipsAndLevels[i].maxPitchRight);
				rightFootAudioSources[i].volume = Random.Range(feetClipsAndLevels[i].minVolumeRight, feetClipsAndLevels[i].maxVolumeRight);
				rightFootAudioSources[i].Play();
			}
		}
		if (leftFeet)
		{
			leftFeet = false;
		}
		else
		{
			leftFeet = true;
		}
	}
}
