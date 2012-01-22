namespace MV.WorldObject;

public class OldLink
{
	public int worldObjectID;

	public OldLinkType type;

	public OldLink(OldLinkType type, int worldObjectID)
	{
		this.type = type;
		this.worldObjectID = worldObjectID;
	}
}
