public class CreySettings
{
	public int TouristPromotionCreyFrequencyPercent { get; private set; }

	public string TouristPromotionCreyURL { get; private set; }

	public bool TouristPromotionCreyRedirect { get; private set; }

	public CreySettings(int frequency, string url, bool shouldRedirect)
	{
		TouristPromotionCreyFrequencyPercent = frequency;
		TouristPromotionCreyURL = url;
		TouristPromotionCreyRedirect = shouldRedirect;
	}
}
