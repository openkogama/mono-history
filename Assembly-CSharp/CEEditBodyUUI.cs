using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class CEEditBodyUUI : ESStateBase
{
	private ConstraintVisualizer constraintVisualizer;

	private MVCubeModelInstance targetCubeModel;

	private IWorldObjectWithModelingConstraint modelBody;

	public override void Enter(EditorStateMachine esm)
	{
		base.Enter(esm);
		MVGameControllerBase.CameraController.SetCamera(CameraType.EditorCamera);
		MVGameControllerBase.CameraController.CurCamera.FocusOnObject(esm.SingleSelectedWO);
		ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IAvatarEditUIState x, BaseEventData y) =>
		{
			x.Set(ActiveEditStateUI.CubeModelTools);
		});
		tintedWo = null;
		targetCubeModel = (MVCubeModelInstance)esm.SingleSelectedWO;
		modelBody = (IWorldObjectWithModelingConstraint)MVGameControllerBase.WOCM.GetWorldObjectClient(targetCubeModel.GroupId);
		if (!esm.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(esm.ParentGroup.Transform, select: false);
		}
		SharedCubeFunctions.SetLayerRecursively(MVGameControllerBase.WOCM.AvatarLocal.Transform, select: false);
		SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: true);
		esm.CameraController.BlueModeEnabled = true;
		DrawPlane.DrawPlaneToModel(targetCubeModel.GameObject);
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(0.25f, 0.25f);
		IModelingConstraint modelConstaint = modelBody.GetModelConstaint(targetCubeModel);
		GameObject gameObject = new GameObject("constrainVisualizer");
		constraintVisualizer = gameObject.AddComponent<ConstraintVisualizer>();
		constraintVisualizer.Init(targetCubeModel, modelConstaint);
		esm.CubeModelingStateMachine.StartEdit(targetCubeModel, modelConstaint);
	}

	public override void Execute(EditorStateMachine esm)
	{
		base.Execute(esm);
		if (targetCubeModel == null || targetCubeModel.State == MVWorldObjectState.Destroyed)
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
			SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: false);
			esm.CameraController.BlueModeEnabled = false;
		}
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		esm.DeSelectAll();
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit);
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(1f, 1f);
		esm.CubeModelingStateMachine.RemoveCursors();
		esm.CubeModelingStateMachine.EndEdit();
		Debug.LogWarning("Reimplement");
	}
}
