namespace MV.WorldObject;

public class ObjectLink
{
	public int id = -1;

	public int objectConnectorWOID = -1;

	public int objectWOID = -1;

	public bool isSet = true;

	public ObjectLink(int id, int objectConnectorWOID, int objectWOID, bool isSet)
	{
		this.id = id;
		this.objectConnectorWOID = objectConnectorWOID;
		this.objectWOID = objectWOID;
		this.isSet = isSet;
	}

	public ObjectLink()
	{
	}
}
