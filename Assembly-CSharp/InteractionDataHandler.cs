using System.Collections.Generic;
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
		MVGameControllerBase.CameraController.PlayPlingSound();
		if (interactionIsLocal)
		{
			worldObjectParent.ReceiveInteractionPackage(interaction, null);
		}
		else
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
