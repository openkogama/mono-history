using System;
using System.Collections.Generic;
using UnityEngine;

internal class JumpState
{
	private enum JumpType
	{
		Regular,
		Wall,
		Water
	}

	public delegate void OnWallJumpDelegate();

	private readonly float extraHeight = 4.1f;

	private readonly float baseHeight = 1f;

	private readonly float sliperyValMin = 0.3f;

	private readonly float sliperyValMax = 0.6f;

	private readonly float regularButtonDownTimeLimit = 0.2f;

	private readonly float bouncyMaterialButtonDownTimeLimit = 3f;

	private readonly float bouncinessThresshold = 0.3f;

	private readonly float wallJumpAngleMin = 70f;

	private readonly float wallJumpAngleMax = 92f;

	private readonly float jumpTimeOutWallJump = 0.25f;

	private bool holdingJumpButton;

	private float lastStartTime;

	private float lastButtonDownTime = -100f;

	private float jumpTimeOut;

	private Vector3 jumpDir = Vector3.up;

	private float accExtraHeight;

	private Vector3 latestSlopeDir;

	private bool jumping;

	public OnWallJumpDelegate OnWallJump;

	private List<MVControllerColliderHit> wallJumpHits = new List<MVControllerColliderHit>();

	public bool Jumping => jumping;

	public JumpState(float regularButtonDownTimeLimit)
	{
		this.regularButtonDownTimeLimit = regularButtonDownTimeLimit;
	}

	public void UpdateJumpState(GroundChange groundChange)
	{
		if (groundChange == GroundChange.FromAirToGrounded)
		{
			jumping = false;
		}
	}

	public Vector3 ApplyJumping(MVInteractableBase interactableLocal, MVGroundState groundState, float density, float waterProximity, bool inputJump, Vector3 velocity, Vector3 movableVelocity)
	{
		UpdateWallJumpValues();
		if (!inputJump)
		{
			holdingJumpButton = false;
			lastButtonDownTime = -100f;
		}
		if (inputJump && lastButtonDownTime < 0f)
		{
			lastButtonDownTime = Time.time;
		}
		if (!groundState.Grounded && jumping && holdingJumpButton && Time.time < lastStartTime + interactableLocal.HandleModifierEffect(AvatarModifierEffect.JumpPower, accExtraHeight) / MVPhysics.CalculateJumpVerticalSpeed(interactableLocal.HandleModifierEffect(AvatarModifierEffect.JumpPower, baseHeight)))
		{
			velocity += jumpDir * MVPhysics.Gravity * interactableLocal.HandleModifierEffect(AvatarModifierEffect.Density, density) * Time.deltaTime;
		}
		bool flag = waterProximity >= Math.Min(1f, interactableLocal.HandleModifierEffect(AvatarModifierEffect.Scale, 1f) / 2f) && Time.time - lastStartTime >= 0.5f && inputJump;
		bool flag2 = interactableLocal.HandleModifierEffect(AvatarModifierEffect.WallJump, 0f) > 0f;
		if ((groundState.Grounded || flag2 || flag) && Time.time - lastStartTime > jumpTimeOut)
		{
			float num = regularButtonDownTimeLimit;
			if (interactableLocal.HandleModifierEffect(AvatarModifierEffect.Bounciness, groundState.GroundMaterial.PhysicalProperties.bouncyness) > bouncinessThresshold)
			{
				num = bouncyMaterialButtonDownTimeLimit;
			}
			holdingJumpButton = false;
			if (Time.time - lastButtonDownTime < num || flag)
			{
				float sliperyFactor = GetSliperyFactor(interactableLocal, groundState, waterProximity, flag);
				SetJumpState(flag2, sliperyFactor);
				float jumpSpeed = GetJumpSpeed(interactableLocal, sliperyFactor);
				JumpType jumpType = JumpType.Regular;
				jumpDir = JumpDir(ref jumpType, groundState, sliperyFactor, flag2, flag);
				velocity = GetJumpTypeVelocity(velocity, jumpType);
				velocity += jumpDir * jumpSpeed;
				velocity += movableVelocity;
				if (jumpType == JumpType.Wall && OnWallJump != null)
				{
					OnWallJump();
				}
			}
		}
		return velocity;
	}

	private void SetJumpState(bool wallJumpPossible, float sliperyFactor)
	{
		jumping = true;
		lastStartTime = Time.time;
		lastButtonDownTime = -100f;
		holdingJumpButton = true;
		if (wallJumpPossible)
		{
			jumpTimeOut = jumpTimeOutWallJump;
		}
		else
		{
			jumpTimeOut = 0f;
		}
		accExtraHeight = extraHeight - extraHeight * sliperyFactor;
	}

	private float GetSliperyFactor(MVInteractableBase interactableLocal, MVGroundState groundState, float waterProximity, bool canWaterJump)
	{
		float num = Mathf.Sin(groundState.GradientAngle * ((float)Math.PI / 180f)) * (1f - SpreadFunction(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.PhysicalProperties.friction)));
		if (num < sliperyValMin || (canWaterJump && waterProximity > 0.5f))
		{
			num = 0f;
		}
		return num;
	}

	private float GetJumpSpeed(MVInteractableBase interactableLocal, float sliperyFactor)
	{
		float num = MVPhysics.CalculateJumpVerticalSpeed(interactableLocal.HandleModifierEffect(AvatarModifierEffect.JumpPower, baseHeight));
		return num - num * sliperyFactor;
	}

	private Vector3 GetJumpTypeVelocity(Vector3 velocity, JumpType jumpType)
	{
		switch (jumpType)
		{
		case JumpType.Wall:
			return GetWallJumpVelocity(velocity);
		case JumpType.Water:
			return velocity;
		default:
			if (velocity.y < 0f)
			{
				velocity.y = 0f;
				return velocity;
			}
			return velocity;
		}
	}

	private Vector3 GetWallJumpVelocity(Vector3 velocity)
	{
		if (velocity.y >= 0f)
		{
			return velocity;
		}
		Vector3 vector = velocity;
		float magnitude = velocity.magnitude;
		velocity.y = (vector.y = 0f);
		vector.Normalize();
		vector *= magnitude;
		float magnitude2 = new Vector2(velocity.x, velocity.z).magnitude;
		if (magnitude2 > 5f)
		{
			velocity = vector;
		}
		return velocity;
	}

	private Vector3 JumpDir(ref JumpType jumpType, MVGroundState groundState, float sliperyFactor, bool wallJumpPossible, bool canWaterJump)
	{
		if (!groundState.Grounded && canWaterJump)
		{
			jumpType = JumpType.Water;
			return Vector3.up;
		}
		if (sliperyFactor > sliperyValMax)
		{
			jumpType = JumpType.Regular;
			return groundState.GroundNormal;
		}
		if (!groundState.Grounded && wallJumpPossible)
		{
			float num = Vector3.Angle(Vector3.up, latestSlopeDir);
			if (num > wallJumpAngleMin && num < wallJumpAngleMax)
			{
				latestSlopeDir += Vector3.up;
				latestSlopeDir.Normalize();
				jumpType = JumpType.Wall;
				return latestSlopeDir;
			}
		}
		jumpType = JumpType.Regular;
		return Vector3.up;
	}

	private void UpdateWallJumpValues()
	{
		if (wallJumpHits.Count > 0)
		{
			latestSlopeDir = Vector3.zero;
			foreach (MVControllerColliderHit wallJumpHit in wallJumpHits)
			{
				latestSlopeDir += wallJumpHit.slopeNormal;
			}
			latestSlopeDir.Normalize();
		}
		wallJumpHits.Clear();
	}

	public void HandleMoveHit(MVControllerColliderHit moveHit)
	{
		if (moveHit.material.ModifierPackageType == AvatarModifierPackageType.WallJump)
		{
			wallJumpHits.Add(moveHit);
		}
	}

	private float SpreadFunction(float x)
	{
		return 0f - x * x + 2f * x;
	}
}
