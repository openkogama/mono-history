using UnityEngine;

internal class ESSingleModelEdit : ESStateBase
{
	private ConstaintVisualizer constraintVisualizer;

	private MVCubeModelBase targetCubeModel;

	private MVGUIEditModel guiEditModel;

	private bool exitButtonWasPressed;

	private EditModel editModel;

	public override void Enter(EditorStateMachine e)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected Obj, but got Unknown
		exitButtonWasPressed = false;
		tintedWo = null;
		exitButtonWasPressed = false;
		targetCubeModel = (MVCubeModelBase)e.SingleSelectedWO;
		e.DeSelect();
		guiEditModel = Object.FindObjectOfType(typeof(MVGUIEditModel)) as MVGUIEditModel;
		guiEditModel.View.Show();
		guiEditModel.exitButton.OnClick = () =>
		{
			exitButtonWasPressed = true;
		};
		GameObject val = new GameObject("constrainVisualizer");
		constraintVisualizer = val.AddComponent<ConstaintVisualizer>();
		constraintVisualizer.Init(targetCubeModel);
		if (!e.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroup).GameObject.transform, select: false);
		}
		SharedCubeFunctions.SetLayerRecursively(targetCubeModel.GameObject.transform, select: true);
		((Behaviour)((Component)e.WeCamera).GetComponent<GrayscaleEffect>()).enabled = true;
		e.WeCamera.SecondaryCameraActive = true;
		MVGameController.Instance.EditorController.PushDrawPlane(targetCubeModel.GameObject);
		MVGameController.Instance.EditorController.WorldEditorDrawPlane.SetToLowerBound();
		editModel = new EditModel();
		editModel.TargetCubeModel = targetCubeModel;
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (MVInputWrapper.GetKeyDown((KeyCode)27) || exitButtonWasPressed)
		{
			e.Event = EditorEvent.ObjectSelected;
			return;
		}
		editModel.ExecuteCubeEditing(e);
		constraintVisualizer.UpdatePosition(targetCubeModel);
		if (!targetCubeModel.CubesLeft())
		{
			Debug.LogWarning((object)"This prototype is empty and should be deleted");
			e.Event = EditorEvent.ObjectSelected;
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		MVGameController.Instance.EditorController.PopDrawPlane();
		Object.Destroy((Object)(object)((Component)constraintVisualizer).gameObject);
		if (!e.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroup).GameObject.transform, select: true);
		}
		else
		{
			SharedCubeFunctions.SetLayerRecursively(targetCubeModel.GameObject.transform, select: false);
			((Behaviour)((Component)e.WeCamera).GetComponent<GrayscaleEffect>()).enabled = false;
			e.WeCamera.SecondaryCameraActive = false;
		}
		guiEditModel.View.Hide();
		guiEditModel.exitButton.OnClick = null;
		editModel.Destroy();
		editModel = null;
	}
}
