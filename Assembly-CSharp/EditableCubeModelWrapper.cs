using MV.WorldObject;

public class EditableCubeModelWrapper
{
	private MVCubeModelBase cubeModelBase;

	public MVCubeModelBase CubeModel => cubeModelBase;

	public EditableCubeModelWrapper(MVCubeModelBase cubeModelBase)
	{
		this.cubeModelBase = cubeModelBase;
		cubeModelBase.InteractionFlags |= InteractionFlags.SelectionRequiresEditGroup;
	}

	public EditableCubeModelWrapper(MVCubeModelBase cubeModelBase, IntVector min, IntVector max, int minCubeCount)
		: this(cubeModelBase)
	{
		SetConstraints(min, max, minCubeCount);
	}

	public virtual bool OnEnterObject(EditorStateMachine e)
	{
		MVGameController.Game.CameraController.CurCamera.FocusOnObject(cubeModelBase);
		e.SelectWO(cubeModelBase.Id, addToSelection: false);
		e.Event = EditorEvent.EditCubes;
		return true;
	}

	public virtual bool OnExitObject(EditorStateMachine e)
	{
		e.ExitGroupToRoot();
		e.Event = EditorEvent.ESTerrainEdit;
		return true;
	}

	protected void SetConstraints(IntVector min, IntVector max, int minCubeCount)
	{
		cubeModelBase.ModelingConstraintBuilder = () => new ModelingBoxCountConstraint(cubeModelBase, min, max, minCubeCount);
	}
}
