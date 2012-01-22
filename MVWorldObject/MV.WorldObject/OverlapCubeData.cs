namespace MV.WorldObject;

public struct OverlapCubeData(IntVector cubePos, OverlapState overlapState)
{
	public IntVector cubePos = cubePos;

	public OverlapState overlapState = overlapState;
}
