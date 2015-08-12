using System.Collections.Generic;

public class EditorStateMachine : FSMEntity
{
	public const float sqrEpsilon = 0.64f;

	private MVNetworkSelector networkSelector;

	private MVCameraController weCamera;

	private CubeModelingStateMachine editModel = new CubeModelingStateMachine();

	private SelectionController selectionController;

	public MVNetworkSelector NetworkSelector => networkSelector;

	public MVCameraController CameraController => weCamera;

	public bool GridMode { get; set; }

	public CubeModelingStateMachine CubeModelingStateMachine => editModel;

	public EditorEvent CurEvent => (EditorEvent)(int)curEvent;

	public EditorEvent PrevEvent => (EditorEvent)(int)prevEvent;

	public EditorEvent NextEvent => (EditorEvent)(int)nextEvent;

	public ISelectionController SelectionController => selectionController;

	public HashSet<int> SelectedIDs => selectionController.SelectedIDs;

	public HashSet<MVWorldObjectClient> SelectedWOs => selectionController.SelectedWOs;

	public MVWorldObjectClient SingleSelectedWO => selectionController.SingleSelectedWO;

	public int ParentGroupID => selectionController.ParentGroupID;

	public MVGroup ParentGroup => selectionController.ParentGroup;

	public bool ParentGroupIsRoot => selectionController.ParentGroupID == MVGameController.WOCM.RootGroup.Id;

	public EditorStateMachine()
	{
		transitionTable = new EditorStateTransitionTable();
		networkSelector = new MVNetworkSelector(this);
		selectionController = new SelectionController();
		weCamera = MVGameController.Game.CameraController;
		GridMode = true;
	}

	public void EnterGroup(MVGroup group)
	{
		selectionController.EnterGroup(group);
	}

	public int ExitGroup()
	{
		return selectionController.ExitGroup();
	}

	public int ExitGroupToRoot()
	{
		return selectionController.ExitGroupToRoot();
	}

	public MVWorldObjectClient Select(bool addToSelection, int layerMask = -5)
	{
		return selectionController.Select(addToSelection, showVisuals: true, layerMask);
	}

	public MVWorldObjectClient Select(VoxelHit hit, bool addToSelection)
	{
		return selectionController.Select(hit, addToSelection);
	}

	public void DeSelectWorldObject(MVWorldObjectClient wo)
	{
		selectionController.DeSelectWorldObject(wo);
	}

	public MVWorldObjectClient SelectWO(int id, bool addToSelection, bool showVisuals = true)
	{
		return selectionController.SelectWO(id, addToSelection, showVisuals);
	}

	public void DeSelectAll()
	{
		selectionController.DeSelectAll();
	}

	public void DeSelectAllExcept(int id)
	{
		selectionController.DeSelectAllExcept(id);
	}

	public bool IsSelected(int id)
	{
		return selectionController.IsSelected(id);
	}

	public override void Update()
	{
		base.Update();
	}
}
