using MV.WorldObject;
using UnityEngine;

public interface ICubeModelCollider
{
	int Id { get; }

	GameObject GameObject { get; }

	Transform Transform { get; }

	InteractionFlags InteractionFlags { get; }

	ChunkInstances ChunkInstances { get; }

	Vector3 Scale { get; }

	Quaternion WorldRotation { get; }

	RuntimePrototypeCubeModel PrototypeCubeModel { get; }

	Cube GetCube(IntVector pos);

	void CubePosToChunkPos(ref IntVector pos);
}
