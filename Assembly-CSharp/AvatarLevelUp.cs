using System;
using UnityEngine;
using UnityEngine.Events;

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
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(ownerActorNr);
		playerUnsafe.OnLevelChanged = (UnityAction<int>)Delegate.Combine(playerUnsafe.OnLevelChanged, new UnityAction<int>(OnLevelChanged));
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
		if (MVGameControllerBase.Game != null && MVGameControllerBase.Game.MVPlayerContainer.ContainsKey(ownerActorNr))
		{
			MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(ownerActorNr);
			playerUnsafe.OnLevelChanged = (UnityAction<int>)Delegate.Remove(playerUnsafe.OnLevelChanged, new UnityAction<int>(OnLevelChanged));
		}
		if (this.scaleAnimation != null)
		{
			ScaleAnimation scaleAnimation = this.scaleAnimation;
			scaleAnimation.OnScaleAnimationStopped = (ScaleAnimationBase.OnScaleAnimationStoppedDelegate)Delegate.Remove(scaleAnimation.OnScaleAnimationStopped, new ScaleAnimationBase.OnScaleAnimationStoppedDelegate(OnScaleAnimationStopped));
		}
	}
}
