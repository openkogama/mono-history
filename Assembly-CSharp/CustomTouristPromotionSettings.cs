public class CustomTouristPromotionSettings
{
	public int FrequencyPercent { get; private set; }

	public string URL { get; private set; }

	public string AssetURL { get; private set; }

	public bool Redirect { get; private set; }

	public CustomTouristPromotionSettings(int frequency, string url, string asset, bool shouldRedirect)
	{
		FrequencyPercent = frequency;
		URL = url;
		AssetURL = asset;
		Redirect = shouldRedirect;
	}
}
