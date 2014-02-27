using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class EditorController : AEditController
{
	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(EditorController));

	private MVGUIPublishButton publish;

	public EditorController()
	{
		EditorStateMachine.Event = EditorEvent.ESTerrainEdit;
	}

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		GameObject gameObject = ((Component)UXUtils.FindGUIObjectOfType<MVGUIEditor>()).gameObject;
		publish = gameObject.GetComponentInChildren<MVGUIPublishButton>();
	}

	public override void Initialize()
	{
		base.Initialize();
		logger.Log("Initialize");
		publish.Initialize();
	}

	protected override void SetPlayInEditorMode(bool playInEditor)
	{
		base.SetPlayInEditorMode(playInEditor);
		if (playInEditor)
		{
			MVTeam team = MVGameController.Instance.Game.LocalPlayer.Team;
			MVTeamManager teamManager = MVGameController.Instance.Game.TeamManager;
			if (team == MVTeam.None || !teamManager.IsTeamActive(team) || teamManager.TeamCount() == 1)
			{
				List<MVTeam> teamList = teamManager.GetTeamList();
				MVGameController.Instance.Game.SetTeam((teamList.Count != 1) ? teamList[0] : MVTeam.None);
			}
			ShowBriefing();
		}
		else
		{
			ClearGameMsg();
		}
		menu.playersWindow.UpdateTeamLists();
	}

	public override void Deinitialize()
	{
		logger.Log("Deinitialize");
		RemoveUI();
	}

	public int GetSettingsDialogSelectionWOID()
	{
		if (EditorStateMachine.SingleSelectedWO != null)
		{
			return EditorStateMachine.SingleSelectedWO.Id;
		}
		return -1;
	}

	public MVWorldObjectClient GetSettingsDialogWOById(int woID)
	{
		if (woID == -1)
		{
			return null;
		}
		return MVGameController.Instance.WOCM.GetWorldObjectClient(woID);
	}

	public MVWorldObjectClient GetSettingsDialogSelectionWO()
	{
		int settingsDialogSelectionWOID = GetSettingsDialogSelectionWOID();
		if (settingsDialogSelectionWOID == -1)
		{
			return null;
		}
		return MVGameController.Instance.WOCM.GetWorldObjectClient(settingsDialogSelectionWOID);
	}

	public override void SetGameMsg()
	{
		if (MVGameController.Instance.Game.IsPlaying)
		{
			base.SetGameMsg();
		}
	}
}
