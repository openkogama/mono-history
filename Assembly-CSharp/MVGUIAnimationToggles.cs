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
		if (!((Object)(object)attachedAnimation == (Object)null))
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected Obj, but got Unknown
		attachedAnimation = animation;
		foreach (AnimationState item in ((Component)attachedAnimation).animation)
		{
			AnimationState val = item;
			val.wrapMode = (WrapMode)2;
		}
	}

	public void DetachAnimation()
	{
		attachedAnimation = null;
	}
}
