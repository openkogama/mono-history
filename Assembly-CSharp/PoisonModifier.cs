using UnityEngine;

public class PoisonModifier : AvatarModifier
{
	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Poison;

	protected override void OnActivated(Avatar target)
	{
	}

	protected override void OnDeactivated(Avatar target)
	{
		Object.Destroy(gameObject);
	}
}
