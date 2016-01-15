public class LoadStats
{
	public double DOMReady;

	public double PluginInit;

	public double GameStartTime;

	public override string ToString()
	{
		return $"DomReady {DOMReady}. PluginInit {PluginInit}. GameStartTime {GameStartTime}.";
	}
}
