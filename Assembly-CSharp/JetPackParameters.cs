using UnityEngine;

public class JetPackParameters : MonoBehaviour
{
	public float density;

	public float thrustStrength;

	public float thrustTimeOverheatThreshold;

	public float coolDownFactor;

	public int[] upperCubeConstraint;

	public int[] lowerCubeConstraint;

	public int minNumberOfCubes;
}
