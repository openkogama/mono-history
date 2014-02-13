namespace MV.WorldObject;

public struct PhysicalProperties(float friction, float bouncyness, float softness, float staticFriction, float toughness)
{
	public float friction = friction;

	public float bouncyness = bouncyness;

	public float softness = softness;

	public float staticFriction = staticFriction;

	public float toughness = toughness;
}
