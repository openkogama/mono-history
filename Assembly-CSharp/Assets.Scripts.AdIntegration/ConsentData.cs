namespace Assets.Scripts.AdIntegration;

public class ConsentData
{
	public bool isAmerican = true;

	public bool isEuropean = true;

	public bool isChild;

	public bool hasConsented;

	public override string ToString()
	{
		return $"isAmerican {isAmerican}. isEuropean {isEuropean}. isChild {isChild}. hasConSented {hasConsented}.";
	}
}
