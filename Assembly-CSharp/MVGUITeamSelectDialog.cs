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
		List<MVTeam> teamList = MVGameController.Instance.Game.TeamManager.GetTeamList();
		NoOfTeams = teamList.Count;
		if (NoOfTeams == 1)
		{
			returnTeam = MVTeam.None;
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CloseDialog();
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
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		MVGUITeamList mVGUITeamList = ((NoOfTeams != 2) ? QuarterTeamList : HalfTeamList);
		MVGUITeamList mVGUITeamList2 = Object.Instantiate((Object)(object)mVGUITeamList) as MVGUITeamList;
		((Component)mVGUITeamList2).transform.parent = TeamListRoot;
		((Component)mVGUITeamList2).transform.localScale = Vector3.one;
		((Component)mVGUITeamList2).transform.localPosition = GetTeamListPosition(mVGUITeamList2);
		mVGUITeamList2.InitializeTeamList(team);
		UXIconButton uXIconButton = Object.Instantiate((Object)(object)TeamSelectButton) as UXIconButton;
		((Component)uXIconButton).transform.parent = ((Component)mVGUITeamList2).transform;
		((Component)uXIconButton).transform.localScale = Vector3.one;
		((Component)uXIconButton).transform.localPosition = GetButtonPosition();
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			SetTeamAndClose(team);
		}));
		((Component)uXIconButton).GetComponent<MVGUITeamJoinButton>().InitializeJoinButton(team, mVGUITeamList2);
		AddedTeams++;
	}

	private Vector3 GetTeamListPosition(MVGUITeamList playerList)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 fullSize = playerList.GetFullSize();
		if (NoOfTeams == 2)
		{
			float num = ((AddedTeams != 0) ? 0f : (0f - fullSize.x));
			return new Vector3(num, fullSize.y / 2f, 0f);
		}
		float num2 = ((AddedTeams % 2 != 0) ? 0f : (0f - fullSize.x));
		float num3 = ((AddedTeams >= 2) ? 0f : fullSize.y);
		return new Vector3(num2, num3, 0f);
	}

	private Vector3 GetButtonPosition()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (NoOfTeams == 2)
		{
			return new Vector3(14f, -10f, -0.1f);
		}
		return new Vector3(14f, -5f, -0.1f);
	}

	private void SetTeamAndClose(MVTeam team)
	{
		returnTeam = team;
		UXUtils.FindGUIObjectOfType<UXDialogFactory>().CloseDialog();
	}

	public override object GetResult()
	{
		return returnTeam;
	}
}
