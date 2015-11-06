using System;
using UnityEngine;

public class AvatarLevelUp : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem onLevelUpParticleSystem;

	[SerializeField]
	private TextMesh levelText;

	[SerializeField]
	private ScaleAnimation scaleAnimation;

	private int ownerActorNr;

	public void Init(int ownerActorNr)
	{
		this.ownerActorNr = ownerActorNr;
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[ownerActorNr];
		mVPlayer.OnLevelChanged = (MVPlayer.OnLevelChangedDelegate)Delegate.Combine(mVPlayer.OnLevelChanged, new MVPlayer.OnLevelChangedDelegate(OnLevelChanged));
		ScaleAnimation scaleAnimation = this.scaleAnimation;
		scaleAnimation.OnScaleAnimationStopped = (ScaleAnimationBase.OnScaleAnimationStoppedDelegate)Delegate.Combine(scaleAnimation.OnScaleAnimationStopped, new ScaleAnimationBase.OnScaleAnimationStoppedDelegate(OnScaleAnimationStopped));
	}

	private void OnScaleAnimationStopped(float extraTime)
	{
		levelText.gameObject.layer = LayerMask.NameToLayer("Hidden");
	}

	private void OnLevelChanged(int level)
	{
		onLevelUpParticleSystem.gameObject.SetActive(value: true);
		onLevelUpParticleSystem.Play(withChildren: true);
		scaleAnimation.Play();
		levelText.gameObject.layer = LayerMask.NameToLayer("Default");
		levelText.text = TM._("LEVEL " + level);
		scaleAnimation.Play();
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game.Players.ContainsKey(ownerActorNr))
		{
			MVPlayer mVPlayer = MVGameControllerBase.Game.Players[ownerActorNr];
			mVPlayer.OnLevelChanged = (MVPlayer.OnLevelChangedDelegate)Delegate.Remove(mVPlayer.OnLevelChanged, new MVPlayer.OnLevelChangedDelegate(OnLevelChanged));
		}
		if (this.scaleAnimation != null)
		{
			ScaleAnimation scaleAnimation = this.scaleAnimation;
			scaleAnimation.OnScaleAnimationStopped = (ScaleAnimationBase.OnScaleAnimationStoppedDelegate)Delegate.Remove(scaleAnimation.OnScaleAnimationStopped, new ScaleAnimationBase.OnScaleAnimationStoppedDelegate(OnScaleAnimationStopped));
		}
	}
}
