using UnityEngine;

public interface IMotorAPI
{
	Vector3 Direction { get; }

	Quaternion Rotation { get; set; }

	bool Jump { get; }
}
