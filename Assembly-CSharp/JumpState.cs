using System;
using System.Collections.Generic;
using UnityEngine;

internal class JumpState
{
	public delegate void OnWallJumpDelegate();

	private const float regularButtonDownTimeLimit = 0.2f;

	private const float bouncyMaterialButtonDownTimeLimit = 3f;

	private const float bouncinessThresshold = 0.3f;

	private float baseHeight = 1f;

	private float extraHeight = 4.1f;

	private float accExtraHeight;

	private bool holdingJumpButton;

	private float lastStartTime;

	private float lastButtonDownTime = -100f;

	private Vector3 jumpDir = Vector3.up;

	private float sliperyValMin = 0.3f;

	private float sliperyValMax = 0.6f;

	private float wallJumpAngleMin = 70f;

	private float wallJumpAngleMax = 92f;

	private float jumpTimeOut;

	private float jumpTimeOutWallJump = 0.25f;

	private Vector3 latestSlopeDir;

	private float latestFriction;

	private bool jumping;

	public OnWallJumpDelegate OnWallJump;

	private bool canWaterJump;

	public bool Jumping => jumping;

	public JumpState()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		SetValuesToTweakSheet();
	}

	private void SetValuesToTweakSheet()
	{
		baseHeight = AvatarTweakSheet.CharacterMotorJumping.baseHeight;
		extraHeight = AvatarTweakSheet.CharacterMotorJumping.extraHeight;
		sliperyValMin = AvatarTweakSheet.CharacterMotorJumping.sliperyValMin;
		sliperyValMax = AvatarTweakSheet.CharacterMotorJumping.sliperyValMax;
	}

	public void UpdateJumpState(GroundChange groundChange)
	{
		if (groundChange == GroundChange.FromAirToGrounded)
		{
			jumping = false;
		}
	}

	public Vector3 ApplyJumping(MVInteractableBase interactableLocal, MVGroundState groundState, float density, float waterProximity, bool inputJump, Vector3 velocity, List<MVControllerColliderHit> moveHits)
	{
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		HandleMoveHits(moveHits);
		if (!inputJump)
		{
			holdingJumpButton = false;
			lastButtonDownTime = -100f;
		}
		if (inputJump && lastButtonDownTime < 0f)
		{
			lastButtonDownTime = Time.time;
		}
		if (!groundState.Grounded)
		{
			if (jumping && holdingJumpButton)
			{
				if (Time.time < lastStartTime + interactableLocal.HandleModifierEffect(AvatarModifierEffect.JumpPower, accExtraHeight) / MVPhysics.CalculateJumpVerticalSpeed(interactableLocal.HandleModifierEffect(AvatarModifierEffect.JumpPower, baseHeight)))
				{
					velocity += jumpDir * 30f * interactableLocal.HandleModifierEffect(AvatarModifierEffect.Density, density) * Time.deltaTime;
					canWaterJump = false;
				}
			}
			else
			{
				canWaterJump = true;
			}
		}
		if (waterProximity < 1f)
		{
			canWaterJump = false;
		}
		bool flag = interactableLocal.HandleModifierEffect(AvatarModifierEffect.WallJump, 0f) > 0f;
		if ((groundState.Grounded || flag || (canWaterJump && Time.time - lastStartTime > 0.5f)) && Time.time - lastStartTime > jumpTimeOut)
		{
			float num = 0.2f;
			if (interactableLocal.HandleModifierEffect(AvatarModifierEffect.Bounciness, groundState.GroundMaterial.physicalProperties.bouncyness) > 0.3f)
			{
				num = 3f;
			}
			if (Time.time - lastButtonDownTime < num)
			{
				float num2 = Mathf.Sin(groundState.GradientAngle * ((float)Math.PI / 180f)) * (1f - SpreadFunction(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.physicalProperties.friction)));
				if (num2 < sliperyValMin || (canWaterJump && waterProximity > 0.5f))
				{
					num2 = 0f;
				}
				jumping = true;
				lastStartTime = Time.time;
				lastButtonDownTime = -100f;
				holdingJumpButton = true;
				jumpDir = Vector3.up;
				float num3 = MVPhysics.CalculateJumpVerticalSpeed(interactableLocal.HandleModifierEffect(AvatarModifierEffect.JumpPower, baseHeight));
				accExtraHeight = extraHeight - extraHeight * num2;
				num3 -= num3 * num2;
				bool flag2 = false;
				if (!groundState.Grounded && canWaterJump)
				{
					jumpDir = Vector3.up;
				}
				else if (num2 > sliperyValMax)
				{
					jumpDir = groundState.GroundNormal;
				}
				else if (!groundState.Grounded && flag)
				{
					float num4 = Vector3.Angle(Vector3.up, latestSlopeDir);
					if (num4 > wallJumpAngleMin && num4 < wallJumpAngleMax)
					{
						latestSlopeDir += Vector3.up;
						latestSlopeDir.Normalize();
						jumpDir = latestSlopeDir;
						if (velocity.y < 0f)
						{
							Vector3 val = velocity;
							float magnitude = velocity.magnitude;
							velocity.y = (val.y = 0f);
							val.Normalize();
							val *= magnitude;
							Vector2 val2 = new Vector2(velocity.x, velocity.z);
							float magnitude2 = val2.magnitude;
							if (magnitude2 > 5f)
							{
								velocity = val;
							}
						}
						flag2 = true;
					}
				}
				if (flag)
				{
					jumpTimeOut = jumpTimeOutWallJump;
				}
				else
				{
					jumpTimeOut = 0f;
				}
				if (velocity.y < 0f && !flag2)
				{
					velocity.y = 0f;
				}
				if (flag2 && OnWallJump != null)
				{
					OnWallJump();
				}
				velocity += jumpDir * num3;
				canWaterJump = false;
			}
			else
			{
				holdingJumpButton = false;
			}
		}
		return velocity;
	}

	private void HandleMoveHits(List<MVControllerColliderHit> moveHits)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		List<MVControllerColliderHit> list = moveHits.FindAll((MVControllerColliderHit x) => x.material.modifierPackageType == AvatarModifierPackageType.WallJump);
		if (list.Count <= 0)
		{
			return;
		}
		latestSlopeDir = Vector3.zero;
		latestFriction = 0f;
		foreach (MVControllerColliderHit item in list)
		{
			latestSlopeDir += item.slopeNormal;
			latestFriction += item.material.physicalProperties.friction;
		}
		latestFriction /= moveHits.Count;
		latestSlopeDir.Normalize();
	}

	private float SpreadFunction(float x)
	{
		return 0f - x * x + 2f * x;
	}
}
