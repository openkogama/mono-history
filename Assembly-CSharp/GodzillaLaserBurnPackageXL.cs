using MV.WorldObject;

public class GodzillaLaserBurnPackageXL : GodzillaLaserBurnPackage
{
	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component != null)
		{
			component.AddModifier(AvatarModifierPackageType.GodzillaLaserBurnXL, shooter.ActorNr);
		}
	}
}
