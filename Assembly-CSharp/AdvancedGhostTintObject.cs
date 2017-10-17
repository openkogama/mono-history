using UnityEngine;

public class AdvancedGhostTintObject : TintObject
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private Material materialToTint;

	public Material Material { get; set; }

	private void Awake()
	{
		materialToTint = new Material(materialToTint);
		meshRenderer.sharedMaterial = materialToTint;
		Material = meshRenderer.sharedMaterial;
	}

	public override void Tint(Color c)
	{
		materialToTint.color = c;
	}
}
