using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;

public class InteractionDataHandler : InteractionDataHandlerBase
{
	private readonly HashSet<PlayerKilledByType> disregardPlingList = new HashSet<PlayerKilledByType>
	{
		PlayerKilledByType.Ghost,
		PlayerKilledByType.None,
		PlayerKilledByType.Suicide,
		PlayerKilledByType.AdvancedGhost,
		PlayerKilledByType.Environmental,
		PlayerKilledByType.Explosive,
		PlayerKilledByType.Impact,
		PlayerKilledByType.Fire,
		PlayerKilledByType.FallOffWorld,
		PlayerKilledByType.Crushed
	};

	public MVWorldObjectClient WorldObjectParent
	{
		set
		{
			worldObjectParent = value;
		}
	}

	public override bool HandleInteraction(InteractionData interaction, bool interactionIsLocal)
	{
		if (!disregardPlingList.Contains(interaction.PlayerKilledByType))
		{
			MVGameControllerBase.CameraController.PlayPlingSound();
		}
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
