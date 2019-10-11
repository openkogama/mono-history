using UnityEngine;

public interface IMotorAPI
{
	Vector3 Direction { get; set; }

	Quaternion Rotation { get; set; }

	bool Jump { get; }
}
