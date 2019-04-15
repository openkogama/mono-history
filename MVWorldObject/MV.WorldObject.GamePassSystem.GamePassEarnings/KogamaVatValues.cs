namespace MV.WorldObject.GamePassSystem.GamePassEarnings;

public class KogamaVatValues
{
	public float regularUserVat;

	public float subscribedUserVat;

	public KogamaVatValues()
	{
	}

	public KogamaVatValues(float regularUserVat, float subscribedUserVat)
	{
		this.regularUserVat = regularUserVat;
		this.subscribedUserVat = subscribedUserVat;
	}

	public override string ToString()
	{
		return $"regularUserVat {regularUserVat}. subscribedUserVat {subscribedUserVat}.";
	}
}
