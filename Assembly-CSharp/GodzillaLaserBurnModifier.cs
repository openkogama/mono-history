using UnityEngine;

public class GodzillaLaserBurnModifier : AvatarModifier
{
	[HideInInspector]
	[SerializeField]
	private AvatarModifierPackageType modifierType;

	public override AvatarModifierPackageType ModifierType => modifierType;

	public void SetType(AvatarModifierPackageType a)
	{
		modifierType = a;
	}

	protected override void OnDeactivated(Avatar target)
	{
		Object.Destroy(gameObject);
	}
}
