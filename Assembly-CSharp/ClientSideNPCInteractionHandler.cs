using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class ClientSideNPCInteractionHandler : InteractionDataHandlerBase
{
	[SerializeField]
	private GameObject attachmentObjectForHealRay;

	private readonly HashSet<InteractionPackageType> unableToDamageNPCs = new HashSet<InteractionPackageType>
	{
		InteractionPackageType.FlamethrowerHit,
		InteractionPackageType.HealRayHit
	};

	private readonly HashSet<InteractionPackageType> friendlyInteractions = new HashSet<InteractionPackageType> { InteractionPackageType.HealRayHit };

	private MVTeam team = MVTeam.None;

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
			MVGameControllerBase.MainCameraManager.PlayPlingSound();
			MVGameControllerBase.PlayModeUI.GetCrossHair().ShowHasHitEffect();
		}
		else if (friendlyInteractions.Contains(interaction.InteractionType))
		{
			worldObjectParent.SendPackage(new Dictionary<object, object> { 
			{
				(byte)0,
				interaction.ToByteArray()
			} });
		}
		return true;
	}

	public GameObject GetHealRayAttachmentObject()
	{
		return attachmentObjectForHealRay;
	}
}
