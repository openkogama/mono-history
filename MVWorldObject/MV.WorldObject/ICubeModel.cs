using UnityEngine;

namespace MV.WorldObject;

public interface ICubeModel
{
	Vector3 Scale { get; }

	bool ContainsCube(IntVector localPos);

	CubeBase GetCubeBase(IntVector localPos);

	void AddCube(IntVector pos, CubeBase cube);

	void RemoveCube(IntVector pos);

	void AddCubeNetworkUpdate(IntVector pos, CubeBase cube);

	void RemoveCubeNetworkUpdate(IntVector pos);
}
