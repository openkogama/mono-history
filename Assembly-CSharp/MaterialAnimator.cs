using UnityEngine;

public abstract class MaterialAnimator : MonoBehaviour
{
	public Material targetMaterial;

	public Material TargetMaterial
	{
		get
		{
			return targetMaterial;
		}
		set
		{
			targetMaterial = value;
		}
	}
}
