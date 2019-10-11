namespace GoogleMobileAds.Api.Mediation.AdColony;

public class AdColonyMediationExtras : MediationExtras
{
	public const string ShowPrePopupKey = "show_pre_popup";

	public const string ShowPostPopupKey = "show_post_popup";

	public override string AndroidMediationExtraBuilderClassName => "com.google.unity.mediation.adcolony.AdColonyUnityExtrasBuilder";

	public override string IOSMediationExtraBuilderClassName => "AdColonyExtrasBuilder";

	public void SetShowPrePopup(bool showPrePopup)
	{
		Extras.Add("show_pre_popup", showPrePopup.ToString());
	}

	public void SetShowPostPopup(bool showPostPopup)
	{
		Extras.Add("show_post_popup", showPostPopup.ToString());
	}
}
