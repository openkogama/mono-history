using MV.WorldObject;
using UnityEngine;

public class PressurePlateTintObject : TintObject
{
	[SerializeField]
	private MeshRenderer meshRendererToTint;

	[SerializeField]
	private Material materialCylinderToTint;

	private Color OriginalColor;

	private void Awake()
	{
		meshRendererToTint.materials = new Material[1] { materialCylinderToTint };
		OriginalColor = materialCylinderToTint.color;
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
			Tint(0.4f, 0.6f, 1f, 0f);
			break;
		case MVTeam.Red:
			Tint(0.855f, 0f, 0f, 0f);
			break;
		case MVTeam.Green:
			Tint(0f, 0.655f, 0f, 0f);
			break;
		case MVTeam.Yellow:
			Tint(1f, 1f, 0f, 0f);
			break;
		default:
			Tint(OriginalColor);
			break;
		}
	}

	public override void Tint(Color c)
	{
		materialCylinderToTint.color = c;
	}
}
