using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class AdvancedGhostTintObject : TintObject
{
	[SerializeField]
	private List<OculusTeamGameObject> teamIrisObjects;

	public override void TeamTint(MVTeam team)
	{
		MVTeam mVTeam = ((team != MVTeam.None) ? team : MVTeam.Server);
		for (int i = 0; i < teamIrisObjects.Count; i++)
		{
			teamIrisObjects[i].gameobject.SetActive(value: false);
			if (teamIrisObjects[i].team == mVTeam)
			{
				teamIrisObjects[i].gameobject.SetActive(value: true);
			}
		}
	}

	public override void Tint(Color c)
	{
		Debug.LogWarning("Attempting to tint oculus object, this is not supposed to be called due to changes to oculus teams.");
	}
}
