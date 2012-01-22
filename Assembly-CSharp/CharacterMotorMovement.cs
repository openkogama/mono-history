using UnityEngine;

public class CharacterMotorMovement
{
	public float maxForwardSpeed = 10f;

	public float maxSidewaysSpeed = 10f;

	public float maxBackwardsSpeed = 10f;

	public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe[3]
	{
		new Keyframe(-90f, 1f),
		new Keyframe(0f, 1f),
		new Keyframe(90f, 1f)
	});

	public MVCollisionFlags collisionFlags;

	public Vector3 velocity;

	public CharacterMotorMovement()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected Obj, but got Unknown
	}
}
