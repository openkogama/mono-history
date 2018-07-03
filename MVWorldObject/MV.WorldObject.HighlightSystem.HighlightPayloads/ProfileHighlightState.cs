using System.Collections.Generic;

namespace MV.WorldObject.HighlightSystem.HighlightPayloads;

public class ProfileHighlightState
{
	public Dictionary<int, int> slotSeenIdMap = new Dictionary<int, int>();

	public void SetHighlightToSeen(int slot, int id)
	{
		if (slotSeenIdMap.ContainsKey(slot))
		{
			slotSeenIdMap[slot] = id;
		}
		else
		{
			slotSeenIdMap.Add(slot, id);
		}
	}
}
