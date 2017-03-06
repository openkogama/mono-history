namespace MV.WorldObject;

public interface IWorldObjectManager
{
	MVWorldObject GetWorldObject(int id);

	bool TryGetWorldObject(int id, out MVWorldObject worldObject);
}
