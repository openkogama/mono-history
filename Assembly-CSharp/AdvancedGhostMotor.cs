using System;
using UnityEngine;

public class AdvancedGhostMotor : MVRigidBody
{
	private float baseHeight;

	private Transform targetTransform;

	private float ghostFriction = 0.2f;

	private Vector3 prevLocalPosition;

	private float minDeltaPos = 0.01f;

	private Vector3 velocity;

	private float speedSmoothing = 5f;

	private MVInteractableBase interactable;

	private SmoothPhysicsMovement smoothPhysicsMovement;

	public override bool Grounded => targetTransform.position.y <= baseHeight;

	public override Vector3 Velocity => velocity;

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

	public Vector3 MoveDirection { get; set; }

	protected override void SuspendImpactDamage()
	{
	}

	public void Init(GameObject ghostBehaviour, MVInteractableBase interactable)
	{
		weight = 0.7f;
		this.interactable = interactable;
		baseHeight = ghostBehaviour.transform.position.y;
		GameObject gameObject = new GameObject(ghostBehaviour.name + " physics");
		gameObject.transform.parent = ghostBehaviour.transform.parent;
		gameObject.transform.position = ghostBehaviour.transform.position;
		gameObject.transform.rotation = ghostBehaviour.transform.rotation;
		targetTransform = gameObject.transform;
		smoothPhysicsMovement = ghostBehaviour.AddComponent<SmoothPhysicsMovement>();
		smoothPhysicsMovement.Init(targetTransform);
	}

	public void FixedUpdateFunction()
	{
		UpdateVelocity();
		Move(velocity);
	}

	public void FixedUpdateRotation()
	{
		Vector3 localPosition = targetTransform.localPosition;
		if ((localPosition - prevLocalPosition).sqrMagnitude > minDeltaPos * minDeltaPos)
		{
			Vector3 normalized = (localPosition - prevLocalPosition).normalized;
			prevLocalPosition = localPosition;
			if (Mathf.Abs(normalized.y) < 0.5f)
			{
				targetTransform.localRotation = Quaternion.Slerp(targetTransform.localRotation, Quaternion.LookRotation(normalized), 0.1f * (Time.fixedDeltaTime / 0.02f));
			}
		}
	}

	public void FrameUpdate()
	{
		smoothPhysicsMovement.SmoothMove();
	}

	public void Reset(Vector3 velocity)
	{
		base.Reset();
		this.velocity = velocity;
		smoothPhysicsMovement.Reset();
	}

	private void UpdateVelocity()
	{
		velocity -= velocity * MathFunctions.Pow2(interactable.HandleModifierEffect(AvatarModifierEffect.Friction, ghostFriction)) * Time.deltaTime;
		velocity = ApplyInputVelocityChange();
		velocity = GetImpulse(velocity, interactable);
		MVRigidBody.VelocityDamping(velocity, 1f, interactable);
	}

	private void Move(Vector3 velocity)
	{
		Vector3 vector = velocity * Time.deltaTime;
		targetTransform.position += vector;
	}

	private Vector3 ApplyInputVelocityChange()
	{
		Vector3 desiredHorizontalVelocity = GetDesiredHorizontalVelocity();
		Vector3 vector = desiredHorizontalVelocity - velocity;
		vector *= MathFunctions.Pow2(interactable.HandleModifierEffect(AvatarModifierEffect.Friction, ghostFriction));
		velocity += vector;
		return velocity;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		float magnitude = velocity.magnitude;
		float magnitude2 = MoveDirection.magnitude;
		float t = speedSmoothing * Time.deltaTime;
		magnitude = Mathf.Lerp(magnitude, magnitude2, t);
		return MoveDirection.normalized * magnitude;
	}
}
