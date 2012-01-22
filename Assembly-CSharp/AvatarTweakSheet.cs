public static class AvatarTweakSheet
{
	public static class WalkMode
	{
		public static float walkSpeed = 8f;

		public static float runSpeed = 8f;

		public static float animationSpeedRatio = 0.38f;

		public static float speedSmoothing = 10f;
	}

	public static class CharacterMotor
	{
		public static float inAirControlFactor = 3f;
	}

	public static class ImpactState
	{
		public static float maxAccBeforeDamageDealt = 55f;

		public static float impactDamageMultiplier = 200f;
	}

	public static class CharacterMotorJumping
	{
		public static float baseHeight = 1f;

		public static float extraHeight = 4.1f;

		public static float sliperyValMin = 0.3f;

		public static float sliperyValMax = 0.6f;
	}

	public static class BounceState
	{
		public static float maxHeight = 10f;

		public static float impactVelSlopeNormalMinDot = 0.2f;

		public static float minBounceVal = 2f;
	}
}
