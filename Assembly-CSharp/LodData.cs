public struct LodData
{
	public static int idCounter;

	public int id = idCounter;

	public float activateDistance;

	public bool isVisible;

	public bool shadows;

	public LodData(float activateDistance, bool isVisible, bool shadows)
	{
		idCounter++;
		this.activateDistance = activateDistance;
		this.isVisible = isVisible;
		this.shadows = shadows;
	}
}
