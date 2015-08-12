using System;
using UnityEngine;

public class KeyFrameCallback : MonoBehaviour
{
	[SerializeField]
	private string keyFrameName;

	[SerializeField]
	private int keyFrameIndex;

	public Action callbacks;

	private bool fired;

	public string KeyFrameName => keyFrameName;

	public int KeyFrameIndex => keyFrameIndex;

	public void Reset()
	{
		fired = false;
	}

	public bool Evaluate(float timePassedSincePlay, AnimationCurve animationCurve)
	{
		if (fired)
		{
			return false;
		}
		if (timePassedSincePlay >= animationCurve.keys[KeyFrameIndex].time)
		{
			fired = true;
			if (callbacks != null)
			{
				callbacks();
			}
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{keyFrameIndex} {keyFrameName}";
	}
}
