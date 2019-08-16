namespace GoogleMobileAds.Api.Mediation.Vungle;

public abstract class VungleMediationExtras : MediationExtras
{
	public const string AllPlacementsKey = "all_placements";

	public const string UserIdKey = "user_id";

	public const string SoundEnabledKey = "sound_enabled";

	public override string IOSMediationExtraBuilderClassName => "VungleExtrasBuilder";

	public VungleMediationExtras()
	{
	}

	public void SetAllPlacements(string[] allPlacements)
	{
		Extras.Add("all_placements", string.Join(",", allPlacements));
	}

	public void SetUserId(string userId)
	{
		Extras.Add("user_id", userId);
	}

	public void SetSoundEnabled(bool soundEnabled)
	{
		Extras.Add("sound_enabled", soundEnabled.ToString());
	}
}
