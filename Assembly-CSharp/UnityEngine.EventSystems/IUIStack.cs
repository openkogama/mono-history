using UnityEngine.Events;

namespace UnityEngine.EventSystems;

public interface IUIStack : IEventSystemHandler
{
	bool StackReady { get; }

	void Push(GameObject gameObject, UIPushOption pushOption, UnityAction onPop = null, UIGroupFlags group = UIGroupFlags.Default);

	void Pop();

	void PopGroups(UIGroupFlags popGroups);

	void PopToGroup(UIGroupFlags group);

	bool PopToStackElement(GameObject gameObject);

	bool IsUIElementBlocked(GameObject gameObject);

	GameObject Peak();
}
