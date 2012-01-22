using System;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimation : MonoBehaviour
{
	private AvatarAnimationState state;

	private Dictionary<string, AvatarAnimationState> states;

	private float totalDistance;

	private bool visible;

	private bool isInitialized;

	public void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!isInitialized)
		{
			states = new Dictionary<string, AvatarAnimationState>();
			AvatarAnimationState[] componentsInChildren = ((Component)this).GetComponentsInChildren<AvatarAnimationState>();
			foreach (AvatarAnimationState avatarAnimationState in componentsInChildren)
			{
				states[((Object)avatarAnimationState).name] = avatarAnimationState;
			}
		}
	}

	public void SetState(string name)
	{
		if (!states.ContainsKey(name))
		{
			Debug.LogError((object)("avatar animation does not contain state: " + name));
		}
		state = states[name];
		UpdateVisibility();
	}

	public void Move(float distance)
	{
		totalDistance += distance;
	}

	public void Update()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f / state.rotationFreq;
		((Component)this).transform.localRotation = Quaternion.Euler(0f, state.rotationAmplitude * Mathf.Cos((float)Math.PI * 2f * state.rotationFreq * totalDistance), 0f);
		((Component)this).transform.localPosition = Vector3.up * state.bobbingAmplitude * Mathf.Cos((float)Math.PI * 2f * state.bobbingFreq * totalDistance);
		state.SetActivePoseIndex(Saw(Mathf.FloorToInt((totalDistance - state.rotationPhase) / (num * 0.5f)), state.poses.Length));
	}

	private static int Saw(int a, int b)
	{
		return (a >= 0) ? (a % b) : ((a + 1) % b + b - 1);
	}

	public void SetCloacked(bool isCloaked)
	{
		Debug.LogWarning((object)"SetCloacked is not implemented in new animation system.");
	}

	public void SetVisible(bool visible)
	{
		this.visible = visible;
		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		foreach (AvatarAnimationState value in states.Values)
		{
			value.SetVisible(visible && (Object)(object)value == (Object)(object)state);
		}
	}
}
