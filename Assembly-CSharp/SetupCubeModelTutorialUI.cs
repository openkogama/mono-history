using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SetupCubeModelTutorialUI : MonoBehaviour, IHandleCubeEditTutorial, IEventSystemHandler
{
	protected CubeModelingStateMachine cubeModelingStateMachine;

	[SerializeField]
	private DesktopCubeModelingControllerCubeTutorial desktopCubeModelTutorialControllerPrefab;

	[SerializeField]
	private MaterialsController materialsController;

	public void Initialize(CubeModelingStateMachine cubeModelingStateMachine)
	{
		this.cubeModelingStateMachine = cubeModelingStateMachine;
	}

	public void PushCubeEditCubeTutorialTools(UnityAction closeAction)
	{
		DesktopCubeModelingControllerCubeTutorial cubeModelTutorialController = Object.Instantiate(desktopCubeModelTutorialControllerPrefab);
		cubeModelTutorialController.Initialize(cubeModelingStateMachine, materialsController);
		cubeModelingStateMachine.CurrentMaterialId = cubeModelingStateMachine.CurrentMaterialId;
		cubeModelTutorialController.SetMaterial(cubeModelingStateMachine.CurrentMaterialId);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.Popup);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(cubeModelTutorialController.gameObject, UIPushOption.HideAllExceptStackBottom, closeAction);
		});
	}
}
