using System.Collections.Generic;

namespace MV.WorldObject;

public class KogamaDataPackage
{
	public int rootIdWo = -1;

	public Dictionary<int, MVPrototype> prototypes = new Dictionary<int, MVPrototype>();

	public Dictionary<int, MVWorldObject> worldObjects = new Dictionary<int, MVWorldObject>();

	public Dictionary<int, Link> links = new Dictionary<int, Link>();

	public Dictionary<int, ObjectLink> objectLinks = new Dictionary<int, ObjectLink>();

	public void SetKogamaPackageToProfileID(int profileID)
	{
		foreach (KeyValuePair<int, MVPrototype> prototype in prototypes)
		{
			prototype.Value.InsertedInWorldByProfileID = profileID;
		}
	}

	public override string ToString()
	{
		return "Prototypes " + prototypes.Count + " WorldObjects " + worldObjects.Count + " Links " + links.Count + " ObjectLinks " + objectLinks.Count + " rootIdWo " + rootIdWo;
	}
}
