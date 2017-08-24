using MV.WorldObject.MetaData;

namespace UnityEngine.EventSystems;

public interface IFirstTimeElementActivator : IEventSystemHandler
{
	void RegisterActivatableElement(IActivatableFirstTimeUiElement firstTimeEventHandlerListener);

	void UnRegisterActivatableElement(IActivatableFirstTimeUiElement firstTimeEventHandlerListener);

	void RequestEvaluateActivatableElements();

	void SkipFirstTimeEvent(FirstTimeEvent firstTimeEvent, FirstTimeActivatableElementBase firstTimeActivatable);
}
