using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

internal class ESCubeEdit : ESStateBase
{
	private IModelingConstraint constraint;

	private ConstraintVisualizer constraintVisualizer;

	private MVCubeModelBase targetCubeModel;

	private bool exiting;

	public override void Enter(EditorStateMachine e)
	{
		Debug.Log("ESCubeEdit enter");
		HandleUnavailableMaterial(e);
		exiting = false;
		if (e.SingleSelectedWO == null)
		{
			Debug.LogError("ESCubeEdit must not be entered with no selected WorldObject");
			e.PopState();
			return;
		}
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit);
		}
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(Mathf.Min(1f, 2f * e.SingleSelectedWO.Scale.x), Mathf.Min(1f, 2f * e.SingleSelectedWO.Scale.x));
		DrawPlane.HideDrawPlane();
		ExecuteEvents.ExecuteHierarchy(e.GameObject, null, (IHandleCubeModelEdit handler, BaseEventData data) =>
		{
			handler.Open(Exit);
		});
		tintedWo = null;
		targetCubeModel = (MVCubeModelBase)e.SingleSelectedWO;
		e.DeSelectAll();
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
			SharedCubeFunctions.SetLayerRecursively(MVGameControllerBase.WOCM.GetWorldObjectClient(e.ParentGroupID).Transform, select: false);
		}
		SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: true);
		DrawPlane.DrawPlaneToModel(targetCubeModel.GameObject);
		e.CubeModelingStateMachine.StartEdit(targetCubeModel, constraint);
		MVGameControllerBase.CameraController.CurCamera.FocusOnObject(targetCubeModel);
		e.CameraController.BlueModeEnabled = true;
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		exiting = exiting || targetCubeModel == null || targetCubeModel.State == MVWorldObjectState.Destroyed;
		if (exiting)
		{
			Debug.Log("Exit cube edit, parentGroup: " + e.ParentGroup);
			if (e.ParentGroupIsRoot)
			{
				Debug.Log("Parent group is root!");
				e.Event = EditorEvent.ESTerrainEdit;
			}
			else if (e.ParentGroup != null)
			{
				MVGroup mVGroup = e.ParentGroup;
				while (!mVGroup.OnExitObject(e) && mVGroup.Group != null)
				{
					Debug.Log("Looping up tree");
					mVGroup = mVGroup.Group;
				}
			}
			else
			{
				Debug.LogError("ParentGroup was null");
				e.ExitGroupToRoot();
			}
		}
		else
		{
			e.CubeModelingStateMachine.Update();
			if (!targetCubeModel.ContainsCubes)
			{
				Debug.LogWarning("This prototype is empty and should be deleted");
				e.Event = EditorEvent.ESTerrainEdit;
			}
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		Debug.Log("ESCubeEdit exit");
		DrawPlane.HideDrawPlane();
		ExecuteEvents.ExecuteHierarchy(e.GameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopToBottom();
		});
		if (targetCubeModel.GameObject == null)
		{
			Debug.LogWarning("Implement: Create drawplane");
			e.CameraController.BlueModeEnabled = false;
		}
		else
		{
			if (targetCubeModel.HasInteractionFlag(InteractionFlags.IsPreview))
			{
				targetCubeModel.AddPreviewBox();
			}
			SharedCubeFunctions.SetLayerRecursively(targetCubeModel.Transform, select: false);
			e.CameraController.BlueModeEnabled = false;
		}
		DrawPlane.ReturnDrawPlaneToLandscape();
		if (constraintVisualizer != null)
		{
			Object.Destroy(constraintVisualizer.gameObject);
		}
		if (constraint is ModelingDynamicBoxConstraint modelingDynamicBoxConstraint)
		{
			modelingDynamicBoxConstraint.DetachFromCubeModel();
		}
		constraint = null;
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		if (e.SelectedIDs.Count == 0)
		{
			DeTintCurrent();
		}
		e.CubeModelingStateMachine.RemoveCursors();
		e.CubeModelingStateMachine.EndEdit();
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(1f, 1f);
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit2D);
			DrawPlane.SetToTerrain(active: true);
		}
	}

	private void Exit()
	{
		exiting = true;
	}

	private void HandleUnavailableMaterial(EditorStateMachine e)
	{
		MVMaterial material = MVGameControllerBase.Game.MaterialRepository.GetMaterial(e.CubeModelingStateMachine.CurrentMaterialId);
		if (!material.IsAvailable)
		{
			Debug.LogWarning("Handle if default material is not available!");
			e.CubeModelingStateMachine.CurrentMaterialId = 21;
		}
	}
}
