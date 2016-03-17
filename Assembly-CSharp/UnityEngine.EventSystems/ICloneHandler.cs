namespace UnityEngine.EventSystems;

public interface ICloneHandler : IEventSystemHandler
{
	void Clone(MVWorldObjectClient original, bool cloneToRoot, bool setAsPreviewItem, bool goToInsert = false);
}
