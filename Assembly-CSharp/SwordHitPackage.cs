using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class SwordHitPackage : InteractionPackage
{
	private const PlayerKilledByType killedByType = PlayerKilledByType.Sword;

	public static InteractionData Create(Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.SwordHit, impulse, PlayerKilledByType.Sword);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.Sword, interactionStruct.Impulse, AvatarModifierPackageType.NoFriction);
	}
}
