public class AvatarRespawnHandler
{
	private MVAvatarLocal mvAvatar;

	private bool shouldRespawnAsGhost = true;

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
			if (MVGameControllerBase.FlagDebriefingControl.IsInFlagDebriefing)
			{
				return;
			}
			mvAvatar.SetMode(AvatarRuntimeState.Ghost);
		}
		else if (MVGameControllerBase.FlagDebriefingControl.IsInFlagDebriefing)
		{
			mvAvatar.SetMode(AvatarRuntimeState.TimeAttackFlagDebriefing);
		}
		else
		{
			mvAvatar.SetMode(AvatarRuntimeState.Playing);
		}
		shouldRespawnAsGhost = true;
	}
}
