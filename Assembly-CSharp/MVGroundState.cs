using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;
using UnityEngine;

public class MVGroundState
{
	private MVMaterial groundMaterial = new MVMaterial();

	private const float groundDepth = 0.1f;

	private ObscuredBool grounded = false;

	private Vector3 groundNormal = Vector3.zero;

	private float gradientAngle;

	private Vector3 gradientDirection;

	public Action<GroundChange> OnGroundChange;

	public float GradientAngle => gradientAngle;

	public float GroundDepth => 0.1f;

	public MVMaterial GroundMaterial => groundMaterial;

	public bool Grounded => grounded;

	public Vector3 GroundNormal => groundNormal;

	public Vector3 ApplySlidingVelocity(Vector3 velocity, float density, MVInteractableBase interactableLocal)
	{
		if (Grounded)
		{
			Vector3 vector = gradientDirection * Mathf.Sin(gradientAngle * ((float)Math.PI / 180f)) * (1f - MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, GroundMaterial.physicalProperties.friction))) * MVPhysics.Gravity * interactableLocal.HandleModifierEffect(AvatarModifierEffect.Density, density);
			if (vector.magnitude > interactableLocal.HandleModifierEffect(AvatarModifierEffect.StaticFriction, GroundMaterial.physicalProperties.staticFriction))
			{
				velocity += vector * Time.deltaTime;
			}
		}
		return velocity;
	}

	public bool GroundTest(out MVControllerColliderHit groundHit, MvCharacterController controller, Vector3 velocity, bool sendCollData, float additionalGroundDepth = 0f)
	{
		return controller.TestWithOutSliding(0.1f + additionalGroundDepth, Vector3.down, velocity * Time.fixedDeltaTime, out groundHit);
	}

	public bool Update(MvCharacterController controller, Vector3 velocity, float additionalGroundDepth = 0f)
	{
		bool flag = GroundTest(out var groundHit, controller, velocity, sendCollData: true, additionalGroundDepth);
		UpdateGroundStateWithHit(controller, flag, groundHit);
		return flag;
	}

	public void UpdateGroundStateWithHitExternal(MvCharacterController controller, MVControllerColliderHit groundHit)
	{
		UpdateGroundStateWithHit(controller, foundGroundHit: true, groundHit);
	}

	private void UpdateGroundStateWithHit(MvCharacterController controller, bool foundGroundHit, MVControllerColliderHit groundHit)
	{
		UpdateGroundData(controller, foundGroundHit, groundHit);
		UpdateGroundChange();
	}

	private void UpdateGroundData(MvCharacterController controller, bool foundGroundHit, MVControllerColliderHit groundHit)
	{
		groundNormal = Vector3.zero;
		if (foundGroundHit)
		{
			groundHit.impactVelocity = Vector3.zero;
			groundNormal = groundHit.slopeNormal;
			gradientDirection = controller.GetGradientDirection(groundHit.hit);
			gradientAngle = GetGradientAngle(gradientDirection);
			groundMaterial = MVGameControllerBase.Game.MaterialRepository.GetMaterial(CubeBase.GetMaterial(groundHit.hit.cube, groundHit.hit.face));
		}
		else
		{
			groundMaterial = MVGameControllerBase.Game.MaterialRepository.InAirMaterial;
		}
	}

	private static float GetGradientAngle(Vector3 gradientDirection)
	{
		return Vector3.Angle(Vector3.up, gradientDirection) - 90f;
	}

	private void UpdateGroundChange()
	{
		GroundChange obj = GroundChange.UnChanged;
		if ((bool)grounded && !IsGroundedTest())
		{
			grounded = false;
			obj = GroundChange.FromGroundedToAir;
		}
		else if (!grounded && IsGroundedTest())
		{
			grounded = true;
			obj = GroundChange.FromAirToGrounded;
		}
		if (OnGroundChange != null)
		{
			OnGroundChange(obj);
		}
	}

	private bool IsGroundedTest()
	{
		return groundNormal.y > 0.01f;
	}
}
