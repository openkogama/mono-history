using System.Collections.Generic;

public class CloneBookkeeping
{
	public int cloneIdIncrement = -1;

	public int cloneLinkIdIncrement = -1;

	public int cloneObjectLinkIdIncrement = -1;

	public List<int> linkIds = new List<int>();

	public List<int> objectLinkIds = new List<int>();

	public Dictionary<int, int> worldObjectIdsMaps = new Dictionary<int, int>();
}
