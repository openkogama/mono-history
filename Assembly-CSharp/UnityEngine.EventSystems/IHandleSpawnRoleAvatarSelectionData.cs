using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.EventSystems;

public interface IHandleSpawnRoleAvatarSelectionData : IEventSystemHandler
{
	void TryGetSpawnRoleAvatarSelectionData(UnityAction<List<SpawnRoleAvatarSelectionData>> onDataReady);
}
