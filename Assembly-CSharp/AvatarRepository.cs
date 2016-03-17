using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvatarRepository
{
	private readonly Dictionary<int, AvatarRepositoryItem> avatars = new Dictionary<int, AvatarRepositoryItem>();

	public int Count => avatars.Count;

	public void AddItem(AvatarRepositoryItem item)
	{
		if (avatars.ContainsKey(item.slotPosition))
		{
			Debug.LogError("Avatar with slotPosition: " + item.slotPosition + " already exists in AvatarRepository");
		}
		else
		{
			avatars.Add(item.slotPosition, item);
		}
	}

	public AvatarRepositoryItem GetAvatar(int slotPosition)
	{
		return avatars[slotPosition];
	}

	public List<AvatarRepositoryItem> GetAvatars()
	{
		return avatars.Values.OrderBy((AvatarRepositoryItem x) => x.slotPosition).ToList();
	}
}
