using MV.WorldObject;

public interface IModelingConstraint
{
	bool CanAddCubeAt(IntVector pos);

	bool CanRemoveCubeAt(IntVector pos);

	bool CanEditCubeAt(IntVector pos);
}
