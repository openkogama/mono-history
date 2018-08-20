using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CubeModelPopup : MonoBehaviour
{
	[SerializeField]
	private RawImage scale25Percent;

	[SerializeField]
	private RawImage scale50Percent;

	[SerializeField]
	private RawImage scale100Percent;

	private byte materialID;

	public void Initialize(byte currentMaterialId)
	{
		materialID = currentMaterialId;
		MVMaterial material = MVGameControllerBase.Game.MaterialRepository.GetMaterial(currentMaterialId);
		if (material.IsDestructible || !material.isUnlocked)
		{
			material = MVGameControllerBase.Game.MaterialRepository.GetMaterial(21);
			materialID = 21;
		}
		MaterialButtonTextureGenerator materialButtonTextureGenerator = Object.Instantiate(PrefabPool.Instance.MaterialButtonTextureGenerator);
		materialButtonTextureGenerator.previewResolution = 180;
		Texture2D texture = materialButtonTextureGenerator.TakePicture(material.Mesh);
		Object.Destroy(materialButtonTextureGenerator.gameObject);
		scale25Percent.texture = texture;
		scale50Percent.texture = texture;
		scale100Percent.texture = texture;
	}

	public void OnModelScalePressed(float scale)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IMaterialClicked x, BaseEventData y) =>
		{
			x.OnMaterialClicked(materialID);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ICreateNewPrototype x, BaseEventData y) =>
		{
			x.OnAddNewPrototype(string.Empty, scale);
		});
	}
}
