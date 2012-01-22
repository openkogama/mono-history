using UnityEngine;

internal class CharacterMotorJumping
{
	public bool enabled = true;

	public float baseHeight = 1f;

	public float extraHeight = 4.1f;

	public float accExtraHeight = 4.1f;

	public float perpAmount;

	public float steepPerpAmount = 0.5f;

	public bool jumping;

	public bool holdingJumpButton;

	public float lastStartTime;

	public float lastButtonDownTime = -100f;

	public Vector3 jumpDir = Vector3.up;

	public float sliperyValMin = 0.3f;

	public float sliperyValMax = 0.6f;

	public CharacterMotorJumping()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		SetValuesToTweakSheet();
	}

	private void SetValuesToTweakSheet()
	{
		baseHeight = AvatarTweakSheet.CharacterMotorJumping.baseHeight;
		extraHeight = AvatarTweakSheet.CharacterMotorJumping.extraHeight;
		accExtraHeight = AvatarTweakSheet.CharacterMotorJumping.extraHeight;
		sliperyValMin = AvatarTweakSheet.CharacterMotorJumping.sliperyValMin;
		sliperyValMax = AvatarTweakSheet.CharacterMotorJumping.sliperyValMax;
	}
}
