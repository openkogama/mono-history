using MV.Common;

public class VehicleInteractable : MVInteractable, IMoveHitHandler
{
	public override void TakeDamage(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (!IgnoreDamage(damageDealer))
		{
			float value = health.Value;
			health.Value -= amount;
			if (health.Value <= 0f && !(value > 0f))
			{
			}
		}
	}

	public override void AddModifier(AvatarModifierPackageType type, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
		if (type != AvatarModifierPackageType.Poison)
		{
			base.AddModifier(type, id, additionalModifers);
		}
	}

	public void HandleMoveHit(MVControllerColliderHit moveHit)
	{
		AddModifier(moveHit.material.ModifierPackageType);
	}
}
