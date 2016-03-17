using UnityEngine.Events;

namespace UnityEngine.EventSystems;

public interface IUIStack : IEventSystemHandler
{
	void Push(GameObject gameObject, UIPushOption pushOption, UnityAction onPop = null, UIGroupFlags group = UIGroupFlags.Default);

	void Pop();

	void PopGroups(UIGroupFlags popGroups);

	void PopToBottom();
}
