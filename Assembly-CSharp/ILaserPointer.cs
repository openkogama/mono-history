using UnityEngine;

public interface ILaserPointer
{
	bool LaserActive { get; set; }

	byte CurrentCubeMaterial { get; set; }

	void ChangeState(LaserPointerState newState);

	void SetLaserCubeVisible(bool visible);

	void UpdatePosition(Vector3 to);

	void ActivateLaserForDuration(float duration);
}
