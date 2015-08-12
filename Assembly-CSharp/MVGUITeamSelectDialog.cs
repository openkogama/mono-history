using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVGUITeamSelectDialog : UXCustomDialogBox
{
	public MVGUITeamList HalfTeamList;

	public MVGUITeamList QuarterTeamList;

	public Transform TeamListRoot;

	public UXIconButton TeamSelectButton;

	private MVTeam returnTeam;

	private int NoOfTeams;

	private int AddedTeams;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		BuildTeamLists();
	}

	private void BuildTeamLists()
	{
		List<MVTeam> teamList = MVGameController.Game.TeamManager.GetTeamList();
		NoOfTeams = teamList.Count;
		if (NoOfTeams == 1)
		{
			returnTeam = teamList[0];
			UXUtils.UXDialogFactory.CloseDialog();
			return;
		}
		AddedTeams = 0;
		foreach (MVTeam item in teamList)
		{
			AddTeamList(item);
		}
	}

	private void AddTeamList(MVTeam team)
	{
		MVGUITeamList original = ((NoOfTeams != 2) ? QuarterTeamList : HalfTeamList);
		MVGUITeamList mVGUITeamList = UnityEngine.Object.Instantiate(original);
		mVGUITeamList.transform.parent = TeamListRoot;
		mVGUITeamList.transform.localScale = Vector3.one;
		mVGUITeamList.transform.localPosition = GetTeamListPosition(mVGUITeamList);
		mVGUITeamList.InitializeTeamList(team);
		UXIconButton uXIconButton = UnityEngine.Object.Instantiate(TeamSelectButton);
		uXIconButton.transform.parent = mVGUITeamList.transform;
		uXIconButton.transform.localScale = Vector3.one;
		uXIconButton.transform.localPosition = GetButtonPosition();
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			SetTeamAndClose(team);
		}));
		uXIconButton.GetComponent<MVGUITeamJoinButton>().InitializeJoinButton(team, mVGUITeamList);
		AddedTeams++;
	}

	private Vector3 GetTeamListPosition(MVGUITeamList playerList)
	{
		Vector2 fullSize = playerList.GetFullSize();
		if (NoOfTeams == 2)
		{
			float x = ((AddedTeams != 0) ? 0f : (0f - fullSize.x));
			return new Vector3(x, fullSize.y / 2f, 0f);
		}
		float x2 = ((AddedTeams % 2 != 0) ? 0f : (0f - fullSize.x));
		float y = ((AddedTeams >= 2) ? 0f : fullSize.y);
		return new Vector3(x2, y, 0f);
	}

	private Vector3 GetButtonPosition()
	{
		if (NoOfTeams == 2)
		{
			return new Vector3(14f, -10f, -0.1f);
		}
		return new Vector3(14f, -5f, -0.1f);
	}

	private void SetTeamAndClose(MVTeam team)
	{
		returnTeam = team;
		UXUtils.UXDialogFactory.CloseDialog();
	}

	public override object GetResult()
	{
		return returnTeam;
	}
}
