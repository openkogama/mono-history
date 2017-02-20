using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ProximityDamageAndImpulse : InteractionPackage
{
	public static InteractionData Create(float damage, Vector3 impulse, PlayerKilledByType playerKilledByType = PlayerKilledByType.None)
	{
		return new InteractionData(InteractionPackageType.ProximityDamageAndImpulse, damage, impulse, playerKilledByType);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, interactionStruct.PlayerKilledByType, interactionStruct.Impulse);
	}
}
