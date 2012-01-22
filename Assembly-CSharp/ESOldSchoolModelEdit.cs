using UnityEngine;

internal class ESOldSchoolModelEdit : ESStateBase
{
	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(ESOldSchoolModelEdit));

	private ConstaintVisualizer constraintVisualizer;

	private MVCubeModelBase targetCubeModel;

	private MVGUIEditModel guiEditModel;

	private bool exitButtonWasPressed;

	private bool isEditingModel;

	private EditModel editModel;

	private LinkObjectScript rightClickedLink;

	public override void Enter(EditorStateMachine e)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected Obj, but got Unknown
		exitButtonWasPressed = false;
		logger.Log("Enter");
		if (e.SingleSelectedWO == null)
		{
			tintedWo = null;
			targetCubeModel = MVGameController.Instance.WOCM.Terrain;
			isEditingModel = false;
		}
		else
		{
			isEditingModel = true;
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
		}
		editModel = new EditModel();
		editModel.TargetCubeModel = targetCubeModel;
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (exitButtonWasPressed)
		{
			if (isEditingModel)
			{
				if (e.ParentGroupIsRoot)
				{
					e.Event = EditorEvent.EditCubes;
				}
				else
				{
					e.Event = EditorEvent.ObjectSelected;
				}
			}
			return;
		}
		if (!isEditingModel)
		{
			TintObjectsOnMouseOver(e);
			if (MVInputWrapper.GetKeyDown((KeyCode)323) && e.Select(addToSelection: false))
			{
				e.Event = EditorEvent.ObjectSelected;
				return;
			}
		}
		editModel.ExecuteCubeEditing(e);
		if (isEditingModel)
		{
			constraintVisualizer.UpdatePosition(targetCubeModel);
		}
		if (!targetCubeModel.CubesLeft() && isEditingModel)
		{
			Debug.LogWarning((object)"This prototype is empty and should be deleted");
			e.Event = EditorEvent.EditCubes;
		}
		if (!MVGameController.Instance.EditorController.IsLogicRendered())
		{
			return;
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)324))
		{
			Ray val = ((Component)e.WeCamera).camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit val2 = default;
			Physics.Raycast(val, ref val2, 100f);
			if ((Object)(object)val2.collider != (Object)null)
			{
				LinkObjectScript componentInChildren = ((Component)val2.collider).gameObject.GetComponentInChildren<LinkObjectScript>();
				if ((Object)(object)componentInChildren != (Object)null)
				{
					rightClickedLink = componentInChildren;
				}
			}
		}
		if (!MVInputWrapper.GetKeyUp((KeyCode)324) || !((Object)(object)rightClickedLink != (Object)null))
		{
			return;
		}
		Ray val3 = ((Component)e.WeCamera).camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit val4 = default;
		Physics.Raycast(val3, ref val4, 100f);
		if ((Object)(object)val4.collider != (Object)null)
		{
			LinkObjectScript componentInChildren2 = ((Component)val4.collider).gameObject.GetComponentInChildren<LinkObjectScript>();
			if ((Object)(object)componentInChildren2 != (Object)null && (Object)(object)componentInChildren2 == (Object)(object)rightClickedLink)
			{
				MVGameController.Instance.Game.RemoveLink(MVGameController.Instance.WOCM.Links[componentInChildren2.linkID]);
			}
		}
		rightClickedLink = null;
	}

	public override void Exit(EditorStateMachine e)
	{
		if (isEditingModel)
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
		}
		editModel.Destroy();
		editModel = null;
	}
}
