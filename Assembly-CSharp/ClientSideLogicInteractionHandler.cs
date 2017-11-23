using System.Collections.Generic;
using MV.WorldObject;

internal class ClientSideLogicInteractionHandler : InteractionDataHandlerBase
{
	public override MVTeam Team => MVTeam.Server;

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
		if (CanHandle(interaction.InteractionType, interactionIsLocal))
		{
			worldObjectParent.SendPackage(new Dictionary<object, object> { 
			{
				(byte)0,
				interaction.ToByteArray()
			} });
			return true;
		}
		return false;
	}
}
