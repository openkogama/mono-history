using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor
{
	public delegate void OnDamageDelegate(float damage);

	public Vector3 inputMoveDirection = Vector3.zero;

	public bool inputJump;

	public CharacterMotorMovement movement = new CharacterMotorMovement();

	private CharacterMotorJumping jumping = new CharacterMotorJumping();

	private Transform tr;

	private MvCharacterController controller;

	private List<MVControllerColliderHit> moveHits = new List<MVControllerColliderHit>();

	private ImpactState impactState = new ImpactState();

	private BounceState bounceState = new BounceState();

	private GroundState groundState;

	public List<Vector3> impulseVectors = new List<Vector3>();

	private float inAirControlFactor = 3f;

	public OnDamageDelegate OnDamage;

	public CharacterMotor(MvCharacterController controller)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SetValuesToTweakSheet();
		this.controller = controller;
		controller.OnControllerColliderHit = (MvCharacterController.OnControllerColliderHitDelegate)Delegate.Combine(controller.OnControllerColliderHit, new MvCharacterController.OnControllerColliderHitDelegate(OnControllerColliderHit));
		tr = ((Component)controller).transform;
		groundState = new GroundState(controller);
	}

	private void SetValuesToTweakSheet()
	{
		inAirControlFactor = AvatarTweakSheet.CharacterMotor.inAirControlFactor;
	}

	private void OnControllerColliderHit(MVControllerColliderHit hit)
	{
		moveHits.Add(hit);
	}

	public void Reset()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		movement.velocity = Vector3.zero;
		impactState.prevVelocityChangeVector = Vector3.zero;
		controller.Velocity = Vector3.zero;
		impulseVectors.Clear();
	}

	public void AddImpulse(Vector3 impulse, bool suspendImpactDamage = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		impulseVectors.Add(impulse);
		if (suspendImpactDamage)
		{
			SuspendImpactDamage();
		}
	}

	public void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	private Vector3 GetImpulse(Vector3 velocity)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (impulseVectors.Count == 0)
		{
			return velocity;
		}
		Vector3 val = Vector3.zero;
		foreach (Vector3 impulseVector in impulseVectors)
		{
			val += impulseVector;
		}
		val /= (float)impulseVectors.Count;
		val *= Time.deltaTime;
		impulseVectors.Clear();
		return velocity + val;
	}

	public void UpdateFunction()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		Vector3 velocity = movement.velocity;
		Vector3 velocity2 = movement.velocity;
		velocity2 = GetVelocity(velocity2);
		Move(velocity2);
		groundState.UpdateIsGrounded(velocity2, moveHits);
		bounceState.UpdateBounceState(moveHits);
		movement.velocity = controller.Velocity / Time.deltaTime;
		if (movement.velocity.y < velocity2.y - 0.001f && movement.velocity.y > 0f)
		{
			jumping.holdingJumpButton = false;
		}
		if (groundState.Grounded && !groundState.IsGroundedTest())
		{
			groundState.Grounded = false;
		}
		else if (!groundState.Grounded && groundState.IsGroundedTest())
		{
			groundState.Grounded = true;
			jumping.jumping = false;
		}
		DealDamage(movement.velocity, velocity);
	}

	private void DealDamage(Vector3 curVelocity, Vector3 prevVelocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Vector3 curVelocityChangeVector = (curVelocity - prevVelocity) * Time.deltaTime;
		impactState.UpdateImpactState(curVelocityChangeVector, moveHits);
		float impactDamage = impactState.ImpactDamage;
		impactDamage += GetDamageFromMaterials();
		if (impactDamage != 0f && OnDamage != null)
		{
			OnDamage(impactDamage);
		}
	}

	private Vector3 GetVelocity(Vector3 velocity)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (groundState.IsGrounded())
		{
			velocity = ApplySlidingVelocity(velocity);
		}
		if (groundState.IsGrounded())
		{
			velocity -= velocity * groundState.PhysicalProperties.frictionPower * Time.deltaTime;
		}
		velocity = ApplyInputVelocityChange(velocity);
		velocity = ApplyGravityAndJumping(velocity);
		velocity = bounceState.ApplyBounceVelocity(velocity);
		velocity = GetImpulse(velocity);
		return velocity;
	}

	private void Move(Vector3 velocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vector3 motion = velocity * Time.deltaTime;
		moveHits.Clear();
		movement.collisionFlags = controller.Move(motion);
	}

	private float GetDamageFromMaterials()
	{
		float num = 0f;
		int num2 = 0;
		foreach (MVControllerColliderHit moveHit in moveHits)
		{
			if (moveHit.material.physicalProperties.damagePrSec != 0f)
			{
				num += moveHit.material.physicalProperties.damagePrSec;
				num2++;
			}
		}
		if (num2 == 0)
		{
			return 0f;
		}
		return num / (float)num2 * Time.deltaTime;
	}

	private Vector3 ApplySlidingVelocity(Vector3 velocity)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (groundState.Grounded)
		{
			Vector3 gradientDirection = groundState.GradientDirection;
			float gradientAngle = groundState.GradientAngle;
			Vector3 val = gradientDirection * Mathf.Sin(gradientAngle * ((float)Math.PI / 180f)) * (1f - groundState.PhysicalProperties.frictionPower) * 30f;
			if (val.magnitude > groundState.PhysicalProperties.staticFriction)
			{
				velocity += val * Time.deltaTime;
			}
		}
		return velocity;
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GetDesiredHorizontalVelocity();
		if (groundState.Grounded)
		{
			val = AdjustGroundVelocityToNormal(val, groundState.GroundNormal);
		}
		Vector3 val2 = val - velocity;
		if (groundState.Grounded)
		{
			val2 *= groundState.PhysicalProperties.frictionPower;
			velocity += val2;
			if (groundState.PhysicalProperties.frictionPower < 0.1f && val.magnitude != 0f)
			{
				velocity += val * 0.5f * Time.deltaTime;
			}
		}
		else if (val.magnitude != 0f)
		{
			Vector3 val3 = velocity;
			val3.y = 0f;
			float magnitude = val3.magnitude;
			val3 += val * inAirControlFactor * Time.deltaTime;
			float magnitude2 = val3.magnitude;
			if (magnitude2 > magnitude && magnitude2 > movement.maxForwardSpeed)
			{
				val3.Normalize();
				val3 *= magnitude;
			}
			velocity.x = val3.x;
			velocity.z = val3.z;
		}
		return velocity;
	}

	private Vector3 ApplyGravityAndJumping(Vector3 velocity)
	{
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		if (!inputJump)
		{
			jumping.holdingJumpButton = false;
			jumping.lastButtonDownTime = -100f;
		}
		if (inputJump && jumping.lastButtonDownTime < 0f)
		{
			jumping.lastButtonDownTime = Time.time;
		}
		if (!groundState.Grounded)
		{
			velocity.y = movement.velocity.y - 30f * Time.deltaTime;
			if (jumping.jumping && jumping.holdingJumpButton && Time.time < jumping.lastStartTime + jumping.accExtraHeight / MVPhysics.CalculateJumpVerticalSpeed(jumping.baseHeight))
			{
				velocity += jumping.jumpDir * 30f * Time.deltaTime;
			}
		}
		if (groundState.Grounded)
		{
			if (jumping.enabled && Time.time - jumping.lastButtonDownTime < 0.2f)
			{
				float gradientAngle = groundState.GradientAngle;
				float num = Mathf.Sin(gradientAngle * ((float)Math.PI / 180f)) * (1f - SpreadFunction(groundState.PhysicalProperties.friction));
				if (num < jumping.sliperyValMin)
				{
					num = 0f;
				}
				groundState.Grounded = false;
				jumping.jumping = true;
				jumping.lastStartTime = Time.time;
				jumping.lastButtonDownTime = -100f;
				jumping.holdingJumpButton = true;
				jumping.jumpDir = Vector3.Slerp(Vector3.up, groundState.GroundNormal, jumping.perpAmount);
				float num2 = MVPhysics.CalculateJumpVerticalSpeed(jumping.baseHeight);
				jumping.accExtraHeight = jumping.extraHeight - jumping.extraHeight * num;
				num2 -= num2 * num;
				if (num > jumping.sliperyValMax)
				{
					jumping.jumpDir = groundState.GroundNormal;
				}
				velocity += jumping.jumpDir * num2;
			}
			else
			{
				jumping.holdingJumpButton = false;
			}
		}
		return velocity;
	}

	private float SpreadFunction(float x)
	{
		return 0f - x * x + 2f * x;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = tr.InverseTransformDirection(inputMoveDirection);
		float num = MaxSpeedInDirection(val);
		if (groundState.Grounded)
		{
			float num2 = Mathf.Asin(movement.velocity.normalized.y) * 57.29578f;
			num *= movement.slopeSpeedMultiplier.Evaluate(num2);
		}
		return tr.TransformDirection(val * num);
	}

	private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Cross(Vector3.up, hVelocity);
		Vector3 val2 = Vector3.Cross(val, groundNormal);
		return val2.normalized * hVelocity.magnitude;
	}

	public bool IsJumping()
	{
		return jumping.jumping;
	}

	private bool IsTouchingCeiling()
	{
		return (movement.collisionFlags & MVCollisionFlags.Above) != 0;
	}

	private float MaxSpeedInDirection(Vector3 desiredMovementDirection)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (desiredMovementDirection == Vector3.zero)
		{
			return 0f;
		}
		float num = ((!(desiredMovementDirection.z > 0f)) ? movement.maxBackwardsSpeed : movement.maxForwardSpeed) / movement.maxSidewaysSpeed;
		Vector3 val = new Vector3(desiredMovementDirection.x, 0f, desiredMovementDirection.z / num);
		Vector3 normalized = val.normalized;
		Vector3 val2 = new Vector3(normalized.x, 0f, normalized.z * num);
		return val2.magnitude * movement.maxSidewaysSpeed;
	}

	public void SetMaxSpeed(float speed)
	{
		movement.maxForwardSpeed = speed;
		movement.maxSidewaysSpeed = speed;
		movement.maxBackwardsSpeed = speed;
	}
}
