namespace UnityEngine.EventSystems;

public interface ISelectCubeModelingTool : IEventSystemHandler
{
	void Select(CubeModelingEvent tool);
}
