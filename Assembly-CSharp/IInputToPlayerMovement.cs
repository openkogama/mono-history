using UnityEngine;

public interface IInputToPlayerMovement
{
	Vector3 Direction { get; }

	bool Jump { get; }

	void HandleInputState(bool fromFrameUpdate);
}
