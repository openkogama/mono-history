using System.Collections;
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
		worldObjectParent.SendPackage(new Hashtable { 
		{
			(byte)0,
			interaction.ToByteArray()
		} });
		return true;
	}
}
