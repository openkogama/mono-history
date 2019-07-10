using UnityEngine;

public interface ILaserPointer
{
	void SetLaserActiveState(bool isActive);

	void SetCurrentCubeMaterial(byte cubeMaterial);

	void ChangeState(LaserPointerState newState);

	void UpdatePosition(Vector3 to);

	void ActivateLaserForDuration(float duration);
}
