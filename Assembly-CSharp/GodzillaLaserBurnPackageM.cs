using MV.WorldObject;

public class GodzillaLaserBurnPackageM : GodzillaLaserBurnPackage
{
	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component != null)
		{
			component.AddModifier(AvatarModifierPackageType.GodzillaLaserBurnM, shooter.ActorNr);
		}
	}
}
