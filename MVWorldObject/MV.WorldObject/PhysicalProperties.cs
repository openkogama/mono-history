namespace MV.WorldObject;

public struct PhysicalProperties(float friction, float bouncyness, float damagePrSec, float softness, float staticFriction, float toughness)
{
	public float friction = friction;

	public float bouncyness = bouncyness;

	public float damagePrSec = damagePrSec;

	public float softness = softness;

	public float staticFriction = staticFriction;

	public float frictionPower = friction * friction;

	public float toughness = toughness;
}
