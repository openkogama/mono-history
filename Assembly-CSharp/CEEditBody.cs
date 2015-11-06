using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class CEEditBody : ESStateBase
{
	private ConstraintVisualizer constraintVisualizer;

	private MVCubeModelInstance targetCubeModel;

	private IWorldObjectWithModelingConstraint modelBody;

	private MVGUIEditModel guiEditModel;

	private bool exitButtonWasPressed;

	private MVWorldObjectClientManager WOCM => MVGameControllerBase.WOCM;

	private CharacterEditorController CharacterEditorController => MVGameControllerLegacyUI.CharacterEditorController;

	public override void Enter(EditorStateMachine esm)
	{
		base.Enter(esm);
		MVGameControllerBase.CameraController.SetCamera(CameraType.EditorCamera);
		MVGameControllerBase.CameraController.CurCamera.FocusOnObject(esm.SingleSelectedWO);
		CharacterEditorController.ShowEditorTools();
		CharacterEditorController.HideAnimationToggles();
		CharacterEditorController.HideAvatarTools();
		exitButtonWasPressed = false;
		tintedWo = null;
		targetCubeModel = (MVCubeModelInstance)esm.SingleSelectedWO;
		modelBody = (IWorldObjectWithModelingConstraint)WOCM.GetWorldObjectClient(targetCubeModel.GroupId);
		guiEditModel = UXUtils.FindGUIObjectOfType<MVGUIEditModel>();
		guiEditModel.View.Show();
		guiEditModel.exitButton.OnClick = () =>
		{
			exitButtonWasPressed = true;
		};
		guiEditModel.exitText.OnClick = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			exitButtonWasPressed = true;
		};
		UXMouseClickObject exitText = guiEditModel.exitText;
		exitText.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(exitText.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
		if (!esm.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(esm.ParentGroup.Transform, select: false);
		}
		SharedCubeFunctions.SetLayerRecursively(WOCM.AvatarLocal.Transform, select: false);
		SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: true);
		esm.CameraController.BlueModeEnabled = true;
		CharacterEditorController.DrawPlaneController.DrawPlaneToModel(targetCubeModel.GameObject);
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
		if (exitButtonWasPressed || targetCubeModel == null || targetCubeModel.State == MVWorldObjectState.Destroyed)
		{
			esm.Event = EditorEvent.CERoam;
		}
		else
		{
			esm.CubeModelingStateMachine.Update();
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
		base.Exit(esm);
		CharacterEditorController.DrawPlaneController.ReturnDrawPlaneToLandscape();
		if (CharacterEditorController.DrawPlaneController.IsDrawPlaneActive)
		{
			CharacterEditorController.DrawPlaneController.ToggleDrawPlane();
		}
		UnityEngine.Object.Destroy(constraintVisualizer.gameObject);
		CharacterEditorController.HideEditorTools();
		if (!esm.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(WOCM.GetWorldObjectClient(esm.ParentGroupID).Transform, select: true);
		}
		else
		{
			SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: false);
			esm.CameraController.BlueModeEnabled = false;
		}
		guiEditModel.View.Hide();
		guiEditModel.exitButton.OnClick = null;
		WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		esm.DeSelectAll();
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit);
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(1f, 1f);
		esm.CubeModelingStateMachine.RemoveCursors();
		esm.CubeModelingStateMachine.EndEdit();
		CharacterEditorController.AvatarSlotButtonView.UpdateAvatarSlotButtons();
	}
}
