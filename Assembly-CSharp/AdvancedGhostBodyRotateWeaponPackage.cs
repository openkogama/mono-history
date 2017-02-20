using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class AdvancedGhostBodyRotateWeaponPackage : InteractionPackage
{
	public static InteractionData Create(float damage, Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.AdvancedGhostBodyRotateWeaponPackage, damage, impulse);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.AdvancedGhost, interactionStruct.Impulse);
	}
}
