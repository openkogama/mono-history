using UnityEngine;

public class TimeAttackFlagTintObject : TintObject
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private Material materialToTint;

	private void Awake()
	{
		meshRenderer.materials = new Material[1] { materialToTint };
		materialToTint = meshRenderer.materials[0];
	}

	private void OnDestroy()
	{
		Object.Destroy(materialToTint);
	}

	public override void Tint(Color c)
	{
		materialToTint.color = c;
	}
}
