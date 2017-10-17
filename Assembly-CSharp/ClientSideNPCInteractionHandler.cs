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

	private MVTeam team = MVTeam.Server;

	public override MVTeam Team => team;

	public void SetTeam(MVTeam team)
	{
		this.team = team;
	}

	public override bool CanHandle(InteractionPackageType interactionPackageType, bool interactionIsLocal)
	{
		if (interactionIsLocal)
		{
			return false;
		}
		return true;
	}

	public override bool HandleInteraction(MVPickupOwner interactor, InteractionData interaction, bool interactionIsLocal)
	{
		if (!CanHandle(interaction.InteractionType, interactionIsLocal))
		{
			return false;
		}
		if (!IsFriendlyFire(interactor) && !unableToDamageNPCs.Contains(interaction.InteractionType))
		{
			worldObjectParent.SendPackage(new Dictionary<object, object> { 
			{
				(byte)0,
				interaction.ToByteArray()
			} });
			MVGameControllerBase.CameraController.PlayPlingSound();
			MVGameControllerBase.IPlayModeUI.GetCrossHair().ShowHasHitEffect();
		}
		return true;
	}
}
