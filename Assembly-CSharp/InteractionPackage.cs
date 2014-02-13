using MV.WorldObject;

public abstract class InteractionPackage
{
	public abstract void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct);
}
