using UnityEngine;
using UnityEngine.EventSystems;

public class CEEditBodyUUI : ESStateBase
{
	private ConstraintVisualizer constraintVisualizer;

	private IWorldObjectWithModelingConstraint modelBody;

	private int targetCubeModelId = -1;

	private MVCubeModelInstance TargetCubeModel
	{
		get
		{
			if (targetCubeModelId == -1)
			{
				return null;
			}
			return (MVCubeModelInstance)MVGameControllerBase.WOCM.GetWorldObjectClient(targetCubeModelId);
		}
		set
		{
			if (value != null)
			{
				targetCubeModelId = value.Id;
			}
			else
			{
				targetCubeModelId = -1;
			}
		}
	}

	public override void Enter(EditorStateMachine esm)
	{
		base.Enter(esm);
		MVGameControllerBase.CameraController.SetCamera(CameraType.AvatarEditModeCamera);
		MVGameControllerBase.CameraController.CurCamera.FocusOnObject(esm.SingleSelectedWO);
		ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IAvatarEditUIState x, BaseEventData y) =>
		{
			x.Set(ActiveEditStateUI.CubeModelTools);
		});
		tintedWo = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
		TargetCubeModel = (MVCubeModelInstance)esm.SingleSelectedWO;
		modelBody = (IWorldObjectWithModelingConstraint)MVGameControllerBase.WOCM.GetWorldObjectClient(TargetCubeModel.GroupId);
		if (!esm.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(esm.ParentGroup.Transform, select: false);
		}
		SharedCubeFunctions.SetLayerRecursively(MVGameControllerBase.WOCM.AvatarLocal.Transform, select: false);
		SharedCubeFunctions.SetLayerRecursively(TargetCubeModel.Transform, select: true);
		esm.CameraController.BlueModeEnabled = true;
		DrawPlane.DrawPlaneToModel(TargetCubeModel.GameObject);
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(0.25f, 0.25f);
		IModelingConstraint modelConstaint = modelBody.GetModelConstaint(TargetCubeModel);
		GameObject gameObject = new GameObject("constrainVisualizer");
		constraintVisualizer = gameObject.AddComponent<ConstraintVisualizer>();
		constraintVisualizer.Init(TargetCubeModel, modelConstaint);
		esm.CubeModelingStateMachine.StartEdit(TargetCubeModel, modelConstaint);
	}

	public override void Execute(EditorStateMachine esm)
	{
		base.Execute(esm);
		if (TargetCubeModel == null)
		{
			esm.Event = EditorEvent.CERoamUUI;
		}
		else
		{
			esm.CubeModelingStateMachine.Update();
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
		base.Exit(esm);
		DrawPlane.ReturnDrawPlaneToLandscape();
		if (DrawPlane.IsDrawPlaneActive)
		{
			DrawPlane.ToggleDrawPlane();
		}
		Object.Destroy(constraintVisualizer.gameObject);
		if (!esm.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(MVGameControllerBase.WOCM.GetWorldObjectClient(esm.ParentGroupID).Transform, select: true);
		}
		else
		{
			SharedCubeFunctions.SetLayerRecursively(TargetCubeModel.Transform, select: false);
		}
		esm.CameraController.BlueModeEnabled = false;
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		esm.DeSelectAll();
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit);
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(1f, 1f);
		esm.CubeModelingStateMachine.RemoveCursors();
		esm.CubeModelingStateMachine.EndEdit();
	}
}
