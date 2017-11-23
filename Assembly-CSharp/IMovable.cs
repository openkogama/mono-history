using UnityEngine;

public interface IMovable
{
	Vector3 Velocity { get; }

	Bounds Bounds { get; }

	Vector3 Position { get; }
}
