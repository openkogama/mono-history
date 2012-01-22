using System;
using UnityEngine;

[Serializable]
public class FeetClipsAndLevels
{
	public AudioClip leftClip;

	public AudioClip rightClip;

	public float minPitchLeft = 0.8f;

	public float maxPitchLeft = 0.9f;

	public float minPitchRight = 1f;

	public float maxPitchRight = 1.1f;

	public float minVolumeLeft = 0.7f;

	public float maxVolumeLeft = 0.8f;

	public float minVolumeRight = 0.9f;

	public float maxVolumeRight = 1f;
}
