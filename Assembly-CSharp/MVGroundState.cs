using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

internal class MVGroundState
{
	private MVMaterial groundMaterial = new MVMaterial();

	private float groundDepth = 0.1f;

	private bool grounded;

	private Vector3 groundNormal = Vector3.zero;

	private float gradientAngle;

	private Vector3 gradientDirection;

	public float GradientAngle => gradientAngle;

	public float GroundDepth => groundDepth;

	public MVMaterial GroundMaterial => groundMaterial;

	public bool Grounded => grounded;

	public Vector3 GroundNormal
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return groundNormal;
		}
	}

	public MVGroundState()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	}

	public Vector3 ApplySlidingVelocity(Vector3 velocity, float density, MVInteractableBase interactableLocal)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (Grounded)
		{
			Vector3 val = gradientDirection * Mathf.Sin(gradientAngle * ((float)Math.PI / 180f)) * (1f - MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, GroundMaterial.physicalProperties.friction))) * 30f * interactableLocal.HandleModifierEffect(AvatarModifierEffect.Density, density);
			if (val.magnitude > interactableLocal.HandleModifierEffect(AvatarModifierEffect.StaticFriction, GroundMaterial.physicalProperties.staticFriction))
			{
				velocity += val * Time.deltaTime;
			}
		}
		return velocity;
	}

	public GroundChange UpdateIsGrounded(Vector3 velocity, List<MVControllerColliderHit> moveHits, MvCharacterController controller)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		groundNormal = Vector3.zero;
		if (controller.TestWithOutSliding(groundDepth, Vector3.down, out var colliderHit))
		{
			colliderHit.impactVelocity = Vector3.zero;
			groundNormal = colliderHit.slopeNormal;
			if (moveHits.Count == 0 && velocity.magnitude > 0.01f)
			{
				colliderHit.impactVelocity = velocity;
			}
			moveHits.Add(colliderHit);
			gradientDirection = controller.GetGradientDirection(colliderHit.hit);
			gradientAngle = controller.GetGradientAngle(gradientDirection);
			groundMaterial = MVGameController.Instance.Game.MaterialRepository.GetMaterial(CubeBase.GetMaterial(colliderHit.hit.cube, colliderHit.hit.face));
		}
		else
		{
			groundMaterial = MVGameController.Instance.Game.MaterialRepository.InAirMaterial;
		}
		if (Grounded && !IsGroundedTest())
		{
			grounded = false;
			return GroundChange.FromGroundedToAir;
		}
		if (!Grounded && IsGroundedTest())
		{
			grounded = true;
			return GroundChange.FromAirToGrounded;
		}
		return GroundChange.UnChanged;
	}

	private bool IsGroundedTest()
	{
		return groundNormal.y > 0.01f;
	}
}
