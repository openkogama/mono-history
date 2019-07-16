using MV.WorldObject;
using UnityEngine;

public class PressurePlateTintObject : TintObject
{
	[SerializeField]
	private MeshRenderer meshRendererToTint;

	[SerializeField]
	private Material materialCylinderToTint;

	[SerializeField]
	private Texture teamTexture;

	[SerializeField]
	private Texture defaultTexture;

	private Color OriginalColor = new Color(1f, 1f, 1f, 0f);

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
			Tint(0.01f, 0.54f, 1f, 0f);
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
		case MVTeam.None:
			Tint(OriginalColor);
			break;
		default:
			Tint(OriginalColor);
			break;
		}
	}

	public override void Tint(Color c)
	{
		if (c == OriginalColor)
		{
			materialCylinderToTint.mainTexture = defaultTexture;
		}
		else
		{
			materialCylinderToTint.mainTexture = teamTexture;
		}
		materialCylinderToTint.color = c;
	}
}
