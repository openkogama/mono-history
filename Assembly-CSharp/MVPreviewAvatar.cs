using System.Collections.Generic;
using UnityEngine;

public class MVPreviewAvatar : MVGroup
{
	public MVPreviewAvatar(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVPreviewAvatarPrefab, worldObjects)
	{
	}

	public override MVWorldObjectClient Clone(int ownerActorNumber, int cloneGroupId, CloneBookkeeping cloneBookkeeping, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
	{
		MVWorldObjectClient mVWorldObjectClient = base.Clone(ownerActorNumber, cloneGroupId, cloneBookkeeping, worldObjects, prototypes);
		if (mVWorldObjectClient.GetType() == typeof(MVAvatarLocal))
		{
			((MVAvatarLocal)mVWorldObjectClient).SpawnId = Id;
		}
		return mVWorldObjectClient;
	}

	public override void Select(Color color)
	{
		AddSelectionBox();
	}

	public override void DeSelect()
	{
		RemoveSelectionBox();
	}
}
