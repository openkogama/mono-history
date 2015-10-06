using MV.WorldObject;

public class GrowthGunHitPackage : InteractionPackage
{
	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.GrowthGunHit);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component != null)
		{
			component.AddModifier(AvatarModifierPackageType.Enlarged);
		}
	}
}
