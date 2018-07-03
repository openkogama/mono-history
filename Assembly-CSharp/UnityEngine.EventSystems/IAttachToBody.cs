namespace UnityEngine.EventSystems;

public interface IAttachToBody : IEventSystemHandler
{
	void AttachToBody(int streamingAssetId, float offset, float scale);
}
