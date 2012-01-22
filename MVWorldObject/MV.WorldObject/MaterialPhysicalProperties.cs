namespace MV.WorldObject;

public static class MaterialPhysicalProperties
{
	private static PhysicalProperties physicalPropertiesDefault = new PhysicalProperties(0.43f, 0f, 0f, 1f, 20f, float.PositiveInfinity);

	private static PhysicalProperties[] physicalProperties = new PhysicalProperties[28]
	{
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		new PhysicalProperties(0.43f, 0f, 0f, 1f, 20f, 50f),
		new PhysicalProperties(0.43f, 0f, 0f, 1f, 20f, 50f),
		new PhysicalProperties(0.43f, 0f, 0f, 1f, 20f, 50f),
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		physicalPropertiesDefault,
		new PhysicalProperties(0.43f, 0f, 0f, 1f, 20f, 50f),
		new PhysicalProperties(0.43f, 0f, 0f, 1f, 20f, 50f),
		new PhysicalProperties(0.43f, 0f, 0f, 1f, 20f, 50f),
		new PhysicalProperties(0.43f, 0f, 0f, 1f, 20f, 50f),
		new PhysicalProperties(0.43f, 0f, 0f, 1f, 20f, 50f),
		new PhysicalProperties(0f, 0f, 0f, 1f, 0f, float.PositiveInfinity),
		new PhysicalProperties(0.43f, 0f, 10f, 1f, 0f, float.PositiveInfinity),
		new PhysicalProperties(0.43f, 1f, 0f, 0f, 20f, float.PositiveInfinity)
	};

	public static PhysicalProperties PhysicalPropertiesDefault => physicalPropertiesDefault;

	public static PhysicalProperties GetPhysicalProperty(int id)
	{
		return physicalProperties[id];
	}
}
