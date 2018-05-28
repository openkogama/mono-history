namespace MV.WorldObject.ThemesData;

public class ThemeData
{
	public int id;

	public string themeIdentifier;

	public int priceGold;

	public int levelRequirement;

	public ThemeData()
	{
	}

	public ThemeData(int id, string themeIdentifier, int priceGold, int levelRequirement)
	{
		this.id = id;
		this.themeIdentifier = themeIdentifier;
		this.priceGold = priceGold;
		this.levelRequirement = levelRequirement;
	}

	public override string ToString()
	{
		return $"id {id}. themeIdentifier {themeIdentifier}. priceGold {priceGold}. levelRequirement {levelRequirement}.";
	}
}
