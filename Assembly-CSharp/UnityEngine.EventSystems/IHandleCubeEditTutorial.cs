using UnityEngine.Events;

namespace UnityEngine.EventSystems;

public interface IHandleCubeEditTutorial : IEventSystemHandler
{
	void PushCubeEditCubeTutorialTools(UnityAction closeAction);
}
