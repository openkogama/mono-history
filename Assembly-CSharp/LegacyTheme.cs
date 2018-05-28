public class LegacyTheme : Theme
{
	public override string Identifier => "legacy";

	public override string DisplayName => TM._("Placeholder");

	public override string Description => TM._("This is theme is a placeholder, which doesn't do anything.");

	protected override void InitializeAttributes()
	{
	}

	protected override void InitializeComponents()
	{
	}
}
