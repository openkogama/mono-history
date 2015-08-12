using UnityEngine;

public class MVGUIBottomCenterToggles : UXViewScript
{
	[SerializeField]
	private MVGUIRespawnButton respawnButton;

	[SerializeField]
	private MVGUIChangeTeamButton changeTeamButton;

	public override void OnInitialize()
	{
		base.OnInitialize();
		respawnButton.Initialize();
		changeTeamButton.Initialize();
	}

	private void Update()
	{
		changeTeamButton.button.SetVisible(MVGameController.Game.TeamManager.TeamCount() > 1);
	}
}
