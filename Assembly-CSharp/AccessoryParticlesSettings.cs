using UnityEngine;

[AddComponentMenu("KoGaMa/AvatarAccessories/Particles")]
public class AccessoryParticlesSettings : AccessorySettings
{
	public bool useEmissionMovement = true;

	public float EmitRateNormal = 4f;

	public float EmitRateMoving = 10f;
}
