using System;

namespace UnityEngine.EventSystems;

public interface IGetCurrentBody : IEventSystemHandler
{
	void GetCurrentBody(Action<MVBody> callback);
}
