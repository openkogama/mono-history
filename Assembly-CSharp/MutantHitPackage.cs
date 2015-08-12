using MV.Common;
using MV.WorldObject;

public class MutantHitPackage : InteractionPackage
{
	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.MutantHit);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component != null)
		{
			component.TakeDamage(interactionStruct.Damage, shooter, PlayerKilledByType.Mutant);
		}
	}
}
