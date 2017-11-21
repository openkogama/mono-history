using MV.WorldObject;
using UnityEngine;

public class TeleporterTintObject : TintObject
{
	[SerializeField]
	private MeshRenderer meshRendererToTint;

	[SerializeField]
	private Material materialCylinderToTint;

	[SerializeField]
	private ParticleSystem particleCircleToTint;

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

	public override void TeamTint(MVTeam team)
	{
		switch (team)
		{
		case MVTeam.Blue:
			Tint(0.075f, 0.372f, 0.859f, 0f);
			break;
		case MVTeam.Red:
			Tint(0.855f, 0f, 0f, 0f);
			break;
		case MVTeam.Green:
			Tint(0f, 0.655f, 0f, 0f);
			break;
		case MVTeam.Yellow:
			Tint(0.7f, 0.7f, 0f, 0f);
			break;
		default:
			Tint(0.82f, 0.82f, 1f, 0f);
			break;
		}
	}

	public override void Tint(Color c)
	{
		materialCylinderToTint.color = c;
		Color startColor = new Color(c.r / 2f, c.g / 2f, c.b / 2f, 1f);
		particleCircleToTint.startColor = startColor;
		lightToTint.color = c;
	}
}
