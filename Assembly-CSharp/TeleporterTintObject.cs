using UnityEngine;

public class TeleporterTintObject : TintObject
{
	[SerializeField]
	private MeshRenderer meshRendererToTint;

	[SerializeField]
	private Material materialCylinderToTint;

	[SerializeField]
	private ParticleSystem particleSystemToTint;

	[SerializeField]
	private Light lightToTint;

	private void Awake()
	{
		meshRendererToTint.materials = new Material[1] { materialCylinderToTint };
		materialCylinderToTint = meshRendererToTint.materials[0];
	}

	private void OnDestroy()
	{
		Object.Destroy(materialCylinderToTint);
	}

	public override void Tint(Color c)
	{
		materialCylinderToTint.color = c;
		particleSystemToTint.startColor = new Color(c.r / 2f, c.g / 2f, c.b / 2f, 1f);
		lightToTint.color = c;
	}
}
