public class ElitePromotionSettings
{
	public bool ElitePromotionEnabled { get; private set; }

	public int ElitePromotionInterval { get; private set; }

	public ElitePromotionSettings(bool isEnabled, int interval)
	{
		ElitePromotionEnabled = isEnabled;
		ElitePromotionInterval = interval;
	}
}
