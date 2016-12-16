using MV.WorldObject;
using UnityEngine;

public abstract class InteractionDataHandlerBase : MVComponent
{
	[SerializeField]
	private ClosestPointBase closestPoint;

	public Vector3 GetClosestPoint(Vector3 from)
	{
		return closestPoint.GetClosestPoint(from);
	}

	public virtual bool CanHandle(InteractionPackageType interactionPackageType, bool interactionIsLocal)
	{
		return true;
	}

	public abstract bool HandleInteraction(InteractionData interaction, bool interactionIsLocal);

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
			Debug.LogWarning(string.Empty + gameObject.name + ": InteractionDataHandler is missing a reference to ClosestPoint.\nDefaulting to ClosestPointPoint. (Reason is probably that AddComponent<InteractionDataHandler>() is used.)");
			ClosestPointPoint closestPointPoint = gameObject.AddComponent<ClosestPointPoint>();
			closestPointPoint.Init(transform);
			closestPoint = closestPointPoint;
		}
	}
}
