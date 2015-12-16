using UnityEngine;

public struct EquipableData(GameObject obj, AvatarEquipableType equipType)
{
	public GameObject prefabObject = obj;

	public AvatarEquipableType equipableType = equipType;
}
