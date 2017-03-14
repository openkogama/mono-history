using System.Collections.Generic;
using MV.WorldObject;

public class ClientSideNPCInteractionHandler : InteractionDataHandlerBase
{
	private readonly HashSet<InteractionPackageType> unableToDamageNPCs = new HashSet<InteractionPackageType>
	{
		InteractionPackageType.FlamethrowerHit,
		InteractionPackageType.GodzillaLaserBurnS,
		InteractionPackageType.GodzillaLaserBurnM,
		InteractionPackageType.GodzillaLaserBurnL,
		InteractionPackageType.GodzillaLaserBurnXL
	};

	public override bool CanHandle(InteractionPackageType interactionPackageType, bool interactionIsLocal)
	{
		if (interactionIsLocal)
		{
			return false;
		}
		return true;
	}

	public override bool HandleInteraction(InteractionData interaction, bool interactionIsLocal)
	{
		if (!CanHandle(interaction.InteractionType, interactionIsLocal))
		{
			return false;
		}
		worldObjectParent.SendPackage(new Dictionary<object, object> { 
		{
			(byte)0,
			interaction.ToByteArray()
		} });
		if (!unableToDamageNPCs.Contains(interaction.InteractionType))
		{
			MVGameControllerBase.CameraController.PlayPlingSound();
			MVGameControllerBase.IPlayModeUI.GetCrossHair().ShowHasHitEffect();
		}
		return true;
	}
}
