using MV.WorldObject;
using UnityEngine;

public class SpawnRolePlateTintObject : TintObject
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

	public override void TeamTint(MVTeam team)
	{
		switch (team)
		{
		case MVTeam.Blue:
			Tint(Styles.GetColor(ColorStyle.TeamBlue));
			break;
		case MVTeam.Red:
			Tint(Styles.GetColor(ColorStyle.TeamRed));
			break;
		case MVTeam.Green:
			Tint(Styles.GetColor(ColorStyle.TeamGreen));
			break;
		case MVTeam.Yellow:
			Tint(Styles.GetColor(ColorStyle.TeamYellow));
			break;
		default:
			Tint(Styles.GetColor(ColorStyle.OffWhite));
			break;
		}
	}

	public override void Tint(Color c)
	{
		materialToTint.color = c;
	}
}
