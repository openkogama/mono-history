using MV.WorldObject;

public class MouseGunHitPackage : InteractionPackage
{
	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.MouseGunHit);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component != null)
		{
			component.AddModifier(AvatarModifierPackageType.Shrunken);
		}
	}
}
