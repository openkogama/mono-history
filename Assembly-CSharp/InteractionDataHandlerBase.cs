using MV.WorldObject;
using UnityEngine;

public abstract class InteractionDataHandlerBase : MVComponent
{
	[SerializeField]
	private ClosestPointBase closestPoint;

	public abstract MVTeam Team { get; }

	public Vector3 GetClosestPoint(Vector3 from)
	{
		return closestPoint.GetClosestPoint(from);
	}

	public virtual bool CanHandle(InteractionPackageType interactionPackageType, bool interactionIsLocal)
	{
		return true;
	}

	public abstract bool HandleInteraction(MVPickupOwner interactor, InteractionData interaction, bool interactionIsLocal);

	public bool HandleInteraction(InteractionData interaction, bool interactionIsLocal)
	{
		return HandleInteraction(null, interaction, interactionIsLocal);
	}

	protected virtual void OnValidate()
	{
		if (closestPoint == null)
		{
			closestPoint = gameObject.GetComponent<ClosestPointBase>();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (closestPoint == null)
		{
			ClosestPointPoint closestPointPoint = gameObject.AddComponent<ClosestPointPoint>();
			closestPointPoint.Init(transform);
			closestPoint = closestPointPoint;
		}
	}

	protected bool IsFriendlyFire(MVPickupOwner interactor)
	{
		bool result = false;
		if (interactor != null)
		{
			MVTeam teamFromActorNr = MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(interactor.WorldObjectOwner.OwnerActorNr);
			result = teamFromActorNr == Team && MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
		}
		return result;
	}
}
