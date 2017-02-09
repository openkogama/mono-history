using System.Collections.Generic;
using MV.WorldObject;

public class ClientSideNPCInteractionHandler : InteractionDataHandlerBase
{
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
		MVGameControllerBase.CameraController.PlayPlingSound();
		MVGameControllerBase.IPlayModeUI.GetCrossHair().ShowHasHitEffect();
		worldObjectParent.SendPackage(new Dictionary<object, object> { 
		{
			(byte)0,
			interaction.ToByteArray()
		} });
		return true;
	}
}
