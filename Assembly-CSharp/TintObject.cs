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
			Tint(0f, 0f, 1f);
			break;
		case MVTeam.Red:
			Tint(1f, 0f, 0f);
			break;
		case MVTeam.Green:
			Tint(0f, 1f, 0f);
			break;
		case MVTeam.Yellow:
			Tint(1f, 1f, 0f);
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
