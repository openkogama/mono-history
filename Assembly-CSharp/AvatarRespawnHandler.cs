public class AvatarRespawnHandler
{
	private MVAvatarLocal mvAvatar;

	private bool shouldRespawnAsGhost;

	public bool ShouldRespawnAsGhost
	{
		set
		{
			shouldRespawnAsGhost = value;
		}
	}

	public void Initialize(MVAvatarLocal mvAvatar)
	{
		this.mvAvatar = mvAvatar;
	}

	public void Respawn()
	{
		if (shouldRespawnAsGhost)
		{
			MVGameControllerBase.PlayModeUI.InLobbyState = true;
			mvAvatar.SetMode(AvatarRuntimeState.Ghost);
		}
		else if (FlagDebriefingControl.IsInFlagDebriefing)
		{
			mvAvatar.SetMode(AvatarRuntimeState.TimeAttackFlagDebriefing);
		}
		else
		{
			mvAvatar.SetMode(AvatarRuntimeState.Playing);
		}
	}
}
