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
		SharedCubeFunctions.SetLayerRecursively(TargetCubeModel.Transform, select: true);
		DrawPlane.DrawPlaneToModel(TargetCubeModel.GameObject);
		IModelingConstraint modelConstaint = modelBody.GetModelConstaint(TargetCubeModel);
		GameObject gameObject = new GameObject("constrainVisualizer");
		constraintVisualizer = gameObject.AddComponent<ConstraintVisualizer>();
		constraintVisualizer.Init(TargetCubeModel, modelConstaint);
		esm.CubeModelingStateMachine.StartEdit(TargetCubeModel, modelConstaint);
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.EnterBuildStateEvent(stateType, new MVBuildModeAvatarLocal.EditMode.CEEditBodyUUIData(esm.SingleSelectedWO.Id));
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
		esm.DeSelectAll();
		esm.CubeModelingStateMachine.RemoveCursors();
		esm.CubeModelingStateMachine.EndEdit();
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.ExitBuildStateEvent(stateType, null);
	}
}
