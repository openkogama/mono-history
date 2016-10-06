using UnityEngine;

public class FlagTintObject : TintObject
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private Material materialToTint;

	[SerializeField]
	private Material materialBlack;

	private void Awake()
	{
		meshRenderer.materials = new Material[2] { materialToTint, materialBlack };
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
