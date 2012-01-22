public struct LodData
{
	public static int idCounter;

	public int id = idCounter;

	public float activateDistance;

	public bool isVisible;

	public MeshSetting mipMeshSetting;

	public bool shadows;

	public LodData(float activateDistance, bool isVisible, MeshSetting mipMeshSetting, bool shadows)
	{
		idCounter++;
		this.activateDistance = activateDistance;
		this.isVisible = isVisible;
		this.mipMeshSetting = mipMeshSetting;
		this.shadows = shadows;
	}
}
