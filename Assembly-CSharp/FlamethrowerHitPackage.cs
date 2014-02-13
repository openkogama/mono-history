using MV.WorldObject;
using UnityEngine;

public class FlamethrowerHitPackage : InteractionPackage
{
	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.FlamethrowerHit);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if ((Object)(object)component != (Object)null)
		{
			component.AddModifier(AvatarModifierPackageType.FlamerBurn, shooter.ActorNr);
		}
	}
}
