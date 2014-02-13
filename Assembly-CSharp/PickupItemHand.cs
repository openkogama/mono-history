using MV.Common;
using UnityEngine;

public class PickupItemHand : PickupItemWithDelay
{
	public ParticleSystem hitParticles;

	public float pushMagnitude = 500f;

	public float pushRadius = 3f;

	public override AvatarItemType Type => AvatarItemType.Hand;

	public override bool ActivateGunModeOnEquip => false;

	public override int Quantity => 0;

	protected override void OnFire(bool isLocal)
	{
	}
}
