using MV.WorldObject;
using UnityEngine;

public abstract class TintObject : MonoBehaviour
{
	public abstract void Tint(Color c);

	public virtual void TeamTint(MVTeam team)
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
			Tint(1f, 1f, 1f);
			break;
		}
	}

	public virtual void Tint(float r, float g, float b, float a = 1f)
	{
		Tint(new Color(r, g, b, a));
	}
}
