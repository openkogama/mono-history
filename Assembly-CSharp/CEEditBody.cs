using System;
using MV.WorldObject;
using UnityEngine;

public class CEEditBody : ESStateBase
{
	private ConstraintVisualizer constraintVisualizer;

	private MVCubeModelInstance targetCubeModel;

	private IWorldObjectWithModelingConstraint modelBody;

	private JetPackMode jetPackMode;

	private MVGUIEditModel guiEditModel;

	private bool exitButtonWasPressed;

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	private AEditController EditController => MVGameController.Instance.EditController;

	private CharacterEditorController CharacterEditorController => MVGameController.Instance.CharacterEditorController;

	public override void Enter(EditorStateMachine esm)
	{
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Expected Obj, but got Unknown
		base.Enter(esm);
		MVGameController.Instance.Game.CameraController.SetCamera(CameraType.JetPackCamera);
		MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(esm.SingleSelectedWO);
		EditController.ShowEditorTools(cubeEditMode: true);
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
		((Behaviour)((Component)esm.CameraController).GetComponent<GrayscaleEffect>()).enabled = true;
		esm.CameraController.SecondaryCameraActive = true;
		EditController.DrawPlaneToModel(targetCubeModel.GameObject);
		if (jetPackMode == null)
		{
			jetPackMode = WOCM.AvatarLocal.AvatarModes.JetPackMode;
		}
		jetPackMode.YMovementSpeedScale = 0.25f;
		jetPackMode.XZMovementSpeedScale = 0.25f;
		IModelingConstraint modelConstaint = modelBody.GetModelConstaint(targetCubeModel);
		GameObject val = new GameObject("constrainVisualizer");
		constraintVisualizer = val.AddComponent<ConstraintVisualizer>();
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
		EditController.ReturnDrawPlaneToLandscape();
		if (EditController.IsDrawPlaneActive)
		{
			EditController.ToggleDrawPlane();
		}
		Object.Destroy((Object)(object)((Component)constraintVisualizer).gameObject);
		if (!esm.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(WOCM.GetWorldObjectClient(esm.ParentGroupID).Transform, select: true);
		}
		else
		{
			SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: false);
			((Behaviour)((Component)esm.CameraController).GetComponent<GrayscaleEffect>()).enabled = false;
			esm.CameraController.SecondaryCameraActive = false;
		}
		guiEditModel.View.Hide();
		guiEditModel.exitButton.OnClick = null;
		WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		esm.DeSelectAll();
		jetPackMode.YMovementSpeedScale = 1f;
		jetPackMode.XZMovementSpeedScale = 1f;
		esm.CubeModelingStateMachine.RemoveCursors();
		esm.CubeModelingStateMachine.EndEdit();
		CharacterEditorController.AvatarSlotButtonView.UpdateAvatarSlotButtons();
	}
}
