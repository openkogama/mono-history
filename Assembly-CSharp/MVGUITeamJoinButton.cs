using MV.WorldObject;
using UnityEngine;

public class MVGUITeamJoinButton : MonoBehaviour
{
	public Material bluePlayerIcon;

	public Material redPlayerIcon;

	public Material greenPlayerIcon;

	public Material yellowPlayerIcon;

	public UXText joinText;

	private MVTeam team;

	public void InitializeJoinButton(MVTeam team, MVGUITeamList playerList)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		this.team = team;
		joinText.ShadowColor = GetShadowColor(playerList);
		((Component)this).renderer.material.SetColor("_Color", GetColor(playerList));
		SetIconMaterial();
	}

	private Color GetColor(MVGUITeamList playerList)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Color result = Color.gray;
		if (team == MVTeam.Blue)
		{
			result = playerList.blueTeamColor;
		}
		else if (team == MVTeam.Red)
		{
			result = playerList.redTeamColor;
		}
		else if (team == MVTeam.Green)
		{
			result = playerList.greenTeamColor;
		}
		else if (team == MVTeam.Yellow)
		{
			result = playerList.yellowTeamColor;
		}
		return result;
	}

	private Color GetShadowColor(MVGUITeamList playerList)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Color result = Color.gray;
		if (team == MVTeam.Blue)
		{
			result = playerList.blueHeaderTextColor;
		}
		else if (team == MVTeam.Red)
		{
			result = playerList.redHeaderTextColor;
		}
		else if (team == MVTeam.Green)
		{
			result = playerList.greenHeaderTextColor;
		}
		else if (team == MVTeam.Yellow)
		{
			result = playerList.yellowHeaderTextColor;
		}
		return result;
	}

	private void SetIconMaterial()
	{
		Material material;
		if (team != MVTeam.Blue)
		{
			if (team == MVTeam.Red)
			{
				material = redPlayerIcon;
			}
			else
			{
				material = ((team != MVTeam.Green) ? yellowPlayerIcon : greenPlayerIcon);
			}
		}
		else
		{
			material = bluePlayerIcon;
		}
		((Component)UXUtils.FindChild(((Component)this).gameObject, "PlayerPicture").GetComponent<UXPlane>()).renderer.material = material;
	}
}
