using MV.WorldObject;

public abstract class InteractionDataHandlerBase : MVComponent
{
	public virtual bool CanHandle(InteractionPackageType interactionPackageType, bool interactionIsLocal)
	{
		return true;
	}

	public abstract bool HandleInteraction(InteractionData interaction, bool interactionIsLocal);
}
