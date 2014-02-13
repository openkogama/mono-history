using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AdvancedGhostMotor : MVRigidBody
{
	private float baseHeight;

	private Transform targetTransform;

	private float ghostFriction = 0.2f;

	private float speedSmoothing = 5f;

	private MVInteractableBase interactable;

	private Vector3 velocity;

	public override bool Grounded
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return targetTransform.position.y <= baseHeight;
		}
	}

	public override Vector3 Velocity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return velocity;
		}
	}

	public override bool IsMovementLocked
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public Vector3 MoveDirection
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	protected override void SuspendImpactDamage()
	{
	}

	public void Init(Transform targetTransform, MVInteractableBase interactable)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		weight = 0.7f;
		this.interactable = interactable;
		this.targetTransform = targetTransform;
		baseHeight = targetTransform.position.y;
	}

	public void UpdateFunction()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UpdateVelocity();
		Move(velocity);
	}

	public void Reset(Vector3 velocity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.Reset();
		this.velocity = velocity;
	}

	private void UpdateVelocity()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		velocity -= velocity * MathFunctions.Pow2(interactable.HandleModifierEffect(AvatarModifierEffect.Friction, ghostFriction)) * Time.deltaTime;
		velocity = ApplyInputVelocityChange();
		velocity = GetImpulse(velocity, interactable);
		velocity *= interactable.HandleModifierEffect(AvatarModifierEffect.VelocityDamping, 1f);
	}

	private void Move(Vector3 velocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = velocity * Time.deltaTime;
		Transform val2 = targetTransform;
		val2.position += val;
	}

	private Vector3 ApplyInputVelocityChange()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector3 desiredHorizontalVelocity = GetDesiredHorizontalVelocity();
		Vector3 val = desiredHorizontalVelocity - velocity;
		val *= MathFunctions.Pow2(interactable.HandleModifierEffect(AvatarModifierEffect.Friction, ghostFriction));
		velocity += val;
		return velocity;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		float magnitude = velocity.magnitude;
		Vector3 moveDirection = MoveDirection;
		float magnitude2 = moveDirection.magnitude;
		float num = speedSmoothing * Time.deltaTime;
		magnitude = Mathf.Lerp(magnitude, magnitude2, num);
		Vector3 moveDirection2 = MoveDirection;
		return moveDirection2.normalized * magnitude;
	}
}
