using MV.WorldObject;
using UnityEngine;

public class EditableCubeModelWrapper
{
	private MVCubeModelInstance cubeModelBase;

	public MVCubeModelInstance CubeModel => cubeModelBase;

	public EditableCubeModelWrapper(MVCubeModelInstance cubeModelBase)
	{
		this.cubeModelBase = cubeModelBase;
		cubeModelBase.InteractionFlags |= InteractionFlags.SelectionRequiresEditGroup;
	}

	public EditableCubeModelWrapper(MVCubeModelInstance cubeModelBase, IntVector min, IntVector max, int minCubeCount)
		: this(cubeModelBase)
	{
		SetConstraints(min, max, minCubeCount);
	}

	public virtual bool OnEnterObject(EditorStateMachine e)
	{
		Debug.Log(e);
		MVGameControllerBase.MainCameraManager.CurrentCamera.FocusOnObject(cubeModelBase);
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
