using System;
using MV.WorldObject;
using UnityEngine;

internal class ESCubeEdit : ESStateBase
{
	private IModelingConstraint constraint;

	private ConstraintVisualizer constraintVisualizer;

	private MVCubeModelBase targetCubeModel;

	private JetPackMode jetPackMode;

	private MVGUIEditModel guiEditModel;

	private bool exitButtonWasPressed;

	private MVWorldObjectClientManager WOCM => MVGameController.WOCM;

	public override void Enter(EditorStateMachine e)
	{
		Debug.Log("ESCubeEdit enter");
		HandleUnavailableMaterial(e);
		exitButtonWasPressed = false;
		if (e.SingleSelectedWO == null)
		{
			Debug.LogError("ESCubeEdit must not be entered with no selected WorldObject");
			e.PopState();
			return;
		}
		tintedWo = null;
		exitButtonWasPressed = false;
		targetCubeModel = (MVCubeModelBase)e.SingleSelectedWO;
		e.DeSelectAll();
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
		constraint = targetCubeModel.ModelingConstraintBuilder();
		GameObject gameObject = new GameObject("ConstrainVisualizer");
		constraintVisualizer = gameObject.AddComponent<ConstraintVisualizer>();
		constraintVisualizer.Init(targetCubeModel, constraint);
		if (targetCubeModel.HasInteractionFlag(InteractionFlags.IsPreview))
		{
			targetCubeModel.RemovePreviewBox();
		}
		if (!e.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(MVGameController.WOCM.GetWorldObjectClient(e.ParentGroupID).Transform, select: false);
		}
		SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: true);
		e.CameraController.GetComponent<GrayscaleEffect>().enabled = true;
		e.CameraController.SecondaryCameraActive = true;
		MVGameController.EditorController.CubeModelingController.DrawPlaneToModel(targetCubeModel.GameObject);
		if (jetPackMode == null)
		{
			jetPackMode = WOCM.AvatarLocal.AvatarModes.JetPackMode;
		}
		jetPackMode.YMovementSpeedScale = Mathf.Min(1f, 2f * targetCubeModel.Scale.x);
		jetPackMode.XZMovementSpeedScale = Mathf.Min(1f, 2f * targetCubeModel.Scale.x);
		e.CubeModelingStateMachine.StartEdit(targetCubeModel, constraint);
		MVGameController.EditorController.EnterCubeModelEdit();
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (exitButtonWasPressed || targetCubeModel == null || targetCubeModel.State == MVWorldObjectState.Destroyed)
		{
			Debug.Log("Exit cube edit, parentGroup: " + e.ParentGroup);
			if (e.ParentGroupIsRoot)
			{
				Debug.Log("Parent group is root!");
				e.Event = EditorEvent.ESTerrainEdit;
				return;
			}
			if (e.ParentGroup == null)
			{
				Debug.LogError("ParentGroup was null");
				e.ExitGroupToRoot();
				return;
			}
			MVGroup mVGroup = e.ParentGroup;
			while (!mVGroup.OnExitObject(e) && mVGroup.Group != null)
			{
				Debug.Log("Looping up tree");
				mVGroup = mVGroup.Group;
			}
		}
		e.CubeModelingStateMachine.Update();
		if (!targetCubeModel.ContainsCubes)
		{
			Debug.LogWarning("This prototype is empty and should be deleted");
			e.Event = EditorEvent.ESTerrainEdit;
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		Debug.Log("ESCubeEdit exit");
		if (targetCubeModel.GameObject == null)
		{
			MVGameController.EditController.CubeModelingController.CreateDrawPlane();
			e.CameraController.GetComponent<GrayscaleEffect>().enabled = false;
			e.CameraController.SecondaryCameraActive = false;
		}
		else
		{
			MVGameController.EditController.CubeModelingController.ReturnDrawPlaneToLandscape();
			if (targetCubeModel.HasInteractionFlag(InteractionFlags.IsPreview))
			{
				targetCubeModel.AddPreviewBox();
			}
			SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: false);
			e.CameraController.GetComponent<GrayscaleEffect>().enabled = false;
			e.CameraController.SecondaryCameraActive = false;
		}
		if (constraintVisualizer != null)
		{
			UnityEngine.Object.Destroy(constraintVisualizer.gameObject);
		}
		if (constraint is ModelingDynamicBoxConstraint modelingDynamicBoxConstraint)
		{
			modelingDynamicBoxConstraint.DetachFromCubeModel();
		}
		constraint = null;
		guiEditModel.View.Hide();
		guiEditModel.exitButton.OnClick = null;
		MVGameController.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		if (e.SelectedIDs.Count == 0)
		{
			DeTintCurrent();
		}
		jetPackMode.YMovementSpeedScale = 1f;
		jetPackMode.XZMovementSpeedScale = 1f;
		e.CubeModelingStateMachine.RemoveCursors();
		e.CubeModelingStateMachine.EndEdit();
		MVGameController.EditorController.LeaveCubeModelEdit();
	}

	private void HandleUnavailableMaterial(EditorStateMachine e)
	{
		MVMaterial material = MVGameController.Game.MaterialRepository.GetMaterial(e.CubeModelingStateMachine.CurrentMaterialId);
		if (!material.IsAvailable)
		{
			Debug.LogWarning("Handle if default material is not available!");
			e.CubeModelingStateMachine.CurrentMaterialId = 21;
		}
	}
}
