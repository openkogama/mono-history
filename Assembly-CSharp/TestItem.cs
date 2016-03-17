using UnityEngine;
using UnityEngine.UI;

public class TestItem : MonoBehaviour
{
	[SerializeField]
	private Text text;

	[SerializeField]
	private RawImage image;

	[SerializeField]
	private InventoryItemMetaData inventoryItemMetaData;

	[SerializeField]
	private PreviewObject previewObject;

	public Texture2D Texture
	{
		set
		{
			image.texture = value;
		}
	}

	public void Initialize(string text, int slotIndex)
	{
		this.text.text = text;
		inventoryItemMetaData.Initialize(slotIndex);
		previewObject = Object.Instantiate(previewObject);
		image.texture = previewObject.RenderTexture;
	}

	private void OnDestroy()
	{
		if (previewObject != null)
		{
			Object.Destroy(previewObject.gameObject);
		}
	}
}
