using System.Collections.Generic;
using MV.WorldObject;

internal class ClientSideLogicInteractionHandler : InteractionDataHandlerBase
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
		worldObjectParent.SendPackage(new Dictionary<object, object> { 
		{
			(byte)0,
			interaction.ToByteArray()
		} });
		return true;
	}
}
