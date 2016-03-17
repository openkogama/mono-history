using UnityEngine.EventSystems;
using UnityEngine.Events;

internal interface IHandleCubeModelEdit : IEventSystemHandler
{
	void Open(UnityAction closeCallback);

	void Close();
}
