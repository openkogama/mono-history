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

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	public override void Enter(EditorStateMachine e)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected Obj, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"ESCubeEdit enter");
		exitButtonWasPressed = false;
		if (e.SingleSelectedWO == null)
		{
			Debug.LogError((object)"ESCubeEdit must not be entered with no selected WorldObject");
			e.PopState();
			return;
		}
		tintedWo = null;
		exitButtonWasPressed = false;
		targetCubeModel = (MVCubeModelBase)e.SingleSelectedWO;
		e.DeSelectAll();
		guiEditModel = Object.FindObjectOfType(typeof(MVGUIEditModel)) as MVGUIEditModel;
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
		GameObject val = new GameObject("ConstrainVisualizer");
		constraintVisualizer = val.AddComponent<ConstraintVisualizer>();
		constraintVisualizer.Init(targetCubeModel, constraint);
		if (targetCubeModel.HasInteractionFlag(InteractionFlags.IsPreview))
		{
			targetCubeModel.RemovePreviewBox();
		}
		if (!e.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroupID).Transform, select: false);
		}
		SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: true);
		((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = true;
		e.CameraController.SecondaryCameraActive = true;
		MVGameController.Instance.EditController.DrawPlaneToModel(targetCubeModel.GameObject);
		if (jetPackMode == null)
		{
			jetPackMode = WOCM.AvatarLocal.AvatarModes.JetPackMode;
		}
		jetPackMode.YMovementSpeedScale = Mathf.Min(1f, 2f * targetCubeModel.Scale.x);
		jetPackMode.XZMovementSpeedScale = Mathf.Min(1f, 2f * targetCubeModel.Scale.x);
		e.CubeModelingStateMachine.StartEdit(targetCubeModel, constraint);
		MVGameController.Instance.EditController.ShowEditorTools(cubeEditMode: true);
	}

	private void Constraint_BoxChanged(object sender, CubeModelChangedEventArgs e)
	{
		targetCubeModel.AddPreviewBox();
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (exitButtonWasPressed || targetCubeModel == null || targetCubeModel.State == MVWorldObjectState.Destroyed)
		{
			Debug.Log((object)("Exit cube edit, parentGroup: " + e.ParentGroup));
			if (e.ParentGroupIsRoot)
			{
				Debug.Log((object)"Parent group is root!");
				e.Event = EditorEvent.ESTerrainEdit;
				return;
			}
			if (e.ParentGroup == null)
			{
				Debug.LogError((object)"ParentGroup was null");
				e.ExitGroupToRoot();
				return;
			}
			MVGroup mVGroup = e.ParentGroup;
			while (!mVGroup.OnExitObject(e) && mVGroup.Group != null)
			{
				Debug.Log((object)"Looping up tree");
				mVGroup = mVGroup.Group;
			}
		}
		e.CubeModelingStateMachine.Update();
		if (!targetCubeModel.ContainsCubes)
		{
			Debug.LogWarning((object)"This prototype is empty and should be deleted");
			e.Event = EditorEvent.ESTerrainEdit;
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		Debug.Log((object)"ESCubeEdit exit");
		if ((Object)(object)targetCubeModel.GameObject == (Object)null)
		{
			MVGameController.Instance.EditController.CreateDrawPlane();
			((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = false;
			e.CameraController.SecondaryCameraActive = false;
		}
		else
		{
			MVGameController.Instance.EditController.ReturnDrawPlaneToLandscape();
			if (targetCubeModel.HasInteractionFlag(InteractionFlags.IsPreview))
			{
				targetCubeModel.AddPreviewBox();
			}
			SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: false);
			((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = false;
			e.CameraController.SecondaryCameraActive = false;
		}
		if ((Object)(object)constraintVisualizer != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)constraintVisualizer).gameObject);
		}
		if (constraint is ModelingDynamicBoxConstraint modelingDynamicBoxConstraint)
		{
			modelingDynamicBoxConstraint.DetachFromCubeModel();
		}
		constraint = null;
		guiEditModel.View.Hide();
		guiEditModel.exitButton.OnClick = null;
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		if (e.SelectedIDs.Count == 0)
		{
			DeTintCurrent();
		}
		jetPackMode.YMovementSpeedScale = 1f;
		jetPackMode.XZMovementSpeedScale = 1f;
		e.CubeModelingStateMachine.RemoveCursors();
		e.CubeModelingStateMachine.EndEdit();
		MVGameController.Instance.EditController.ShowEditorTools();
	}
}
