using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MaterialsControllerEditMode : MaterialsController, IEventSystemHandler, IHandleCubeModelEdit
{
	private DesktopCubeModelingController desktopCubeModelingControllerEditMode;

	[SerializeField]
	private CreateCubeModelController createCubeModelController;

	private UnityAction closeCallback;

	[SerializeField]
	private DesktopCubeModelingController desktopCubeModelingControllerEditCubeModelPrefab;

	private byte prevMaterial;

	public static byte targetMaterial;

	private void OnEnable()
	{
		if (prevMaterial != targetMaterial)
		{
			OnMaterialChanged(targetMaterial);
		}
	}

	public override Transform SetActive()
	{
		Transform transform = base.SetActive();
		createCubeModelController.Initialize(this, transform, cubeModelingStateMachine.CurrentMaterialId);
		return transform;
	}

	public void Open(UnityAction closeCallback)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyRegister x, BaseEventData y) =>
		{
			x.RegisterShortcutKey(KogamaControls.TogglePlayInEditor, KeyState.Up, PlayModeToggleOverwrite);
		});
		desktopCubeModelingController.gameObject.SetActive(value: false);
		this.closeCallback = closeCallback;
		desktopCubeModelingControllerEditMode = Object.Instantiate(desktopCubeModelingControllerEditCubeModelPrefab);
		desktopCubeModelingControllerEditMode.Initialize(cubeModelingStateMachine);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI | UIGroupFlags.GameObjectUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(desktopCubeModelingControllerEditMode.gameObject, UIPushOption.None, OnPop);
		});
		cubeModelingStateMachine.CurrentMaterialId = cubeModelingStateMachine.CurrentMaterialId;
	}

	public override void OnMaterialChanged(byte id)
	{
		prevMaterial = id;
		base.OnMaterialChanged(id);
		if (desktopCubeModelingControllerEditMode != null)
		{
			desktopCubeModelingControllerEditMode.SetMaterial(id);
		}
	}

	public void Close()
	{
		closeCallback();
	}

	private void PlayModeToggleOverwrite()
	{
		Debug.Log("Ignoring");
	}

	private void OnPop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyUnRegister x, BaseEventData y) =>
		{
			x.UnRegisterShortcutKey(KogamaControls.TogglePlayInEditor, KeyState.Up);
		});
		desktopCubeModelingController.gameObject.SetActive(value: true);
		desktopCubeModelingControllerEditMode = null;
	}
}
