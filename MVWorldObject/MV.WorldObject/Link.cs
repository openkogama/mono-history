namespace MV.WorldObject;

public class Link
{
	public int id = -1;

	public int outputWOID = -1;

	public int inputWOID = -1;

	public bool isSet = false;

	public Link(int id, int outputWOID, int inputWOID, bool isSet)
	{
		this.id = id;
		this.outputWOID = outputWOID;
		this.inputWOID = inputWOID;
		this.isSet = isSet;
	}

	public Link()
	{
	}
}
