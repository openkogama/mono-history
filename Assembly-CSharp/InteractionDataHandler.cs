using System.Collections;
using MV.WorldObject;

public class InteractionDataHandler : InteractionDataHandlerBase
{
	public MVWorldObjectClient WorldObjectParent
	{
		set
		{
			worldObjectParent = value;
		}
	}

	public override bool HandleInteraction(InteractionData interaction, bool interactionIsLocal)
	{
		if (interactionIsLocal)
		{
			worldObjectParent.ReceiveInteractionPackage(interaction, null);
		}
		else
		{
			worldObjectParent.SendPackage(new Hashtable { 
			{
				(byte)0,
				interaction.ToByteArray()
			} });
		}
		return true;
	}
}
