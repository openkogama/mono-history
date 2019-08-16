namespace GoogleMobileAds.Api.Mediation.Vungle;

public class VungleInterstitialMediationExtras : VungleMediationExtras
{
	public override string AndroidMediationExtraBuilderClassName => "com.google.unity.mediation.vungle.VungleUnityInterstitialExtrasBuilder";
}
