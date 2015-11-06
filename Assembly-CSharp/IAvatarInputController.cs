using UnityEngine;

internal interface IAvatarInputController : IMotorAPI
{
	void HandleInput(Vector3 moveDirection, bool jump, bool didShoot, Vector3 velocity, bool inGunMode, bool forceRotateToCamDirection);
}
