public class Boost
{
	public BoostType Type { get; private set; }

	public object Value { get; private set; }

	public string Description { get; private set; }

	public bool AllowedForGame { get; set; }

	public Boost(BoostType type, string desc, bool allowedForGame, object val)
	{
		Type = type;
		Value = val;
		Description = desc;
		AllowedForGame = allowedForGame;
	}
}
