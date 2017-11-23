using System.Collections.Generic;
using MV.WorldObject;

public class InteractionDataHandler : InteractionDataHandlerBase
{
	public override MVTeam Team => MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(worldObjectParent.OwnerActorNr);

	public MVWorldObjectClient WorldObjectParent
	{
		set
		{
			worldObjectParent = value;
		}
	}

	public override bool HandleInteraction(MVPickupOwner interactor, InteractionData interaction, bool interactionIsLocal)
	{
		if (interactionIsLocal)
		{
			worldObjectParent.ReceiveInteractionPackage(interaction, null);
		}
		else if (!IsFriendlyFire(interactor))
		{
			worldObjectParent.SendPackage(new Dictionary<object, object> { 
			{
				(byte)0,
				interaction.ToByteArray()
			} });
		}
		return true;
	}
}
