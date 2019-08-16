namespace UnityEngine.EventSystems;

internal interface IEditModeController : IEventSystemHandler
{
	void DeleteWoid(int woid);

	void DisableEditMode();

	void EnterPlayMode();

	void EnterBuildMode();
}
