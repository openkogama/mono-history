using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIAnimationToggles : UXViewScript
{
	public UXIconButton animationToggle;

	public List<string> animationsToToggle;

	private int currentAnimationIndex;

	private BoneAnimation attachedAnimation;

	public override void OnInitialize()
	{
		UXIconButton uXIconButton = animationToggle;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			ToggleNextAnimation();
		}));
	}

	private void ToggleNextAnimation()
	{
		if (!(attachedAnimation == null))
		{
			currentAnimationIndex++;
			currentAnimationIndex %= animationsToToggle.Count;
			ToggleAnimation(animationsToToggle[currentAnimationIndex]);
		}
	}

	public void ToggleAnimation(string animationName)
	{
		attachedAnimation.Play(animationName);
	}

	public void AttachAnimation(BoneAnimation animation)
	{
		attachedAnimation = animation;
		foreach (AnimationState item in attachedAnimation.GetComponent<Animation>())
		{
			item.wrapMode = WrapMode.Loop;
		}
	}

	public void DetachAnimation()
	{
		attachedAnimation = null;
	}
}
