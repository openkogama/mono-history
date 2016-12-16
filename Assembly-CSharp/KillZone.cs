using MV.Common;
using UnityEngine;

[RequireComponent(typeof(TriggerBoxEvents))]
public class KillZone : MonoBehaviour
{
	[SerializeField]
	private TriggerBoxEvents[] triggerBoxEvents;

	[SerializeField]
	private bool dontKillBelow;

	[SerializeField]
	private float belowMargin;

	private void Awake()
	{
		for (int i = 0; i < triggerBoxEvents.Length; i++)
		{
			triggerBoxEvents[i].TriggerEnter += OnEnter;
		}
	}

	private bool AllowedToKill(MVWorldObjectClient obj)
	{
		return !dontKillBelow || obj.Position.y + belowMargin >= transform.position.y;
	}

	private bool IsImmune(MVInteractableBase i)
	{
		if (i == null)
		{
			return false;
		}
		return i.HasModifier(AvatarModifierPackageType.GodzillaS) || i.HasModifier(AvatarModifierPackageType.GodzillaM) || i.HasModifier(AvatarModifierPackageType.GodzillaL) || i.HasModifier(AvatarModifierPackageType.GodzillaXL);
	}

	private void OnEnter(object sender, TriggerEventArgs e)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID);
		if (worldObjectClient != null && AllowedToKill(worldObjectClient))
		{
			InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
			MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
			if (interactionDataHandlerBase != null && !IsImmune(component))
			{
				interactionDataHandlerBase.HandleInteraction(ProximityDamageAndImpulse.Create(float.PositiveInfinity, Vector3.zero, PlayerKilledByType.KillZone), interactionIsLocal: true);
			}
		}
	}
}
